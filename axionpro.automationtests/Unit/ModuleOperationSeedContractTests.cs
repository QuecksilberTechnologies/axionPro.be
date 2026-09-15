using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("ModuleOperationSeed")]
public sealed class ModuleOperationSeedContractTests
{
    [Test]
    public void Consolidated_seed_keeps_canonical_tenant_dashboard_and_completes_module_metadata()
    {
        var repositoryRoot = FindRepositoryRoot();
        var authoritativeSeed = Path.Combine(
            repositoryRoot,
            "database-scripts",
            "complete seed data",
            "AxionPro_New_Production_Module_Operation_Seed.sql");
        var removedDuplicateSeed = Path.Combine(
            repositoryRoot,
            "database-scripts",
            "AxionPro_New_Production_Module_Operation_Seed.sql");

        var sql = File.ReadAllText(authoritativeSeed);

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(removedDuplicateSeed), Is.False);
            Assert.That(sql, Does.Contain("'TENANT_DASHBOARD'"));
            Assert.That(sql, Does.Not.Contain("'HOST_DASHBOARD'"));
            Assert.That(sql, Does.Contain("$module_presentation_metadata_validation$"));
            Assert.That(sql, Does.Contain("NULLIF(BTRIM(\"ImageIconWeb\"), '') IS NULL"));
            Assert.That(sql, Does.Contain("NULLIF(BTRIM(\"ImageIconMobile\"), '') IS NULL"));
            Assert.That(sql, Does.Contain("NULLIF(BTRIM(\"Remark\"), '') IS NULL"));
            Assert.That(sql, Does.Contain("Expected one active Add and zero Create operations"));
        });
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")) &&
                Directory.Exists(Path.Combine(directory.FullName, "database-scripts")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root containing database-scripts was not found.");
    }
}

[TestFixture]
[Category("ModuleOperationSeedDatabase")]
[NonParallelizable]
public sealed class ModuleOperationSeedDatabaseTests
{
    private NpgsqlConnection _connection = null!;

    [SetUp]
    public async Task SetUp()
    {
        var connectionString = Environment.GetEnvironmentVariable("AXIONPRO_BULK_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Ignore("Requires disposable axionpro_bulk_test database.");
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        Assert.Multiple(() =>
        {
            Assert.That(builder.Database, Is.EqualTo("axionpro_bulk_test"));
            Assert.That(builder.Host, Is.AnyOf("127.0.0.1", "localhost"));
        });

        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    [Test]
    public async Task Module_metadata_is_complete_and_create_alias_is_absent()
    {
        var incompleteModules = await ScalarAsync("""
            SELECT count(*)
            FROM axionpro."Module"
            WHERE nullif(btrim("ImageIconWeb"), '') IS NULL
               OR nullif(btrim("ImageIconMobile"), '') IS NULL
               OR nullif(btrim("Remark"), '') IS NULL;
            """);

        var createOperations = await ScalarAsync("""
            SELECT count(*)
            FROM axionpro."Operation"
            WHERE lower(btrim("OperationName")) = 'create';
            """);

        var addOperations = await ScalarAsync("""
            SELECT count(*)
            FROM axionpro."Operation"
            WHERE lower(btrim("OperationName")) = 'add'
              AND "OperationType" = 1
              AND "IsActive";
            """);

        Assert.Multiple(() =>
        {
            Assert.That(incompleteModules, Is.Zero);
            Assert.That(createOperations, Is.Zero);
            Assert.That(addOperations, Is.EqualTo(1));
        });
    }

    private async Task<long> ScalarAsync(string sql)
    {
        await using var command = new NpgsqlCommand(sql, _connection);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }
}
