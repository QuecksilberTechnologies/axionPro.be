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
    /// Supports the Angular UI flow for get role module permissions.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes host role.</para>
    /// <para>Angular page(s): /app/host-roles.</para>
    /// <para>Angular API service call(s): HostApi.deleteHostRole (app/core/services/host-api.ts:124).</para>
    /// </remarks>
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
    /// Supports the Angular UI flow for save role module permissions.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes host role.</para>
    /// <para>Angular page(s): /app/host-roles.</para>
    /// <para>Angular API service call(s): HostApi.deleteHostRole (app/core/services/host-api.ts:139).</para>
    /// </remarks>
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
