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

    /// <summary>
    /// Supports the Angular UI flow for create.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: creates device master.</para>
    /// <para>Angular page(s): /app/device-masters/new; /app/device-masters/:deviceMasterId/edit.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.addDeviceMaster (app/core/services/device-master-api.ts:39).</para>
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster create request."); return Ok(await mediator.Send(new CreateDeviceMasterCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for get by id.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves device master.</para>
    /// <para>Angular page(s): /app/device-masters/new; /app/device-masters/:deviceMasterId/edit.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.getDeviceMaster (app/core/services/device-master-api.ts:33).</para>
    /// </remarks>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster get-by-id request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new GetDeviceMasterByIdQuery(id), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for get all.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves device masters.</para>
    /// <para>Angular page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit; /app/device-masters; /app/tenant-devices.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.getDeviceMasters (app/core/services/device-master-api.ts:27).</para>
    /// </remarks>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetDeviceMasterListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster list request."); return Ok(await mediator.Send(new GetAllDeviceMastersQuery(filter), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for update.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates device master.</para>
    /// <para>Angular page(s): /app/device-masters/new; /app/device-masters/:deviceMasterId/edit.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.updateDeviceMaster (app/core/services/device-master-api.ts:46).</para>
    /// </remarks>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster update request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for update status.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates device master status.</para>
    /// <para>Angular page(s): /app/device-masters.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.setDeviceMasterStatus (app/core/services/device-master-api.ts:52).</para>
    /// </remarks>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateDeviceMasterStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster status request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterStatusCommand(dto), cancellationToken)); }

    /// <summary>
    /// Supports the Angular UI flow for delete.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes device master.</para>
    /// <para>Angular page(s): /app/device-masters.</para>
    /// <para>Angular API service call(s): DeviceMasterApi.deleteDeviceMaster (app/core/services/device-master-api.ts:58).</para>
    /// </remarks>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster delete request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new DeleteDeviceMasterCommand(id), cancellationToken)); }

    #endregion
}
