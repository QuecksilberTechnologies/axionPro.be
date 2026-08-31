using AxionPro.AutomationTests.Infrastructure;
using NUnit.Framework;

namespace AxionPro.AutomationTests.Api;

[TestFixture]
[Category("API")]
public sealed class AuthenticationTests : ApiTestBase
{
    [Test]
    public async Task Protected_navigation_endpoint_rejects_an_anonymous_request()
    {
        var response = await Api.GetAsync("/api/Navigation/my-menu");

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
