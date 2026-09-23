using System.Reflection;
using axionpro.application.Features.EmployeeCmd;
using axionpro.application.Features.EmployeeCmd.ResetPassword.Handlers;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("ResetPasswordModuleConsolidation")]
public sealed class ResetPasswordModuleConsolidationTests
{
    [Test]
    public void Reset_password_request_is_authorized_by_employee_list_module()
    {
        var behaviorType = typeof(EmployeeTenantPermissionBehavior<,>)
            .MakeGenericType(typeof(ResetPasswordCommand), typeof(object));
        var resolver = behaviorType.GetMethod(
            "ResolveExpectedModuleCode",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new AssertionException("Employee permission module resolver was not found.");

        Assert.That(resolver.Invoke(null, null), Is.EqualTo("EMP_LIST"));
    }

    [Test]
    public void Migration_and_seed_remove_module_38_and_map_operation_21_to_employee_list()
    {
        var root = FindRepositoryRoot();
        var migration = File.ReadAllText(Path.Combine(
            root, "database-scripts", "MoveResetPasswordToEmployeeList.sql"));
        var seed = File.ReadAllText(Path.Combine(
            root, "database-scripts", "complete seed data",
            "AxionPro_New_Production_Module_Operation_Seed.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(migration, Does.Contain("Operation Id 21 must be Reset Password"));
            Assert.That(migration, Does.Contain("module.\"ModuleCode\" = 'EMP_LIST'"));
            Assert.That(migration, Does.Contain("DELETE FROM axionpro.\"TenantEnabledOperation\" WHERE \"ModuleId\" = 38"));
            Assert.That(migration, Does.Contain("DELETE FROM axionpro.\"TenantEnabledModule\" WHERE \"ModuleId\" = 38"));
            Assert.That(migration, Does.Contain("DELETE FROM axionpro.\"ModuleOperationMapping\" WHERE \"ModuleId\" = 38"));
            Assert.That(seed, Does.Contain("RESET PASSWORD OPERATION ON EMP_LIST"));
            Assert.That(seed, Does.Not.Contain("'Employee-Password-Management'"));
            Assert.That(seed, Does.Not.Contain("WHERE legacy.\"ModuleId\" = 38"));
        });
    }

    [Test]
    public async Task Configured_database_has_no_module_38_and_employee_list_owns_reset_password()
    {
        var connectionString = Environment.GetEnvironmentVariable("AXIONPRO_MODULE_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Ignore("Set AXIONPRO_MODULE_TEST_CONNECTION to run the PostgreSQL state check.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                (SELECT count(*) FROM axionpro."Module"
                 WHERE "Id" = 38 OR "ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'),
                (SELECT count(*) FROM axionpro."ModuleOperationMapping"
                 WHERE "ModuleId" = (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" = 'EMP_LIST')
                   AND "OperationId" = 21 AND "IsActive" = true),
                (SELECT count(*) FROM axionpro."TenantEnabledModule" WHERE "ModuleId" = 38),
                (SELECT count(*) FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" = 38),
                (SELECT count(*) FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" = 38),
                (SELECT count(*) FROM axionpro."PlanModuleMapping" WHERE "ModuleId" = 38),
                (SELECT count(*) FROM axionpro."Operation"
                 WHERE "Id" = 21 AND "OperationName" = 'Reset Password' AND "IsActive" = true),
                (SELECT count(*) FROM axionpro."TenantEnabledOperation"
                 WHERE "ModuleId" = (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" = 'EMP_LIST')
                   AND "OperationId" = 21),
                (SELECT count(*) FROM axionpro."RoleModuleAndPermission"
                 WHERE "ModuleId" = (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" = 'EMP_LIST')
                   AND "OperationId" = 21 AND "IsSoftDeleted" IS DISTINCT FROM true);
            """;

        await using var reader = await command.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetInt64(0), Is.Zero, "Module 38 must be absent.");
            Assert.That(reader.GetInt64(1), Is.EqualTo(1), "EMP_LIST must own one active Reset Password mapping.");
            Assert.That(reader.GetInt64(2), Is.Zero);
            Assert.That(reader.GetInt64(3), Is.Zero);
            Assert.That(reader.GetInt64(4), Is.Zero);
            Assert.That(reader.GetInt64(5), Is.Zero);
            Assert.That(reader.GetInt64(6), Is.EqualTo(1), "Operation Id 21 must remain active.");
            Assert.That(reader.GetInt64(7), Is.GreaterThanOrEqualTo(1), "Tenant entitlement must be on EMP_LIST.");
            Assert.That(reader.GetInt64(8), Is.GreaterThanOrEqualTo(1), "Role grant must be on EMP_LIST.");
        });
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate AxionPro.sln.");
    }
}
