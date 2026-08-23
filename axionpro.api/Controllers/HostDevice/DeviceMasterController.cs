// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Host administration endpoints for the DeviceMaster catalog.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides authenticated Host endpoints for global device model administration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class DeviceMasterController(IMediator mediator, ILogger<DeviceMasterController> logger) : ControllerBase
{
    #region Device Master Endpoints

    /// <summary>Creates a device model.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster create request."); return Ok(await mediator.Send(new CreateDeviceMasterCommand(dto), cancellationToken)); }

    /// <summary>Gets a device model by identifier.</summary>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster get-by-id request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new GetDeviceMasterByIdQuery(id), cancellationToken)); }

    /// <summary>Gets a database-paged device model list.</summary>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetDeviceMasterListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster list request."); return Ok(await mediator.Send(new GetAllDeviceMastersQuery(filter), cancellationToken)); }

    /// <summary>Updates a device model.</summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster update request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterCommand(dto), cancellationToken)); }

    /// <summary>Updates a device model active state.</summary>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateDeviceMasterStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster status request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterStatusCommand(dto), cancellationToken)); }

    /// <summary>Soft deletes a device model.</summary>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster delete request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new DeleteDeviceMasterCommand(id), cancellationToken)); }

    #endregion
}
