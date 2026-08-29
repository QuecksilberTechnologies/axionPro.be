// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Host and Tenant endpoints for Tenant device connection configuration.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides authenticated Host and Tenant endpoints for separate Tenant device connection configuration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceConfigurationController(IMediator mediator, ILogger<TenantDeviceConfigurationController> logger) : ControllerBase
{
    /// <summary>Creates a connection configuration after a device has been assigned to a Tenant.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceConfigurationRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration create request for TenantDevice {TenantDeviceId}.", dto.TenantDeviceId);
        return Ok(await mediator.Send(new CreateTenantDeviceConfigurationCommand(dto), cancellationToken));
    }

    /// <summary>Retrieves a connection configuration by its raw configuration identifier.</summary>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration get-by-id request for {TenantDeviceConfigurationId}.", id);
        return Ok(await mediator.Send(new GetTenantDeviceConfigurationByIdQuery(id, accessRequest), cancellationToken));
    }

    /// <summary>Retrieves a database-paged configuration list; Host Admin may omit TenantId to retrieve all Tenants.</summary>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceConfigurationListRequestDTO filter, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration list request.");
        return Ok(await mediator.Send(new GetAllTenantDeviceConfigurationsQuery(filter), cancellationToken));
    }

    /// <summary>Updates a connection configuration without accepting runtime telemetry fields.</summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceConfigurationRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration update request for {TenantDeviceConfigurationId}.", dto.Id);
        return Ok(await mediator.Send(new UpdateTenantDeviceConfigurationCommand(dto), cancellationToken));
    }

    /// <summary>Hard deletes a connection configuration. The physical Tenant device remains assigned.</summary>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration delete request for {TenantDeviceConfigurationId}.", id);
        return Ok(await mediator.Send(new DeleteTenantDeviceConfigurationCommand(id, accessRequest), cancellationToken));
    }
}
