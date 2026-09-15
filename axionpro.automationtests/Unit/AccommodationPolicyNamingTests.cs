using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>Guards the corrected accommodation policy persistence name.</summary>
[TestFixture]
[Category("AccommodationPolicyNaming")]
public sealed class AccommodationPolicyNamingTests
{
    [Test]
    public void Runtime_source_does_not_reference_legacy_misspelling()
    {
        var repositoryRoot = FindRepositoryRoot();
        var runtimeRoots = new[]
        {
            "axionpro.api",
            "axionpro.application",
            "axionpro.domain",
            "axionpro.infrastructure",
            "axionpro.persistance"
        };

        var references = runtimeRoots
            .SelectMany(root => Directory.EnumerateFiles(
                Path.Combine(repositoryRoot, root),
                "*.cs",
                SearchOption.AllDirectories))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(path => File.ReadAllText(path).Contains("Accoumndation", StringComparison.Ordinal))
            .ToArray();

        Assert.That(references, Is.Empty);
    }

    [Test]
    public void Rename_script_preserves_data_by_using_table_rename()
    {
        var repositoryRoot = FindRepositoryRoot();
        var sql = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "database-scripts",
            "RenameAccommodationAllowancePolicyTable.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("RENAME TO \"AccommodationAllowancePolicyByDesignation\""));
            Assert.That(sql, Does.Contain("CREATE VIEW axionpro.\"AccoumndationAllowancePolicyByDesignation\""));
            Assert.That(sql, Does.Not.Contain("DROP TABLE"));
            Assert.That(sql, Does.Not.Contain("DELETE FROM"));
        });
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null &&
               !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate AxionPro.sln.");
    }
}
