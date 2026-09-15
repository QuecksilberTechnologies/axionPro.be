using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.domain.Entity;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("OrganizationHolidayCalendarRefactor")]
public sealed class OrganizationHolidayCalendarRefactorTests
{
    [Test]
    public void Entity_uses_location_and_date_without_redundant_geography_fields()
    {
        var properties = typeof(OrganizationHolidayCalendar)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(properties, Does.Contain("TenantLocationId"));
            Assert.That(typeof(OrganizationHolidayCalendar).GetProperty("HolidayDate")!.PropertyType, Is.EqualTo(typeof(DateOnly)));
            Assert.That(properties, Does.Not.Contain("CountryCode"));
            Assert.That(properties, Does.Not.Contain("StateCode"));
            Assert.That(properties, Does.Not.Contain("HolidayYear"));
            Assert.That(properties, Does.Not.Contain("Remark"));
        });
    }

    [Test]
    public void Response_contract_exposes_location_and_nonredundant_holiday_fields()
    {
        var properties = typeof(OrganizationHolidayCalendarDTO)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(properties, Does.Contain("TenantLocationId"));
            Assert.That(properties, Does.Contain("IsOptional"));
            Assert.That(properties, Does.Contain("Description"));
            Assert.That(properties, Does.Not.Contain("StateCode"));
            Assert.That(properties, Does.Not.Contain("HolidayYear"));
        });
    }

    [Test]
    public void Migration_is_guarded_and_creates_fk_and_location_date_index()
    {
        var root = FindRepositoryRoot();
        var sql = File.ReadAllText(Path.Combine(
            root,
            "database-scripts",
            "RefactorOrganizationHolidayCalendar.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.Contain("require an explicit TenantLocation mapping"));
            Assert.That(sql, Does.Contain("FK_OrganizationHolidayCalendar_TenantLocation"));
            Assert.That(sql, Does.Contain("ON DELETE RESTRICT"));
            Assert.That(sql, Does.Contain("IX_OrganizationHolidayCalendar_Location_Date"));
            Assert.That(sql, Does.Contain("ALTER COLUMN \"TenantLocationId\" SET NOT NULL"));
            Assert.That(sql, Does.Contain("DROP COLUMN IF EXISTS \"CountryCode\""));
            Assert.That(sql, Does.Contain("ALTER COLUMN \"HolidayDate\" TYPE date"));
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
