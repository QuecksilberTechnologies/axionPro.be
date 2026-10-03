using System.Reflection;
using System.Text.RegularExpressions;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.infrastructure.EncryptionService;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeBankEncryption")]
public sealed class EmployeeBankEncryptionTests
{
    private static readonly Regex AccountNumberPattern = new(@"^\d{9,18}$", RegexOptions.CultureInvariant);
    private static readonly Regex IfscPattern = new(@"^[A-Z]{4}0[A-Z0-9]{6}$", RegexOptions.CultureInvariant);
    private static readonly Regex UpiPattern = new(@"^[a-zA-Z0-9.\-_]{2,256}@[a-zA-Z]{2,64}$", RegexOptions.CultureInvariant);

    [Test]
    public void Bank_projection_decrypts_sensitive_values_for_the_existing_masked_UI_contract()
    {
        const string tenantKey = "test-tenant-key";
        var encryption = new AesEncryptionService();
        var source = new PagedResponseDTO<GetBankResponseDTO>
        {
            Data = new List<GetBankResponseDTO>
            {
                new()
                {
                    EmployeeId = "42",
                    AccountNumber = encryption.Encrypt("501234567890", tenantKey),
                    IFSCCode = encryption.Encrypt("HDFC0001234", tenantKey),
                    UPIId = encryption.Encrypt("employee@bank", tenantKey)
                }
            }
        };
        var encoder = Proxy<axionpro.application.Interfaces.IEncryptionService.IIdEncoderService>((method, arguments) =>
            method.Name == "EncodeId_long" ? $"encoded-{arguments![0]}" : throw new AssertionException(method.Name));
        var storage = Proxy<IFileStorageService>((method, _) =>
            throw new AssertionException("Unexpected storage call: " + method.Name));

        var result = ProjectionHelper.ToGetBankResponseDTOs(
            source,
            encoder,
            encryption,
            tenantKey,
            new ConfigurationBuilder().Build(),
            storage).Single();

        Assert.Multiple(() =>
        {
            Assert.That(result.EmployeeId, Is.EqualTo("encoded-42"));
            Assert.That(result.AccountNumber, Is.EqualTo("501234567890"));
            Assert.That(result.IFSCCode, Is.EqualTo("HDFC0001234"));
            Assert.That(result.UPIId, Is.EqualTo("employee@bank"));
        });
    }

    [Test]
    [Category("EmployeeBankEncryptionDatabase")]
    [NonParallelizable]
    public async Task Schema_and_existing_bank_values_are_encrypted()
    {
        var settingsPath = Environment.GetEnvironmentVariable("AXIONPRO_BANK_DB_SETTINGS");
        if (string.IsNullOrWhiteSpace(settingsPath))
            Assert.Ignore("Opt-in Employee bank encryption database migration.");

        var configuration = new ConfigurationBuilder().AddJsonFile(settingsPath!).Build();
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var scriptPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..",
            "database-scripts", "EncryptEmployeeBankSensitiveFields.sql");
        var schemaSql = (await File.ReadAllTextAsync(Path.GetFullPath(scriptPath)))
            .Replace("BEGIN;", string.Empty, StringComparison.Ordinal)
            .Replace("COMMIT;", string.Empty, StringComparison.Ordinal);
        await using (var schema = new NpgsqlCommand(schemaSql, connection, transaction))
            await schema.ExecuteNonQueryAsync();

        var rows = new List<BankRow>();
        const string selectSql = """
            SELECT bank."Id", bank."AccountNumber", bank."IFSCCode", bank."UPIId", tenantKey."EncryptionKey"
            FROM axionpro."EmployeeBankDetail" bank
            INNER JOIN axionpro."Employee" employee ON employee."Id" = bank."EmployeeId"
            INNER JOIN axionpro."TenantEncryptionKeys" tenantKey ON tenantKey."TenantId" = employee."TenantId";
            """;
        await using (var select = new NpgsqlCommand(selectSql, connection, transaction))
        await using (var reader = await select.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                rows.Add(new BankRow(
                    reader.GetInt32(0),
                    reader.IsDBNull(1) ? null : reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    reader.GetString(4)));
            }
        }

        await using (var count = new NpgsqlCommand(
            "SELECT COUNT(*) FROM axionpro.\"EmployeeBankDetail\";",
            connection,
            transaction))
        {
            var totalRows = Convert.ToInt32(await count.ExecuteScalarAsync());
            Assert.That(rows, Has.Count.EqualTo(totalRows),
                "Every bank row must resolve through Employee to a Tenant encryption key.");
        }

        var encryption = new AesEncryptionService();
        foreach (var row in rows)
        {
            var accountNumber = ProtectOrValidate(row.AccountNumber, row.TenantKey, AccountNumberPattern, encryption, "AccountNumber");
            var ifscCode = ProtectOrValidate(
                row.IfscCode,
                row.TenantKey,
                IfscPattern,
                encryption,
                "IFSCCode",
                value => value.ToUpperInvariant());
            var upiId = ProtectOrValidate(row.UpiId, row.TenantKey, UpiPattern, encryption, "UPIId");

            const string updateSql = """
                UPDATE axionpro."EmployeeBankDetail"
                SET "AccountNumber" = @accountNumber, "IFSCCode" = @ifscCode, "UPIId" = @upiId
                WHERE "Id" = @id;
                """;
            await using var update = new NpgsqlCommand(updateSql, connection, transaction);
            update.Parameters.AddWithValue("id", row.Id);
            update.Parameters.AddWithValue("accountNumber", (object?)accountNumber ?? DBNull.Value);
            update.Parameters.AddWithValue("ifscCode", (object?)ifscCode ?? DBNull.Value);
            update.Parameters.AddWithValue("upiId", (object?)upiId ?? DBNull.Value);
            await update.ExecuteNonQueryAsync();
        }

        const string columnSql = """
            SELECT column_name, character_maximum_length
            FROM information_schema.columns
            WHERE table_schema = 'axionpro'
              AND table_name = 'EmployeeBankDetail'
              AND column_name IN ('AccountNumber', 'IFSCCode', 'UPIId');
            """;
        var lengths = new Dictionary<string, int>();
        await using (var columns = new NpgsqlCommand(columnSql, connection, transaction))
        await using (var reader = await columns.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                lengths[reader.GetString(0)] = reader.GetInt32(1);
        }

        Assert.Multiple(() =>
        {
            Assert.That(lengths["AccountNumber"], Is.EqualTo(128));
            Assert.That(lengths["IFSCCode"], Is.EqualTo(128));
            Assert.That(lengths["UPIId"], Is.EqualTo(512));
        });

        await transaction.CommitAsync();
        TestContext.Out.WriteLine($"Encrypted and verified {rows.Count} EmployeeBankDetail row(s).");
    }

    private static string? ProtectOrValidate(
        string? value,
        string tenantKey,
        Regex plainTextPattern,
        IEncryptionService encryption,
        string fieldName,
        Func<string, string>? normalizePlainText = null)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalizedPlainText = normalizePlainText?.Invoke(value) ?? value;
        if (plainTextPattern.IsMatch(normalizedPlainText))
            return encryption.Encrypt(normalizedPlainText, tenantKey);

        var decrypted = encryption.Decrypt(value, tenantKey);
        Assert.That(plainTextPattern.IsMatch(decrypted), Is.True, $"Existing {fieldName} value is neither valid plaintext nor valid tenant-key ciphertext.");
        return value;
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }

    private sealed record BankRow(int Id, string? AccountNumber, string? IfscCode, string? UpiId, string TenantKey);
}
