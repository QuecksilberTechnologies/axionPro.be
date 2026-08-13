// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes HostRole module-operation permission assignment APIs.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostCmd.Handler;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Host;

/// <summary>
/// Coordinates HostRole module-operation permission requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Retrieves available module-operation permissions and selection state for one Host role.
    /// </summary>
    /// <param name="hostRoleId">The Host-role identifier.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The HostRole permission selection structure.</returns>
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
    /// Saves the complete selected module-operation permission set for one Host role.
    /// </summary>
    /// <param name="dto">The selected module-operation permissions.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The number of permissions inserted, reactivated, or deactivated.</returns>
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
