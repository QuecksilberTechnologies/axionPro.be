// ================================================================
// Purpose : Regression coverage for the device configuration, employee
//           enrollment, work setup, card inventory, and DDL API surface.
//           These tests intentionally use controller metadata and constants,
//           so they do not require a device, a broker, or a live database.
// ================================================================

using System.Reflection;
using axionpro.api.Controllers.HostDevice;
using axionpro.api.Controllers.TenantConfiguration;
using axionpro.application.Constants;
using axionpro.application.Features.EmployeeCmd;
using axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>
/// Locks down every recently added Device and Employee endpoint. A route,
/// verb, authorization attribute, DDL section, or server-owned permission
/// module cannot drift without causing an explicit automated-test failure.
/// </summary>
[TestFixture]
[Category("RecentDeviceEmployee")]
public sealed class RecentDeviceEmployeeApiContractTests
{
    public sealed record EndpointContract(Type ControllerType, string HttpMethod, string Template);

    public sealed record DdlContract(string ActionName, string Section, params string[] FieldKeys);

    public static IEnumerable<TestCaseData> EndpointContracts() =>
        BuildEndpointContracts().Select(contract => new TestCaseData(contract)
            .SetName($"{contract.ControllerType.Name}_{contract.HttpMethod}_{contract.Template.Replace('/', '_')}"));

