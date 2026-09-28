// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Guards runtime prerequisites required after the canonical production reset.
// ================================================================

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("CanonicalProductionSeed")]
public sealed class CanonicalProductionSeedTests
{
    [Test]
    public void Shared_master_seed_contains_the_global_permanent_onboarding_template()
    {
        var repositoryRoot = FindRepositoryRoot();
        var seed = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "database-scripts",
            "production-seed",
            "02-parent-master",
            "001-shared-master-data.sql"));
        var verification = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "database-scripts",
            "production-seed",
            "99-verification",
            "001-verify-canonical-seed.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(seed, Does.Contain("INSERT INTO axionpro.\"EmployeeType\""));
            Assert.That(seed, Does.Contain("(1, NULL, 'Permanent'"));
            Assert.That(verification, Does.Contain("Global Permanent employee onboarding template is missing."));
        });
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, "database-scripts")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Repository root was not found from the test directory.");
    }
}
