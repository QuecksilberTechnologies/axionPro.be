using System.Reflection;
using axionpro.api.Controllers.Employee;
using axionpro.application.Common.Models;
using axionpro.application.DTOS.EmployeeMonitoring;
using axionpro.application.Exceptions;
using axionpro.application.Features.EmployeeCmd.EmployeeMonitoring;
using axionpro.infrastructure.FileStoringService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeMonitoring")]
public sealed class EmployeeMonitoringTests
{
    [Test]
    public void Tenant_administration_endpoints_require_bearer_authentication()
    {
        var names = new[]
        {
            nameof(EmployeeMonitoringController.UpsertPolicy),
            nameof(EmployeeMonitoringController.RegisterAgent),
            nameof(EmployeeMonitoringController.GetAgentStatus)
        };

        Assert.Multiple(() =>
        {
            foreach (var name in names)
            {
                var method = typeof(EmployeeMonitoringController).GetMethod(name);
                Assert.That(method, Is.Not.Null, name);
                Assert.That(method!.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null, name);
                Assert.That(method.GetCustomAttribute<AllowAnonymousAttribute>(), Is.Null, name);
            }
        });
    }

    [Test]
    public void Agent_runtime_endpoints_use_agent_credential_boundary_and_rate_limit()
    {
        var names = new[]
        {
            nameof(EmployeeMonitoringController.GetRuntimeConfiguration),
            nameof(EmployeeMonitoringController.Heartbeat),
            nameof(EmployeeMonitoringController.UploadCapture)
        };

        Assert.Multiple(() =>
        {
            foreach (var name in names)
            {
                var method = typeof(EmployeeMonitoringController).GetMethod(name);
                Assert.That(method, Is.Not.Null, name);
                Assert.That(method!.GetCustomAttribute<AllowAnonymousAttribute>(), Is.Not.Null, name);
                Assert.That(method.GetCustomAttribute<EnableRateLimitingAttribute>()?.PolicyName,
                    Is.EqualTo("employee-monitoring-agent"), name);
                Assert.That(method.GetParameters().Any(parameter =>
                    parameter.GetCustomAttributes().Any(attribute =>
                        attribute.GetType().Name == "FromHeaderAttribute")), Is.True, name);
            }
        });
    }

    [TestCase(59, 600, 60, 180, 600, 3, 1024, 70)]
    [TestCase(600, 300, 60, 180, 600, 3, 1024, 70)]
    [TestCase(300, 600, 60, 60, 600, 3, 1024, 70)]
    [TestCase(300, 600, 60, 180, 180, 3, 1024, 70)]
    [TestCase(300, 600, 60, 180, 600, 0, 1024, 70)]
    [TestCase(300, 600, 60, 180, 600, 3, 0, 70)]
    [TestCase(300, 600, 60, 180, 600, 3, 1024, 101)]
    public void Monitoring_policy_rejects_unsafe_ranges(
        int minimum,
        int maximum,
        int heartbeat,
        int delayed,
        int unreachable,
        int retentionDays,
        long maximumBytes,
        int quality)
    {
        var handler = new UpsertEmployeeMonitoringPolicyCommandHandler(null!, null!);
        var request = new UpsertEmployeeMonitoringPolicyRequestDTO
        {
            MinimumCaptureIntervalSeconds = minimum,
            MaximumCaptureIntervalSeconds = maximum,
            HeartbeatIntervalSeconds = heartbeat,
            DelayedAfterSeconds = delayed,
            UnreachableAfterSeconds = unreachable,
            OfflineRetentionDays = retentionDays,
            MaximumOfflineBytes = maximumBytes,
            ImageQuality = quality
        };

        Assert.ThrowsAsync<ValidationErrorException>(async () =>
            await handler.Handle(new UpsertEmployeeMonitoringPolicyCommand(request), CancellationToken.None));
    }

    [Test]
    public async Task File_storage_generates_tenant_and_employee_isolated_object_key()
    {
        var root = Path.Combine(Path.GetTempPath(), "axionpro-monitoring-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new EmployeeScreenshotFileStorage(Options.Create(new EmployeeMonitoringStorageOptions
            {
                Provider = "FileSystem",
                RootPath = root,
                MaximumUploadBytes = 1024,
                AllowedContentTypes = ["image/jpeg"]
            }));
            var captureId = Guid.CreateVersion7();
            await using var content = new MemoryStream([1, 2, 3, 4]);

            var objectKey = await storage.SaveAsync(12, 34, captureId, "image/jpeg", content, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(objectKey, Does.StartWith("tenant/12/employee/34/"));
                Assert.That(objectKey, Does.EndWith(captureId.ToString("N") + ".jpg"));
                Assert.That(File.Exists(Path.Combine(root, objectKey.Replace('/', Path.DirectorySeparatorChar))), Is.True);
            });
            await storage.DeleteAsync(objectKey, CancellationToken.None);
            Assert.That(File.Exists(Path.Combine(root, objectKey.Replace('/', Path.DirectorySeparatorChar))), Is.False);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }
}
