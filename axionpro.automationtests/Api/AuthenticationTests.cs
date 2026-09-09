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

    [Test]
    public async Task Protected_navigation_endpoint_rejects_an_anonymous_request()
    {
        var response = await Api.GetAsync("/api/Navigation/my-menu");

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
