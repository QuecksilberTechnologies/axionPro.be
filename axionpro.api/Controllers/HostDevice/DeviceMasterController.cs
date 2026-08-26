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
    public async Task<IActionResult> Create([FromBody] CreateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster create request."); return Ok(await mediator.Send(new CreateDeviceMasterCommand(dto), cancellationToken)); }

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
    { logger.LogInformation("Received DeviceMaster get-by-id request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new GetDeviceMasterByIdQuery(id), cancellationToken)); }

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
    public async Task<IActionResult> GetAll([FromQuery] GetDeviceMasterListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster list request."); return Ok(await mediator.Send(new GetAllDeviceMastersQuery(filter), cancellationToken)); }

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
    public async Task<IActionResult> Update([FromBody] UpdateDeviceMasterRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster update request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterCommand(dto), cancellationToken)); }

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
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateDeviceMasterStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received DeviceMaster status request for {DeviceMasterId}.", dto.Id); return Ok(await mediator.Send(new UpdateDeviceMasterStatusCommand(dto), cancellationToken)); }

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
    { logger.LogInformation("Received DeviceMaster delete request for {DeviceMasterId}.", id); return Ok(await mediator.Send(new DeleteDeviceMasterCommand(id), cancellationToken)); }

    #endregion
}
