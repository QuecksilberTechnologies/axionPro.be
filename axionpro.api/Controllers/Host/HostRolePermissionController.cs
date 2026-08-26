// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes HostRole module-operation permission assignment APIs.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostCmd.Handler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Host;

/// <summary>
/// Coordinates HostRole module-operation permission requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HostRolePermissionController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HostRolePermissionController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches HostRole permission commands and queries.</param>
    public HostRolePermissionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region HostRole Module Permissions

    /// <summary>
    /// Get Role Module Permissions.
    /// </summary>
    /// <remarks>
    /// Handles the request to get role module permissions.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="hostRoleId">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-role-module-permissions/{hostRoleId:long}")]
    public async Task<IActionResult> GetRoleModulePermissions(
        long hostRoleId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHostRoleModulePermissionsQuery(hostRoleId),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Save Role Module Permissions.
    /// </summary>
    /// <remarks>
    /// Handles the request to save role module permissions.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="dto">The request body used to save role module permissions.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("save-role-module-permissions")]
    public async Task<IActionResult> SaveRoleModulePermissions(
        [FromBody] SaveHostRoleModulePermissionsRequestDTO? dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SaveHostRoleModulePermissionsCommand(dto),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
