using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace AxionPro.AutomationTests.Infrastructure;

/// <summary>
/// Creates an isolated, unauthenticated API request context for every test.
/// </summary>
public abstract class ApiTestBase : PlaywrightTest
{
    protected IAPIRequestContext Api = null!;

    [SetUp]
    public async Task CreateApiRequestContext()
    {
        Api = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = TestSettings.ApiBaseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "AxionPro.AutomationTests"
            }
        });
    }

    [TearDown]
    public async Task DisposeApiRequestContext()
    {
        if (Api is not null)
        {
            await Api.DisposeAsync();
        }
    }
}
