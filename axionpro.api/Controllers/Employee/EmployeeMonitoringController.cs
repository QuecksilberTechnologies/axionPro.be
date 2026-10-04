using axionpro.application.DTOS.EmployeeMonitoring;
using axionpro.application.Features.EmployeeCmd.EmployeeMonitoring;
using axionpro.application.Features.EmployeeMonitoringAgent;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace axionpro.api.Controllers.Employee;

/// <summary>Configures employee monitoring and exposes authenticated agent synchronization endpoints.</summary>
[ApiController]
[Route("api/Employee/Monitoring")]
public sealed class EmployeeMonitoringController(IMediator mediator, ILoggerService logger) : ControllerBase
{
    #region Tenant administration

    /// <summary>Creates or updates the current Tenant's employee-monitoring policy.</summary>
    [Authorize]
    [HttpPost("policy")]
    public async Task<IActionResult> UpsertPolicy([FromBody] UpsertEmployeeMonitoringPolicyRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInfo("Received employee monitoring policy request.");
        return Ok(await mediator.Send(new UpsertEmployeeMonitoringPolicyCommand(dto), cancellationToken));
    }

    /// <summary>Registers an employee PC agent and returns its credential once.</summary>
    [Authorize]
    [HttpPost("agent/register")]
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterEmployeeMonitoringAgentRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInfo("Received employee monitoring agent registration request.");
        return Ok(await mediator.Send(new RegisterEmployeeMonitoringAgentCommand(dto), cancellationToken));
    }

    /// <summary>Returns current agent connectivity and screenshot-upload status.</summary>
    [Authorize]
    [HttpGet("agent/status")]
    public async Task<IActionResult> GetAgentStatus([FromQuery] EmployeeMonitoringPermissionRequestDTO permissionRequest, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetEmployeeMonitoringAgentsQuery(permissionRequest), cancellationToken));
    }

    #endregion

    #region Installed agent runtime

    /// <summary>Returns the server-owned policy for an enrolled agent.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("employee-monitoring-agent")]
    [HttpGet("runtime/configuration")]
    public async Task<IActionResult> GetRuntimeConfiguration([FromHeader(Name = "X-Agent-Credential")] string credential, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAgentConfigurationQuery(credential), cancellationToken));
    }

    /// <summary>Records the agent's latest connectivity and local-queue state.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("employee-monitoring-agent")]
    [HttpPost("runtime/heartbeat")]
    public async Task<IActionResult> Heartbeat(
        [FromHeader(Name = "X-Agent-Credential")] string credential,
        [FromBody] AgentHeartbeatRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new RecordAgentHeartbeatCommand(credential, dto), cancellationToken));
    }

    /// <summary>Idempotently uploads one offline or online employee screenshot.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("employee-monitoring-agent")]
    [HttpPost("runtime/captures")]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> UploadCapture(
        [FromHeader(Name = "X-Agent-Credential")] string credential,
        [FromForm] Guid captureId,
        [FromForm] DateTime capturedAtUtc,
        [FromForm] int monitorNumber,
        [FromForm] string? checksumSha256,
        IFormFile screenshot,
        CancellationToken cancellationToken)
    {
        await using var content = screenshot.OpenReadStream();
        var command = new UploadEmployeeScreenCaptureCommand(
            credential,
            captureId,
            capturedAtUtc,
            monitorNumber,
            screenshot.ContentType,
            screenshot.Length,
            checksumSha256,
            content);
        return Ok(await mediator.Send(command, cancellationToken));
    }

    #endregion
}
