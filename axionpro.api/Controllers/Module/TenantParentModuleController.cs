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
    /// Get Tenant Module Headers.
    /// </summary>
    /// <remarks>
    /// Retrieves Main Parent Headers and qualifying direct Sub-Parent Headers only when each Module has an explicit TenantEnabledModule entitlement row.
    /// Requires an authenticated Host Super Admin. The supplied and returned Tenant identifiers are encrypted strings; <c>ModuleScope</c> is required and <c>IsEnabled</c> is optional.
    /// A Parent remains in the response with an empty <c>Children</c> collection when it has no qualifying child headers. Leaf Modules are not returned.
    /// </remarks>
    /// <param name="requestDTO">The encrypted Tenant identifier, Module scope, and optional Tenant entitlement enabled-state filter.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The Tenant-entitled Header tree.</returns>
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
    /// List Tenant Main Parent Headers.
    /// </summary>
    /// <remarks>
    /// Retrieves a server-paged, all-Tenant list of explicitly provisioned Main Parent Headers only.
    /// Requires an authenticated Host Super Admin. The optional <c>IsActive</c> filter maps to Tenant entitlement <c>IsEnabled</c>.
    /// Each returned Tenant identifier is encrypted; Sub-Parent Headers and Leaf Modules are not included.
    /// </remarks>
    /// <param name="requestDTO">The optional active-state filter and paging values.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The requested page of Main Parent Header entitlements.</returns>
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
    /// Get Tenant Parent Module by global Module identifier.
    /// </summary>
    /// <remarks>
    /// Retrieves one explicitly provisioned Tenant Header Module by its global <c>ModuleId</c>; the internal TenantEnabledModule row identifier is never exposed.
    /// Requires an authenticated Host Super Admin. The supplied and returned Tenant identifiers are encrypted strings.
    /// </remarks>
    /// <param name="id">The global Header Module identifier.</param>
    /// <param name="requestDTO">The encrypted Tenant identifier and required Module scope.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The Tenant-entitled Header Module.</returns>
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
    /// Update Tenant Parent Module status.
    /// </summary>
    /// <remarks>
    /// Updates one explicitly provisioned Main Parent or Sub-Parent Header Module and every entitled descendant for the supplied Tenant.
    /// Requires an authenticated Host Super Admin. The supplied Tenant identifier is encrypted; Leaf Modules cannot be targeted directly.
    /// Ancestors and sibling branches are not changed.
    /// When enabling, only descendant operation entitlements marked as used are enabled. When disabling, operation usage selections are preserved.
    /// </remarks>
    /// <param name="id">The target global Header Module identifier.</param>
    /// <param name="requestDTO">The encrypted Tenant identifier and requested enabled state.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The updated target Header Module.</returns>
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
