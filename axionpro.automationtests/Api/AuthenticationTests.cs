using AxionPro.AutomationTests.Infrastructure;
using NUnit.Framework;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace AxionPro.AutomationTests.Api;

[TestFixture]
[Category("API")]
public sealed class AuthenticationTests : ApiTestBase
{
    [Test]
    [Category("HostLive")]
    public async Task Tenant_device_live_api_supports_the_complete_assignment_lifecycle()
    {
        using var client = await CreateAuthenticatedHostClientAsync();
        var modules = await GetHostMenuModulesAsync(client);
        var tenantDeviceModule = Module(modules, "TENANT_DEVICES");
        var tenantListModule = Module(modules, "HOST_TENANT_LIST");
        var tenantLocationModule = Module(modules, "HOST_TENANT_LOCATION_LIST");
        var deviceCatalogueModule = Module(modules, "HOST_DEVICE_SETUP");

        var tenants = await GetSuccessfulPayloadAsync(
            client,
            $"/api/Tenant/get-all-tenants?PageNumber=1&PageSize=10&ModuleId={Property(tenantListModule, "id")}&OperationId={Operation(tenantListModule, "View")}");
        var tenantId = Property(Property(tenants.RootElement, "data").EnumerateArray().First(), "id").GetString()!;

        var locations = await GetSuccessfulPayloadAsync(
            client,
            $"/api/TenantLocation/get-all?IsActive=true&PageNumber=1&PageSize=10&TenantId={tenantId}&ModuleId={Property(tenantLocationModule, "id")}&OperationId={Operation(tenantLocationModule, "View")}");
        var tenantLocationId = Property(Property(locations.RootElement, "data").EnumerateArray().First(), "id").GetInt64();

        var catalogue = await GetSuccessfulPayloadAsync(
            client,
            $"/api/DeviceMaster/get-all?IsActive=true&PageNumber=1&PageSize=10&ModuleId={Property(deviceCatalogueModule, "id")}&OperationId={Operation(deviceCatalogueModule, "View")}");
        var deviceMasterId = Property(Property(catalogue.RootElement, "data").EnumerateArray().First(), "id").GetInt64();

        var code = $"API-TD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        string? tenantDeviceId = null;

        try
        {
            var created = await PostSuccessfulPayloadAsync(client, "/api/TenantDevice/create", new
            {
                tenantId,
                tenantLocationId,
                deviceMasterId,
                deviceCode = code,
                deviceName = "Automated TenantDevice lifecycle",
                installedDateTime = DateTime.UtcNow,
                installationRemark = "Disposable HostLive API test",
                isAttendanceDevice = true,
                description = "Created by automated API verification",
                remark = "Removed by test cleanup",
                isActive = true,
                moduleId = Property(tenantDeviceModule, "id").GetInt32(),
                operationId = Operation(tenantDeviceModule, "Assign")
            });
            tenantDeviceId = Property(Property(created.RootElement, "data"), "id").GetString();

            using var listed = await GetSuccessfulPayloadAsync(
                client,
                $"/api/TenantDevice/get-all?Search={Uri.EscapeDataString(code)}&PageNumber=1&PageSize=10&ModuleId={Property(tenantDeviceModule, "id")}&OperationId={Operation(tenantDeviceModule, "View")}");
            Assert.That(Property(listed.RootElement, "totalRecords").GetInt32(), Is.EqualTo(1));

            using var read = await GetSuccessfulPayloadAsync(
                client,
                $"/api/TenantDevice/get-by-id/{tenantDeviceId}?TenantId={tenantId}&ModuleId={Property(tenantDeviceModule, "id")}&OperationId={Operation(tenantDeviceModule, "View")}");
            Assert.That(Property(Property(read.RootElement, "data"), "deviceCode").GetString(), Is.EqualTo(code));

            await PostSuccessfulPayloadAsync(client, "/api/TenantDevice/update", new
            {
                id = tenantDeviceId,
                tenantId,
                tenantLocationId,
                deviceMasterId,
                deviceCode = code,
                deviceName = "Automated TenantDevice lifecycle updated",
                isAttendanceDevice = true,
                isActive = true,
                moduleId = Property(tenantDeviceModule, "id").GetInt32(),
                operationId = Operation(tenantDeviceModule, "Update")
            });

            await PostSuccessfulPayloadAsync(client, "/api/TenantDevice/update-status", new
            {
                id = tenantDeviceId,
                tenantId,
                isActive = false,
                moduleId = Property(tenantDeviceModule, "id").GetInt32(),
                operationId = Operation(tenantDeviceModule, "Inactive")
            });

            await PostSuccessfulPayloadAsync(client, "/api/TenantDevice/update-status", new
            {
                id = tenantDeviceId,
                tenantId,
                isActive = true,
                moduleId = Property(tenantDeviceModule, "id").GetInt32(),
                operationId = Operation(tenantDeviceModule, "Active")
            });
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(tenantDeviceId))
            {
                using var removed = await client.DeleteAsync(
                    $"/api/TenantDevice/delete/{tenantDeviceId}?TenantId={tenantId}&ModuleId={Property(tenantDeviceModule, "id")}&OperationId={Operation(tenantDeviceModule, "Remove")}");
                Assert.That(removed.IsSuccessStatusCode, Is.True, await removed.Content.ReadAsStringAsync());
            }
        }
    }

    [Test]
    [Category("HostLive")]
    public async Task Refresh_apis_rotate_a_fresh_host_token_and_return_the_expected_payloads()
    {
        var loginId = RequiredEnvironment("AXIONPRO_HOST_LOGIN_ID");
        var password = RequiredEnvironment("AXIONPRO_HOST_LOGIN_PASSWORD");
        using var client = new HttpClient
        {
            BaseAddress = new Uri(TestSettings.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(60)
        };
        using var loginResponse = await client.PostAsJsonAsync("/api/NewLogin/login", LoginBody(loginId, password));
        Assert.That(loginResponse.IsSuccessStatusCode, Is.True, await loginResponse.Content.ReadAsStringAsync());
        using var login = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var originalRefreshToken = Property(Property(login.RootElement, "data"), "refreshToken").GetString()!;

        using var legacyResponse = await client.PostAsJsonAsync("/api/Auth/refresh-token", new
        {
            refreshToken = originalRefreshToken,
            ipAddress = "127.0.0.1"
        });
        Assert.That((int)legacyResponse.StatusCode, Is.EqualTo(200), await legacyResponse.Content.ReadAsStringAsync());
        using var legacy = JsonDocument.Parse(await legacyResponse.Content.ReadAsStringAsync());
        Assert.That(Property(legacy.RootElement, "isSucceeded").GetBoolean(), Is.True);
        var legacyReplacement = Property(Property(legacy.RootElement, "data"), "refreshToken").GetString()!;

        using var reusedLegacy = await client.PostAsJsonAsync("/api/Auth/refresh-token", new
        {
            refreshToken = originalRefreshToken,
            ipAddress = "127.0.0.1"
        });
        Assert.That((int)reusedLegacy.StatusCode, Is.EqualTo(401));

        using var v2Response = await client.PostAsJsonAsync("/api/Auth/refresh-token-v2", new
        {
            refreshToken = legacyReplacement,
            ipAddress = "127.0.0.1"
        });
        Assert.That((int)v2Response.StatusCode, Is.EqualTo(200), await v2Response.Content.ReadAsStringAsync());
        using var v2 = JsonDocument.Parse(await v2Response.Content.ReadAsStringAsync());
        Assert.That(Property(v2.RootElement, "isSucceeded").GetBoolean(), Is.True);
        var v2Data = Property(v2.RootElement, "data");
        Assert.That(
            v2Data.EnumerateObject().Select(property => property.Name).OrderBy(name => name),
            Is.EqualTo(new[] { "refreshToken", "refreshTokenExpiresAtUtc", "token", "tokenExpiry" }.OrderBy(name => name)));

        using var reusedV2 = await client.PostAsJsonAsync("/api/Auth/refresh-token-v2", new
        {
            refreshToken = legacyReplacement,
            ipAddress = "127.0.0.1"
        });
        Assert.That((int)reusedV2.StatusCode, Is.EqualTo(401));
    }

    [TestCase("TenantDevice", "AXIONPRO_HOST_DEVICE_MODULE_ID")]
    [TestCase("TenantDeviceConfiguration", "AXIONPRO_HOST_CONFIGURATION_MODULE_ID")]
    [Category("HostLive")]
    public async Task Authenticated_host_device_list_returns_a_successful_paged_response(string controller, string moduleVariable)
    {
        var loginId = Environment.GetEnvironmentVariable("AXIONPRO_HOST_LOGIN_ID");
        var password = Environment.GetEnvironmentVariable("AXIONPRO_HOST_LOGIN_PASSWORD");
        var moduleId = Environment.GetEnvironmentVariable(moduleVariable);
        var operationId = Environment.GetEnvironmentVariable("AXIONPRO_HOST_VIEW_OPERATION_ID");
        if (new[] { loginId, password, moduleId, operationId }.Any(string.IsNullOrWhiteSpace))
            Assert.Ignore("HostLive requires Host credentials and module/View operation IDs from my-menu in environment variables.");
        using var client = new HttpClient { BaseAddress = new Uri(TestSettings.ApiBaseUrl), Timeout = TimeSpan.FromSeconds(60) };
        using var loginResponse = await client.PostAsJsonAsync("/api/NewLogin/login", new
        {
            LoginId = loginId, Password = password, IpAddressLocal = "127.0.0.1", IpAddressPublic = "",
            MacAddress = "", LoginDevice = 1, Latitude = 0, Longitude = 0
        });
        Assert.That(loginResponse.IsSuccessStatusCode, Is.True, $"Host login returned {(int)loginResponse.StatusCode}");
        using var login = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var data = Property(login.RootElement, "data");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Property(data, "accessToken").GetString());
        using var response = await client.GetAsync($"/api/{controller}/get-all?PageNumber=1&PageSize=10&ModuleId={moduleId}&OperationId={operationId}");
        Assert.That((int)response.StatusCode, Is.EqualTo(200), $"Authenticated {controller}/get-all returned {(int)response.StatusCode}");
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.That(Property(payload.RootElement, "isSucceeded").GetBoolean(), Is.True);
        Assert.That(Property(payload.RootElement, "data").ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(Property(payload.RootElement, "pageNumber").GetInt32(), Is.EqualTo(1));
        Assert.That(Property(payload.RootElement, "totalRecords").GetInt32(), Is.GreaterThanOrEqualTo(0));
    }

    private static JsonElement Property(JsonElement element, string name) =>
        element.EnumerateObject().First(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;

    private static string RequiredEnvironment(string name) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
            ? value
            : throw new IgnoreException($"HostLive requires {name}.");

    private static object LoginBody(string loginId, string password) => new
    {
        LoginId = loginId,
        Password = password,
        IpAddressLocal = "127.0.0.1",
        IpAddressPublic = string.Empty,
        MacAddress = string.Empty,
        LoginDevice = 1,
        Latitude = 0,
        Longitude = 0
    };

    private static async Task<HttpClient> CreateAuthenticatedHostClientAsync()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(TestSettings.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(60)
        };
        using var response = await client.PostAsJsonAsync(
            "/api/NewLogin/login",
            LoginBody(RequiredEnvironment("AXIONPRO_HOST_LOGIN_ID"), RequiredEnvironment("AXIONPRO_HOST_LOGIN_PASSWORD")));
        Assert.That(response.IsSuccessStatusCode, Is.True, await response.Content.ReadAsStringAsync());
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Property(Property(payload.RootElement, "data"), "accessToken").GetString());
        return client;
    }

    private static async Task<List<JsonElement>> GetHostMenuModulesAsync(HttpClient client)
    {
        using var payload = await GetSuccessfulPayloadAsync(client, "/api/Navigation/my-menu");
        return FlattenModules(Property(Property(payload.RootElement, "data"), "items"))
            .Select(module => module.Clone())
            .ToList();
    }

    private static JsonElement Module(IEnumerable<JsonElement> modules, string code) =>
        modules.First(module => string.Equals(Property(module, "moduleCode").GetString(), code, StringComparison.OrdinalIgnoreCase));

    private static int Operation(JsonElement module, string name) =>
        Property(module, "operations").EnumerateArray()
            .First(operation => string.Equals(Property(operation, "name").GetString(), name, StringComparison.OrdinalIgnoreCase))
            .GetProperty("id")
            .GetInt32();

    private static async Task<JsonDocument> GetSuccessfulPayloadAsync(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(response.IsSuccessStatusCode, Is.True, content);
        var payload = JsonDocument.Parse(content);
        Assert.That(Property(payload.RootElement, "isSucceeded").GetBoolean(), Is.True, content);
        return payload;
    }

    private static async Task<JsonDocument> PostSuccessfulPayloadAsync(HttpClient client, string url, object body)
    {
        using var response = await client.PostAsJsonAsync(url, body);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(response.IsSuccessStatusCode, Is.True, content);
        var payload = JsonDocument.Parse(content);
        Assert.That(Property(payload.RootElement, "isSucceeded").GetBoolean(), Is.True, content);
        return payload;
    }

    [Test]
    [Category("HostLive")]
    public async Task Host_my_menu_operational_modules_have_a_live_read_contract()
    {
        var loginId = Environment.GetEnvironmentVariable("AXIONPRO_HOST_LOGIN_ID");
        var password = Environment.GetEnvironmentVariable("AXIONPRO_HOST_LOGIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrWhiteSpace(password))
            Assert.Ignore("HostLive requires AXIONPRO_HOST_LOGIN_ID and AXIONPRO_HOST_LOGIN_PASSWORD.");
        using var client = new HttpClient { BaseAddress = new Uri(TestSettings.ApiBaseUrl), Timeout = TimeSpan.FromSeconds(60) };
        using var loginResponse = await client.PostAsJsonAsync("/api/NewLogin/login", new { LoginId = loginId, Password = password, IpAddressLocal = "127.0.0.1", IpAddressPublic = "", MacAddress = "", LoginDevice = 1, Latitude = 0, Longitude = 0 });
        Assert.That(loginResponse.IsSuccessStatusCode, Is.True);
        using var login = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Property(Property(login.RootElement, "data"), "accessToken").GetString());
        using var menuResponse = await client.GetAsync("/api/Navigation/my-menu");
        Assert.That(menuResponse.IsSuccessStatusCode, Is.True);
        using var menu = JsonDocument.Parse(await menuResponse.Content.ReadAsStringAsync());
        var modules = FlattenModules(Property(Property(menu.RootElement, "data"), "items"));
        var tenantListModule = modules.First(x => Property(x, "moduleCode").GetString() == "HOST_TENANT_LIST");
        using var tenantList = await client.GetAsync($"/api/Tenant/get-all-tenants?PageNumber=1&PageSize=10&ModuleId={Property(tenantListModule,"id")}&OperationId={ViewOperation(tenantListModule)}");
        Assert.That(tenantList.IsSuccessStatusCode, Is.True);
        using var tenantPayload = JsonDocument.Parse(await tenantList.Content.ReadAsStringAsync());
        var tenantId = Property(tenantPayload.RootElement, "data").EnumerateArray().FirstOrDefault();
        var tenantKey = tenantId.ValueKind == JsonValueKind.Undefined ? null : Property(tenantId, "id").GetString();
        var routes = new Dictionary<string, Func<JsonElement, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["HOST_DEVICE_INVENTORY"] = m => $"/api/DeviceMaster/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["TENANT_DEVICE_CONFIG"] = m => $"/api/TenantDeviceConfiguration/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["TENANT_DEVICES"] = m => $"/api/TenantDevice/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_TENANT_LIST"] = m => $"/api/Tenant/get-all-tenants?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_TENANT_LOCATION_LIST"] = m => $"/api/TenantLocation/get-all?IsActive=true&PageNumber=1&PageSize=500&TenantId={tenantKey}&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_USERS"] = m => $"/api/Host/get-all-host-users?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_ROLES"] = m => $"/api/Host/get-all-host-roles?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_MODULES"] = m => $"/api/Host/get-host-modules?ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_OPERATIONS"] = m => $"/api/OperationsMaster/get-all-operations?ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_MODULE_OPERATIONS"] = m => $"/api/ModuleOperation/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_DEFAULT_EMAIL_CONFIG"] = m => $"/api/DefaultEmailConfig/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_EMAIL_TEMPLATE"] = m => $"/api/EmailTemplate/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_DEVICE_SETUP"] = m => $"/api/DeviceMaster/get-all?PageNumber=1&PageSize=10&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_SUBSCRIPTIONS"] = m => "/api/Subscription/get-all-host-subscription-plans",
            ["HOST_TENANT_CARD_INVENTORY"] = m => $"/api/TenantCardMaster/get-all?PageNumber=1&PageSize=10&TenantId={tenantKey}&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}",
            ["HOST_TENANT_EMAIL_CONFIG"] = m => $"/api/TenantEmailConfig/get-all?PageNumber=1&PageSize=10&TenantId={tenantKey}&ModuleId={Property(m,"id")}&OperationId={ViewOperation(m)}"
        };
        var failures = new List<string>();
        foreach (var module in modules)
        {
            var code = Property(module, "moduleCode").GetString()!;
            if (!routes.TryGetValue(code, out var route)) continue;
            using var response = code.Equals("HOST_SUBSCRIPTIONS", StringComparison.OrdinalIgnoreCase)
                ? await client.PostAsJsonAsync(route(module), new { PageNumber = 1, PageSize = 10 })
                : await client.GetAsync(route(module));
            if (!response.IsSuccessStatusCode) failures.Add($"{code}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        }
        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    private static List<JsonElement> FlattenModules(JsonElement items)
    {
        var result = new List<JsonElement>();
        foreach (var item in items.EnumerateArray())
        {
            result.Add(item);
            if (item.TryGetProperty("children", out var children)) result.AddRange(FlattenModules(children));
        }
        return result;
    }

    private static int ViewOperation(JsonElement module) => module.GetProperty("operations").EnumerateArray()
        .First(x => string.Equals(Property(x, "name").GetString(), "View", StringComparison.OrdinalIgnoreCase)).GetProperty("id").GetInt32();

    [Test]
    public async Task Protected_navigation_endpoint_rejects_an_anonymous_request()
    {
        var response = await Api.GetAsync("/api/Navigation/my-menu");

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
