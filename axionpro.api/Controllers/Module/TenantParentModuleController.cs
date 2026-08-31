// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates Host Super Admin HTTP requests for Tenant-enabled Parent/Header Modules.
// ================================================================

using axionpro.application.DTOS.Module.TenantParentModule;
using axionpro.application.Features.ModuleCmd.TenantParent.Handlers;
using axionpro.application.Features.ModuleCmd.TenantParent.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module;

/// <summary>
/// Provides Host Super Admin read endpoints for a Tenant's explicitly provisioned Parent and Sub-Parent Header Modules.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TenantParentModuleController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantParentModuleController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches Tenant Parent Module queries.</param>
    public TenantParentModuleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Tenant Parent Module Queries
    /// <summary>
    /// Not-Used-In-Angular.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Not-Used-In-Angular.</para>
    /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    /// <para>Backend endpoint: GET /api/tenantparentmodule/get-module-headers.</para>
    /// </remarks>

    [HttpGet("get-module-headers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModuleHeaders(
        [FromQuery] TenantParentModuleHeaderRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantParentModuleHeadersQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Not-Used-In-Angular.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Not-Used-In-Angular.</para>
    /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    /// <para>Backend endpoint: GET /api/tenantparentmodule/list.</para>
    /// </remarks>

    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModules(
        [FromQuery] TenantParentModuleListRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantParentModulesQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Not-Used-In-Angular.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Not-Used-In-Angular.</para>
    /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    /// <para>Backend endpoint: GET /api/tenantparentmodule/{}.</para>
    /// </remarks>

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModuleById(
        int id,
        [FromQuery] TenantParentModuleByIdRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantParentModuleByIdQuery(id, requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Tenant Parent Module Status
    /// <summary>
    /// Not-Used-In-Angular.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Not-Used-In-Angular.</para>
    /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    /// <para>Backend endpoint: PATCH /api/tenantparentmodule/{}/status.</para>
    /// </remarks>

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateModuleStatus(
        int id,
        [FromBody] UpdateTenantParentModuleStatusRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateTenantParentModuleStatusCommand(id, requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
