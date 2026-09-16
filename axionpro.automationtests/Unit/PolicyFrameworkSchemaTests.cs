using axionpro.application.Common.Enums;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("PolicyFramework")]
public sealed class PolicyFrameworkSchemaTests
{
    [Test]
    public void Schema_contains_versioned_policy_governance_tables_and_json_rules()
    {
        var sql = ReadRepositoryFile("database-scripts", "CreateGenericTenantPolicyFramework.sql");
        var tables = new[]
        {
            "PolicyCategory", "PolicyStatus", "PolicyRuleType", "PolicyDocumentType",
            "PolicyType", "Policy", "PolicyVersion", "PolicyRule", "PolicyApplicability",
            "PolicyAssignment", "PolicyException", "PolicyDocument", "PolicyApprovalStage",
            "PolicyApprovalHistory", "PolicyAcknowledgement", "PolicyChangeAudit"
        };

        Assert.Multiple(() =>
        {
            foreach (var table in tables)
            {
                Assert.That(sql, Does.Contain($"CREATE TABLE IF NOT EXISTS axionpro.\"{table}\""), table);
            }

            Assert.That(sql, Does.Contain("\"RuleConfiguration\" jsonb NOT NULL"));
            Assert.That(sql, Does.Contain("UX_PolicyVersion_Current"));
            Assert.That(sql, Does.Contain("CK_PolicyVersion_EffectiveDates"));
            Assert.That(sql, Does.Contain("FK_PolicyApplicability_TenantLocation"));
            Assert.That(sql, Does.Contain("FK_PolicyApplicability_EmployeeType"));
        });
    }

    [Test]
    public void Module_seed_has_one_parent_seven_leaf_modules_and_stable_page_names()
    {
        var sql = ReadRepositoryFile("database-scripts", "complete seed data", "SeedTenantPolicyModules.sql");
        var leafCodes = new[]
        {
            "TENANT_POLICY_TYPES", "TENANT_POLICY_DEFINITIONS", "TENANT_POLICY_ASSIGNMENTS",
            "TENANT_POLICY_EXCEPTIONS", "TENANT_POLICY_APPROVALS",
            "TENANT_POLICY_ACKNOWLEDGEMENTS", "TENANT_POLICY_AUDIT"
        };

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("('TENANT_POLICIES','Tenant-Policies','Policies',NULL,'tenant-policies-root',NULL,false"));
            foreach (var code in leafCodes)
            {
                Assert.That(sql, Does.Contain($"('{code}'"), code);
            }

            Assert.That(sql, Does.Contain("\"ModuleScope\"=1"));
            Assert.That(sql, Does.Contain("ADD COLUMN IF NOT EXISTS \"PageName\""));
            Assert.That(sql, Does.Contain("\"PageName\"=COALESCE(module.\"PageName\",seed.\"PageName\")"));
            Assert.That(sql, Does.Contain("INSERT INTO axionpro.\"PlanModuleMapping\""));
        });
    }

    [Test]
    public void Policy_operations_reuse_existing_names_and_add_only_missing_lifecycle_actions()
    {
        var sql = ReadRepositoryFile("database-scripts", "complete seed data", "SeedTenantPolicyModules.sql");

        Assert.Multiple(() =>
        {
            Assert.That((int)OperationType.Publish, Is.EqualTo(28));
            Assert.That((int)OperationType.Archive, Is.EqualTo(29));
            Assert.That((int)OperationType.Acknowledge, Is.EqualTo(30));
            Assert.That(sql, Does.Contain("WHERE NOT EXISTS (SELECT 1 FROM axionpro.\"Operation\" operation WHERE lower(btrim(operation.\"OperationName\"))=lower(seed.\"OperationName\"))"));
            Assert.That(sql, Does.Contain("('Publish',28"));
            Assert.That(sql, Does.Contain("('Archive',29"));
            Assert.That(sql, Does.Contain("('Acknowledge',30"));
            Assert.That(sql, Does.Contain("ORDER BY CASE WHEN candidate.\"OperationType\"=seed.\"OperationType\" THEN 0 ELSE 1 END"));
        });
    }

    [Test]
    public void Consolidated_production_seed_contains_policy_module_mirror()
    {
        var sql = ReadRepositoryFile(
            "database-scripts",
            "complete seed data",
            "AxionPro_New_Production_Module_Operation_Seed.sql");

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("SECTION: TENANT POLICY FRAMEWORK MODULES (canonical mirror)"));
            Assert.That(sql, Does.Contain("TENANT_POLICIES"));
            Assert.That(sql, Does.Contain("TENANT_POLICY_ACKNOWLEDGEMENTS"));
            Assert.That(sql, Does.Not.Contain("\\i"));
        });
    }

    private static string ReadRepositoryFile(params string[] parts)
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        var root = directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate AxionPro.sln.");
        return File.ReadAllText(Path.Combine(new[] { root }.Concat(parts).ToArray()));
    }
}
