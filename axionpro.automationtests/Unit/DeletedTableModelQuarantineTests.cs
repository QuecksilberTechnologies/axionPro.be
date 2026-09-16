using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("DeletedTableModelQuarantine")]
public sealed class DeletedTableModelQuarantineTests
{
    private static readonly string[] RetiredEntityNames =
    {
        "AccommodationAllowancePolicyByDesignation", "BasicMenu", "Candidate",
        "CandidateCategorySkill", "EmployeeDeviceAccessWindow", "InsurancePolicy",
        "InsurancePolicyDocument", "LeaveRule", "LeaveSandwichRule",
        "LeaveSandwichRuleMapping", "MealAllowancePolicyByDesignation",
        "PolicyLeaveTypeMapping", "PolicyTypeDocument",
        "PolicyTypeInsuranceMapping", "ServiceProvider",
        "TenantEmployeeSectionDefault", "TravelAllowancePolicyByDesignation",
        "TravelMode", "UnStructuredPolicyTypeMappingWithEmployeeType",
        "WorkflowStage", "WorkstationType"
    };

    [Test]
    public void Production_deleted_entities_are_explicitly_excluded_from_ef_model()
    {
        var root = FindRepositoryRoot();
        var context = File.ReadAllText(Path.Combine(
            root,
            "axionpro.persistance",
            "Data",
            "Context",
            "WorkforceDbContext.cs"));

        Assert.Multiple(() =>
        {
            foreach (var entityName in RetiredEntityNames)
            {
                Assert.That(
                    context,
                    Does.Contain($"modelBuilder.Ignore<{entityName}>();"),
                    $"{entityName} must remain excluded while its production table is absent.");
            }
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
