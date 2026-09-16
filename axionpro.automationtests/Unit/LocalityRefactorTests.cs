using axionpro.api.Controllers.Location;
using axionpro.application.DTOS.Location;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("LocalityRefactor")]
public sealed class LocalityRefactorTests
{
    [Test]
    public void Locality_model_requires_district_and_type_relationships()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(Locality).GetProperty(nameof(Locality.DistrictId)), Is.Not.Null);
            Assert.That(typeof(Locality).GetProperty(nameof(Locality.LocalityTypeId)), Is.Not.Null);
            Assert.That(typeof(Locality).GetProperty(nameof(Locality.LocalityName)), Is.Not.Null);
            Assert.That(typeof(LocalityType).GetProperty(nameof(LocalityType.TypeName)), Is.Not.Null);
        });
    }

    [Test]
    public void Locality_option_contract_requires_district_and_returns_type_context()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(GetLocalityOptionRequestDTO).GetProperty(nameof(GetLocalityOptionRequestDTO.DistrictId)), Is.Not.Null);
            Assert.That(typeof(GetLocalityOptionResponseDTO).GetProperty(nameof(GetLocalityOptionResponseDTO.LocalityTypeId)), Is.Not.Null);
            Assert.That(typeof(GetLocalityOptionResponseDTO).GetProperty(nameof(GetLocalityOptionResponseDTO.LocalityTypeName)), Is.Not.Null);
        });
    }

    [Test]
    public void Tenant_location_contract_uses_district_and_locality_names()
    {
        var createProperties = typeof(CreateTenantLocationRequestDTO).GetProperties().Select(x => x.Name).ToArray();
        var responseProperties = typeof(TenantLocationResponseDTO).GetProperties().Select(x => x.Name).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(createProperties, Does.Contain("DistrictId"));
            Assert.That(createProperties, Does.Contain("LocalityId"));
            Assert.That(createProperties, Does.Not.Contain("CityId"));
            Assert.That(responseProperties, Does.Contain("DistrictName"));
            Assert.That(responseProperties, Does.Contain("LocalityName"));
            Assert.That(responseProperties, Does.Not.Contain("CityName"));
        });
    }

    [TestCase(nameof(LocationController.GetLocality), "Locality/option")]
    [TestCase(nameof(LocationController.GetLocalityType), "LocalityType/option")]
    public void Location_lookup_routes_are_exposed(string actionName, string route)
    {
        var method = typeof(LocationController).GetMethod(actionName);
        var attribute = method?.GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.That(attribute?.Template, Is.EqualTo(route));
    }

    [Test]
    public void Migration_is_data_preserving_guarded_and_seeds_all_locality_types()
    {
        var sql = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "database-scripts",
            "RenameCityToLocality.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("ALTER TABLE axionpro.\"City\" RENAME TO \"Locality\""));
            Assert.That(sql, Does.Contain("district.\"Id\" = locality.\"Id\""));
            Assert.That(sql, Does.Contain("RAISE EXCEPTION '% Locality row(s) cannot be mapped"));
            Assert.That(sql, Does.Contain("(1, 'City', TRUE)"));
            Assert.That(sql, Does.Contain("(2, 'Town', TRUE)"));
            Assert.That(sql, Does.Contain("(3, 'Village', TRUE)"));
            Assert.That(sql, Does.Contain("CREATE VIEW axionpro.\"City\""));
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