    public static IEnumerable<TestCaseData> DdlContracts() =>
        new[]
        {
            new DdlContract(nameof(DeviceDdlOptionsController.GetTime), DeviceDdl.Time, "timeFormat", "dateFormat", "networkTimeEnabled", "timeZone"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetBell), DeviceDdl.Bell, "ringStyle", "bellOutput"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetDeviceSetup), DeviceDdl.DeviceSetup, "language", "screenWakeUpMethod", "resultDisplayStyle", "faceRecognitionDistance"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetAdvanced), DeviceDdl.Advanced, "verificationMode", "qrCodeMode", "fillLightMode"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetLock), DeviceDdl.Lock, "doorSensorMode", "antiPassbackMode", "wiegandOutput", "wiegandFormat", "cardDisplayFormat"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetSerial), DeviceDdl.Serial, "baudRate", "serialFunction"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetEthernet), DeviceDdl.Ethernet, "dhcpEnabled"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetWifi), DeviceDdl.Wifi, "dhcpEnabled"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetAppNotification), DeviceDdl.AppNotification, "appNotificationEnabled", "notificationType"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetEmployeeDeviceCredentials), DeviceDdl.EmployeeDeviceCredentials, "credentialType"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetEmployeeDeviceAccessWindows), DeviceDdl.EmployeeDeviceAccessWindows, "dayOfWeek"),
            new DdlContract(nameof(DeviceDdlOptionsController.GetTenantCardInventory), DeviceDdl.TenantCardInventory, "taxTreatment", "cardStatus")
        }.Select(contract => new TestCaseData(contract).SetName($"DDL_{contract.Section}"));

    public static IEnumerable<TestCaseData> EmployeePermissionModules() =>
        new[]
        {
            new { Request = typeof(GetEmployeeDeviceEnrollmentsQuery), ExpectedModule = "EMP_DEVICES" },
            new { Request = typeof(GetEmployeeLocationAssignmentsQuery), ExpectedModule = "EMP_WORK_LOCATIONS" },
            new { Request = typeof(GetEmployeeWorkArrangementsQuery), ExpectedModule = "EMP_WORK_ARRANGEMENT" },
            new { Request = typeof(GetEmployeeWorkPatternsQuery), ExpectedModule = "EMP_WORK_PATTERN" },
            new { Request = typeof(GetEmployeeWorkModeOverridesQuery), ExpectedModule = "EMP_OVERRIDES" }
        }.Select(test => new TestCaseData(test.Request, test.ExpectedModule)
            .SetName($"{test.Request.Name}_requires_{test.ExpectedModule}"));

    [TestCaseSource(nameof(EndpointContracts))]
    public void Recent_endpoint_keeps_its_expected_route_verb_and_authorization(EndpointContract contract)
    {
        var route = contract.ControllerType.GetCustomAttribute<RouteAttribute>(inherit: true);
        var endpointExists = contract.ControllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>(inherit: true))
            .Any(attribute =>
                string.Equals(attribute.Template, contract.Template, StringComparison.Ordinal)
                && attribute.HttpMethods.Contains(contract.HttpMethod, StringComparer.OrdinalIgnoreCase));

        Assert.Multiple(() =>
        {
            Assert.That(route?.Template, Is.Not.Null.And.Not.Empty,
                $"{contract.ControllerType.Name} must declare an API route.");
            Assert.That(contract.ControllerType.IsDefined(typeof(AuthorizeAttribute), inherit: true), Is.True,
                $"{contract.ControllerType.Name} must remain authenticated.");
            Assert.That(endpointExists, Is.True,
                $"Missing {contract.HttpMethod} {contract.Template} on {contract.ControllerType.Name}.");
        });
    }

    [TestCaseSource(nameof(DdlContracts))]
    public void Every_ddl_endpoint_returns_only_its_owned_complete_constant_section(DdlContract contract)
    {
        var controller = new DeviceDdlOptionsController();
        var action = typeof(DeviceDdlOptionsController).GetMethod(contract.ActionName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new AssertionException($"DDL action {contract.ActionName} was not found.");
        var result = action.Invoke(controller, null) as OkObjectResult;
        var response = result?.Value as axionpro.application.Wrappers.ApiResponse<IReadOnlyList<DeviceDdlField>>;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSucceeded, Is.True);
            Assert.That(response.Data.Select(field => field.Key), Is.EquivalentTo(contract.FieldKeys));
            Assert.That(response.Data.SelectMany(field => field.Options), Is.Not.Empty);
            Assert.That(response.Data.SelectMany(field => field.Options).All(option =>
                !string.IsNullOrWhiteSpace(option.Value) && !string.IsNullOrWhiteSpace(option.Label)), Is.True);
            Assert.That(DeviceDdl.GetSection(contract.Section).Select(field => field.Key), Is.EquivalentTo(contract.FieldKeys));
        });
    }

    [Test]
    public void Device_ddl_catalog_contains_every_section_exposed_by_the_controller()
    {
        var sections = DeviceDdl.GetSectionNames();

        Assert.That(sections, Is.EquivalentTo(new[]
        {
            DeviceDdl.Time,
            DeviceDdl.Bell,
            DeviceDdl.DeviceSetup,
            DeviceDdl.Advanced,
            DeviceDdl.Lock,
            DeviceDdl.Serial,
            DeviceDdl.Ethernet,
            DeviceDdl.Wifi,
            DeviceDdl.AppNotification,
            DeviceDdl.EmployeeDeviceCredentials,
            DeviceDdl.EmployeeDeviceAccessWindows,
            DeviceDdl.TenantCardInventory
        }));
    }

    [TestCaseSource(nameof(EmployeePermissionModules))]
    public void Employee_request_family_is_bound_to_its_server_owned_permission_module(Type requestType, string expectedModuleCode)
    {
        var behaviorType = typeof(EmployeeTenantPermissionBehavior<,>).MakeGenericType(requestType, typeof(object));
        var resolver = behaviorType.GetMethod("ResolveExpectedModuleCode", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new AssertionException("Employee permission module resolver was not found.");

        var actualModuleCode = resolver.Invoke(null, null) as string;

        Assert.That(actualModuleCode, Is.EqualTo(expectedModuleCode));
    }

    [Test]
    public void Employee_device_contract_keeps_credentials_and_physical_enroll_id_out_of_the_ui_response()
    {
        var responseType = typeof(axionpro.application.DTOS.TenantConfiguration.EmployeeDeviceEnrollmentResponseDTO);

        Assert.Multiple(() =>
        {
            Assert.That(responseType.GetProperty("Pin"), Is.Null);
            Assert.That(responseType.GetProperty("CardNumber"), Is.Null);
            Assert.That(responseType.GetProperty("FaceImage"), Is.Null);
            Assert.That(responseType.GetProperty("DeviceEnrollId"), Is.Null);
            Assert.That(responseType.GetProperty("FaceCommandStatus"), Is.Not.Null);
            Assert.That(responseType.GetProperty("CardCommandStatus"), Is.Not.Null);
            Assert.That(responseType.GetProperty("PinCommandStatus"), Is.Not.Null);
            Assert.That(responseType.GetProperty("UserActivationCommandStatus"), Is.Not.Null);
        });
    }

    private static IEnumerable<EndpointContract> BuildEndpointContracts()
    {
        // Tenant Device lifecycle APIs.
        foreach (var contract in Crud(typeof(TenantDeviceController), "{id}"))
            yield return contract;
        yield return Endpoint(typeof(TenantDeviceController), "POST", "update-location");

        // Host provisioning + Tenant device connectivity and runtime APIs.
        foreach (var template in new[]
        {
            "issue-bootstrap-url", "apply-runtime-configuration", "reboot",
            "settings/time", "settings/time/sync", "settings/bell", "settings/device-setup",
            "settings/advanced", "settings/lock", "settings/serial", "settings/ethernet",
            "settings/wifi", "settings/app-notification", "settings/web-access", "settings/screen-menu-pin",
            "replace-https-gateway-url", "dispatch-mqtts-now", "create", "update", "rotate-https-ingress-token"
        })
            yield return Endpoint(typeof(TenantDeviceConfigurationController), "POST", template);
        yield return Endpoint(typeof(TenantDeviceConfigurationController), "GET", "gateway-address/{tenantDeviceId}");
        yield return Endpoint(typeof(TenantDeviceConfigurationController), "GET", "get-by-id/{id}");
        yield return Endpoint(typeof(TenantDeviceConfigurationController), "GET", "get-all");
        yield return Endpoint(typeof(TenantDeviceConfigurationController), "DELETE", "delete/{id}");

        // Dropdown API: one endpoint per UI section; no Angular hard-coded values.
        foreach (var template in new[]
        {
            "time", "bell", "device-setup", "advanced", "lock", "serial", "ethernet", "wifi",
            "app-notification", "employee-device-credentials", "employee-device-access-windows", "tenant-card-inventory"
        })
            yield return Endpoint(typeof(DeviceDdlOptionsController), "GET", template);

        // Tenant location, attendance, employee work, and Host card lifecycle APIs.
        foreach (var contract in Crud(typeof(TenantLocationController), "{id:long}"))
            yield return contract;
        foreach (var contract in Crud(typeof(AttendancePolicyController), "{id:int}"))
            yield return contract;
        foreach (var contract in Crud(typeof(EmployeeLocationAssignmentController), "{id:long}"))
            yield return contract;
        foreach (var contract in Crud(typeof(EmployeeWorkArrangementController), "{id:long}"))
            yield return contract;
        foreach (var contract in Crud(typeof(EmployeeWorkPatternController), "{id:long}"))
            yield return contract;
        foreach (var contract in Crud(typeof(EmployeeWorkModeOverrideController), "{id:long}"))
            yield return contract;
        foreach (var contract in Crud(typeof(TenantCardMasterController), "{id}"))
            yield return contract;

        // Employee device enrollment owns credentials in addition to standard lifecycle calls.
        foreach (var contract in Crud(typeof(EmployeeDeviceEnrollmentController), "{id}"))
            yield return contract;
        foreach (var template in new[] { "face/upsert", "pin/upsert", "card/bind", "credential/remove" })
            yield return Endpoint(typeof(EmployeeDeviceEnrollmentController), "POST", template);
    }

    private static IEnumerable<EndpointContract> Crud(Type controllerType, string idTemplate)
    {
        yield return Endpoint(controllerType, "POST", "create");
        yield return Endpoint(controllerType, "GET", $"get-by-id/{idTemplate}");
        yield return Endpoint(controllerType, "GET", "get-all");
        yield return Endpoint(controllerType, "POST", "update");
        yield return Endpoint(controllerType, "POST", "update-status");
        yield return Endpoint(controllerType, "DELETE", $"delete/{idTemplate}");
    }

    private static EndpointContract Endpoint(Type controllerType, string httpMethod, string template) =>
        new(controllerType, httpMethod, template);
}
