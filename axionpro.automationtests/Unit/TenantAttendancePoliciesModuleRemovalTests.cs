using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("TenantAttendancePoliciesModuleRemoval")]
public sealed class TenantAttendancePoliciesModuleRemovalTests
{
    [Test]
    public void Production_seed_does_not_define_attendance_policies_module()
    {
        var seed = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(), "database-scripts", "complete seed data",
            "AxionPro_New_Production_Module_Operation_Seed.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(seed, Does.Not.Contain("TENANT_ATTENDANCE_POLICIES"));
            Assert.That(seed, Does.Not.Contain("Tenant Attendance Policies"));
            Assert.That(seed, Does.Not.Contain("/app/attendance-policies"));
            Assert.That(seed, Does.Contain("Expected nine active Tenant feature modules"));
        });
    }

    [Test]
    public async Task Configured_database_has_no_module_67_dependencies()
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
              (SELECT count(*) FROM axionpro."Module" WHERE "Id"=67 OR "ModuleCode"='TENANT_ATTENDANCE_POLICIES'),
              (SELECT count(*) FROM axionpro."ModuleOperationMapping" WHERE "ModuleId"=67),
              (SELECT count(*) FROM axionpro."TenantEnabledModule" WHERE "ModuleId"=67),
              (SELECT count(*) FROM axionpro."TenantEnabledOperation" WHERE "ModuleId"=67),
              (SELECT count(*) FROM axionpro."PlanModuleMapping" WHERE "ModuleId"=67),
              (SELECT count(*) FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId"=67),
              (SELECT count(*) FROM axionpro."HostRoleModuleAndPermission" WHERE "ModuleId"=67);
            """;

        await using var reader = await command.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        for (var index = 0; index < 7; index++)
        {
            Assert.That(reader.GetInt64(index), Is.Zero, $"Dependency result {index} must be zero.");
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate AxionPro.sln.");
    }
}
