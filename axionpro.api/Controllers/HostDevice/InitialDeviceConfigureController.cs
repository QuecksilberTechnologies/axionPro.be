// ================================================================
// Purpose : Host-only initial physical-device bootstrap administration.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>
/// Creates one-time Host bootstrap URLs. The returned URL must be copied to the
/// physical device's Server Domain Name setting; it is never readable later.
/// </summary>
[Authorize]
[ApiController]
[Route("api/initial-device-configure")]
public sealed class InitialDeviceConfigureController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a 20-second-heartbeat initial URL for an unassigned HTTPS-capable device.</summary>
    [HttpPost("issue-bootstrap-url")]
    public async Task<IActionResult> IssueBootstrapUrl(
        [FromBody] IssueInitialDeviceBootstrapRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new IssueInitialDeviceBootstrapCommand(dto), cancellationToken));

    /// <summary>Tenant-admin-only typed configuration endpoint. It never accepts arbitrary vendor JSON.</summary>
    [HttpPost("apply-runtime-configuration")]
    public async Task<IActionResult> ApplyRuntimeConfiguration(
        [FromBody] ApplyTenantDeviceRuntimeConfigurationRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new ApplyTenantDeviceRuntimeConfigurationCommand(dto), cancellationToken));

    /// <summary>Tenant-admin-only reboot endpoint through the device's outbound HTTPS connection.</summary>
    [HttpPost("reboot")]
    public async Task<IActionResult> Reboot(
        [FromBody] RebootTenantDeviceRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new RebootTenantDeviceCommand(dto), cancellationToken));
}
