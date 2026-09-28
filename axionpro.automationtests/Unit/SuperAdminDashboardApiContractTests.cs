using System.Reflection;
using axionpro.api.Controllers.Dashboard;
using axionpro.application.DTOS.Dashboard;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
public sealed class SuperAdminDashboardApiContractTests
{
    [Test]
    public void Dashboard_controller_exposes_independent_super_admin_widget_routes()
    {
        var routes = typeof(DashboardController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
            .Select(attribute => $"{attribute.HttpMethods.Single()}:{attribute.Template}")
            .ToArray();

        Assert.That(routes, Is.EquivalentTo(new[]
        {
            "GET:SuperAdmin/Summary",
            "GET:SuperAdmin/EmployeeOverview",
            "GET:SuperAdmin/Birthdays",
            "GET:SuperAdmin/Onboarding",
            "GET:SuperAdmin/DepartmentHeadcount",
            "GET:SuperAdmin/CurrentlyOnLeave",
            "GET:SuperAdmin/Locations",
            "GET:SuperAdmin/Storage",
            "GET:SuperAdmin/HiringPipeline"
        }));
    }

    [Test]
    public void Birthday_contract_carries_employee_context_without_numeric_employee_identifier()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(DashboardEmployeeDTO).GetProperty(nameof(DashboardEmployeeDTO.EmployeeId))?.PropertyType, Is.EqualTo(typeof(string)));
            Assert.That(typeof(DashboardEmployeeDTO).GetProperty(nameof(DashboardEmployeeDTO.DepartmentName)), Is.Not.Null);
            Assert.That(typeof(DashboardEmployeeDTO).GetProperty(nameof(DashboardEmployeeDTO.DesignationName)), Is.Not.Null);
            Assert.That(typeof(DashboardEmployeeDTO).GetProperty(nameof(DashboardEmployeeDTO.RoleName)), Is.Not.Null);
        });
    }

    [Test]
    public void Dashboard_handlers_enforce_permission_pipeline_and_super_admin_role()
    {
        var source = File.ReadAllText(Path.Combine(FindRoot(), "axionpro.application", "Features", "DashboardCmd", "SuperAdminDashboardHandlers.cs"));
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("ValidateTenantPermissionAsync(permission, cancellationToken)"));
            Assert.That(source, Does.Contain("context.RoleTypeId != ConstantValues.RoleTypeAdmin"));
            Assert.That(source, Does.Contain("ForbiddenAccessException"));
        });
    }

    private static string FindRoot()
    {
        var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "AxionPro.sln"))) current = current.Parent;
        return current?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
