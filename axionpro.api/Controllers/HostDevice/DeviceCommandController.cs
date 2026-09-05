// ================================================================
// Purpose : Exposes authenticated submission to the MQTT device command queue.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.DeviceCommandCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Queues supported commands for Tenant devices through the central MQTT/MQTTS gateway.</summary>
[Authorize]
[ApiController]
[Route("api/device-commands")]
public sealed class DeviceCommandController(IMediator mediator) : ControllerBase
{
    /// <summary>Queues a protocol-confirmed command after current Host or Tenant permission validation.</summary>
    [HttpPost("submit")]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitDeviceCommandRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new SubmitDeviceCommand(dto), cancellationToken));
}
