// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Manages Host-controlled physical Tenant device registration, configuration, and lifecycle.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides authenticated Host endpoints for physical Tenant device administration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceController(IMediator mediator, ILogger<TenantDeviceController> logger) : ControllerBase
{
    #region Tenant Device Endpoints

    /// <summary>Creates a physical Tenant device.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice create request for Tenant {TenantId}.", dto.TenantId); return Ok(await mediator.Send(new CreateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>Gets a physical Tenant device by identifier.</summary>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice get-by-id request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new GetTenantDeviceByIdQuery(id), cancellationToken)); }

    /// <summary>Gets a database-paged physical Tenant device list.</summary>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice list request."); return Ok(await mediator.Send(new GetAllTenantDevicesQuery(filter), cancellationToken)); }

    /// <summary>Updates a physical Tenant device.</summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice update request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>Updates a physical Tenant device active state.</summary>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantDeviceStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice status request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceStatusCommand(dto), cancellationToken)); }

    /// <summary>Soft deletes a physical Tenant device.</summary>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice delete request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new DeleteTenantDeviceCommand(id), cancellationToken)); }

    #endregion
}
