// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Tenant employee authorization and operational-navigation bootstrap endpoints.
// ================================================================

using axionpro.application.Features.UserAccessCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.TenantUserAccess;

/// <summary>
/// Exposes the current Tenant employee operational-access bootstrap.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TenantUserAccessController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantUserAccessController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches Tenant access bootstrap queries.</param>
    public TenantUserAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Tenant Access Endpoints

    /// <summary>
    /// Get Bootstrap.
    /// </summary>
    /// <remarks>
    /// Handles the request to get bootstrap.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("bootstrap")]
    public async Task<IActionResult> GetBootstrap(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantUserAccessQuery(), cancellationToken);
        return Ok(result);
    }

    #endregion
}
