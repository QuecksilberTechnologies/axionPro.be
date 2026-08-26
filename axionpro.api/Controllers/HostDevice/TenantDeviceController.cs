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

    /// <summary>
    /// Create.
    /// </summary>
    /// <remarks>
    /// Handles the request to create.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="dto">The request body used to create.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice create request for Tenant {TenantId}.", dto.TenantId); return Ok(await mediator.Send(new CreateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Get By ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get by id.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice get-by-id request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new GetTenantDeviceByIdQuery(id), cancellationToken)); }

    /// <summary>
    /// Get All.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="filter">The query parameters used to get all.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice list request."); return Ok(await mediator.Send(new GetAllTenantDevicesQuery(filter), cancellationToken)); }

    /// <summary>
    /// Update.
    /// </summary>
    /// <remarks>
    /// Handles the request to update.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="dto">The request body used to update.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice update request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Update Status.
    /// </summary>
    /// <remarks>
    /// Handles the request to update status.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="dto">The request body used to update status.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantDeviceStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice status request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceStatusCommand(dto), cancellationToken)); }

    /// <summary>
    /// Delete.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice delete request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new DeleteTenantDeviceCommand(id), cancellationToken)); }

    #endregion
}
