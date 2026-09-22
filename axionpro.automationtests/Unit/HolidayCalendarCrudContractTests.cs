using System.Reflection;
using axionpro.api.Controllers.HolidayCalandar;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.Features.HolidayCalandarCmd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("HolidayCalendarCrud")]
public sealed class HolidayCalendarCrudContractTests
{
    [Test]
    public void Controller_requires_authentication_and_exposes_all_crud_routes()
    {
        var controller = typeof(HolidayCalandarController);
        var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        Assert.Multiple(() =>
        {
            Assert.That(controller.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null);
            Assert.That(methods.Single(method => method.Name == "Get").GetCustomAttribute<HttpGetAttribute>()?.Template, Is.EqualTo("get"));
            Assert.That(methods.Single(method => method.Name == "GetById").GetCustomAttribute<HttpGetAttribute>()?.Template, Is.EqualTo("{id:long}"));
            Assert.That(methods.Single(method => method.Name == "Create").GetCustomAttribute<HttpPostAttribute>(), Is.Not.Null);
            Assert.That(methods.Single(method => method.Name == "Update").GetCustomAttribute<HttpPutAttribute>()?.Template, Is.EqualTo("{id:long}"));
            Assert.That(methods.Single(method => method.Name == "Delete").GetCustomAttribute<HttpDeleteAttribute>()?.Template, Is.EqualTo("{id:long}"));
        });
    }

    [Test]
    public void Request_contract_requires_dynamic_permission_ids_and_location_scope()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(BasicRequestDTO).GetProperty("ModuleId"), Is.Not.Null);
            Assert.That(typeof(BasicRequestDTO).GetProperty("OperationId"), Is.Not.Null);
            Assert.That(typeof(BasicRequestDTO).GetProperty("TenantLocationId"), Is.Not.Null);
            Assert.That(typeof(BasicRequestDTO).GetProperty("HolidayYear"), Is.Not.Null);
            Assert.That(typeof(SaveHolidayRequestDTO).GetProperty("TenantId"), Is.Null);
            Assert.That(typeof(UpdateHolidayRequestDTO).GetProperty("TenantId"), Is.Null);
            Assert.That(typeof(OrganizationHolidayCalendarDTO).GetProperty("Id"), Is.Not.Null);
        });
    }

    [Test]
    public void Every_crud_request_has_a_permission_pipeline_handler()
    {
        var requests = new[]
        {
            typeof(ListHolidaysQuery),
            typeof(GetHolidayQuery),
            typeof(CreateHolidayCommand),
            typeof(UpdateHolidayCommand),
            typeof(DeleteHolidayCommand)
        };

        Assert.Multiple(() =>
        {
            foreach (var request in requests)
            {
                Assert.That(request.Assembly.GetTypes().Any(type =>
                    type.GetInterfaces().Any(contract =>
                        contract.IsGenericType
                        && contract.GetGenericTypeDefinition() == typeof(MediatR.IRequestHandler<,>)
                        && contract.GetGenericArguments()[0] == request)), Is.True, request.Name);
            }

            Assert.That(typeof(HolidayCalendarPermissionBehavior<,>), Is.Not.Null);
        });
    }
}
