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

    [Test]
    public void Consolidated_seed_groups_master_leaves_under_singular_duplicate_safe_parents()
    {
        var repositoryRoot = FindRepositoryRoot();
        var authoritativeSeed = Path.Combine(
            repositoryRoot,
            "database-scripts",
            "complete seed data",
            "AxionPro_New_Production_Module_Operation_Seed.sql");

        var sql = File.ReadAllText(authoritativeSeed);

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("('TENANT_DEPARTMENT','Tenant-Department','Department'"));
            Assert.That(sql, Does.Contain("('TENANT_DESIGNATION','Tenant-Designation','Designation'"));
            Assert.That(sql, Does.Contain("('TENANT_ROLE','Tenant-Role','Role'"));
            Assert.That(sql, Does.Contain("WHEN 'TENANT_DEPARTMENTS' THEN department_parent_id"));
            Assert.That(sql, Does.Contain("WHEN 'TENANT_DESIGNATIONS' THEN designation_parent_id"));
            Assert.That(sql, Does.Contain("WHEN 'TENANT_ROLES_PERMISSIONS' THEN role_parent_id"));
            Assert.That(sql, Does.Contain("'tenant-departments'"));
            Assert.That(sql, Does.Contain("'tenant-designations'"));
            Assert.That(sql, Does.Contain("'tenant-roles-permissions'"));
            Assert.That(sql, Does.Contain("Each singular parent inherits the plans of its own functional child."));
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

    [Test]
    public async Task Singular_navigation_parents_have_expected_children_without_duplicate_catalogue_rows()
    {
        var validHierarchyRows = await ScalarAsync("""
            SELECT count(*)
            FROM (VALUES
                ('TENANT_DEPARTMENT','TENANT_DEPARTMENTS','tenant-departments'),
                ('TENANT_DESIGNATION','TENANT_DESIGNATIONS','tenant-designations'),
                ('TENANT_ROLE','TENANT_ROLES_PERMISSIONS','tenant-roles-permissions')
            ) expected(parent_code,child_code,child_page_name)
            JOIN axionpro."Module" parent
              ON parent."ModuleCode"=expected.parent_code
             AND parent."ModuleScope"=1
             AND parent."IsActive"
             AND parent."IsModuleDisplayInUI"
             AND NOT parent."IsLeafNode"
             AND parent."URLPath" IS NULL
            JOIN axionpro."Module" child
              ON child."ModuleCode"=expected.child_code
             AND child."ModuleScope"=1
             AND child."ParentModuleId"=parent."Id"
             AND child."PageName"=expected.child_page_name
             AND child."IsLeafNode";
            """);

        var duplicateModules = await ScalarAsync("""
            SELECT count(*) FROM (
                SELECT "ModuleCode","ModuleScope"
                FROM axionpro."Module"
                WHERE "ModuleCode" IN
                    ('TENANT_DEPARTMENT','TENANT_DESIGNATION','TENANT_ROLE')
                GROUP BY "ModuleCode","ModuleScope"
                HAVING count(*) > 1
            ) duplicate;
            """);

        var parentOperationMappings = await ScalarAsync("""
            SELECT count(*)
            FROM axionpro."Module" module
            JOIN axionpro."ModuleOperationMapping" mapping
              ON mapping."ModuleId"=module."Id"
            WHERE module."ModuleCode" IN
                ('TENANT_DEPARTMENT','TENANT_DESIGNATION','TENANT_ROLE');
            """);

        var duplicatePlanMappings = await ScalarAsync("""
            SELECT count(*) FROM (
                SELECT mapping."SubscriptionPlanId",mapping."ModuleId"
                FROM axionpro."PlanModuleMapping" mapping
                JOIN axionpro."Module" module ON module."Id"=mapping."ModuleId"
                WHERE module."ModuleCode" IN
                    ('TENANT_DEPARTMENT','TENANT_DESIGNATION','TENANT_ROLE')
                GROUP BY mapping."SubscriptionPlanId",mapping."ModuleId"
                HAVING count(*) > 1
            ) duplicate;
            """);

        var requiredLeafOperationMappings = await ScalarAsync("""
            SELECT count(*)
            FROM axionpro."Module" module
            JOIN axionpro."ModuleOperationMapping" mapping
              ON mapping."ModuleId"=module."Id" AND mapping."IsActive"
            JOIN axionpro."Operation" operation
              ON operation."Id"=mapping."OperationId" AND operation."IsActive"
            WHERE module."ModuleCode" IN
                ('TENANT_DEPARTMENTS','TENANT_DESIGNATIONS','TENANT_ROLES_PERMISSIONS')
              AND lower(btrim(operation."OperationName")) IN
                ('add','update','delete','view','import','export');
            """);

        Assert.Multiple(() =>
        {
            Assert.That(validHierarchyRows, Is.EqualTo(3));
            Assert.That(duplicateModules, Is.Zero);
            Assert.That(parentOperationMappings, Is.Zero);
            Assert.That(duplicatePlanMappings, Is.Zero);
            Assert.That(requiredLeafOperationMappings, Is.EqualTo(18));
        });
    }

    private async Task<long> ScalarAsync(string sql)
    {
        await using var command = new NpgsqlCommand(sql, _connection);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }
}
