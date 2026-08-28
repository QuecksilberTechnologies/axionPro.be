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

/// <summary>Provides authenticated Host and Tenant endpoints for physical Tenant device administration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceController(IMediator mediator, ILogger<TenantDeviceController> logger) : ControllerBase
{
    #region Tenant Device Endpoints

    /// <summary>
    /// Supports the Angular UI flow for create.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: creates tenant device.</para>
    /// <para>Angular page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.addTenantDevice (app/core/services/tenant-device-api.ts:39).</para>
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice create request for Tenant {TenantId}.", dto.TenantId); return Ok(await mediator.Send(new CreateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for get by id.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves tenant device.</para>
    /// <para>Angular page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.getTenantDevice (app/core/services/tenant-device-api.ts:33).</para>
    /// </remarks>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice get-by-id request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new GetTenantDeviceByIdQuery(id, accessRequest), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for get all.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves tenant devices.</para>
    /// <para>Angular page(s): /app/tenant-devices; /app/profile/device-enrollment-info.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.getTenantDevices (app/core/services/tenant-device-api.ts:27).</para>
    /// </remarks>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice list request."); return Ok(await mediator.Send(new GetAllTenantDevicesQuery(filter), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for update.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates tenant device.</para>
    /// <para>Angular page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.updateTenantDevice (app/core/services/tenant-device-api.ts:46).</para>
    /// </remarks>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice update request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for update status.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates tenant device status.</para>
    /// <para>Angular page(s): /app/tenant-devices.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.setTenantDeviceStatus (app/core/services/tenant-device-api.ts:52).</para>
    /// </remarks>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantDeviceStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice status request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceStatusCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for delete.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes tenant device.</para>
    /// <para>Angular page(s): /app/tenant-devices.</para>
    /// <para>Angular API service call(s): TenantDeviceApi.deleteTenantDevice (app/core/services/tenant-device-api.ts:58).</para>
    /// </remarks>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice delete request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new DeleteTenantDeviceCommand(id, accessRequest), cancellationToken)); }

    #endregion
}
