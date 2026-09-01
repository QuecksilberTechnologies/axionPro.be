// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes the authenticated user's lightweight, permission-filtered application navigation.
// ================================================================

using axionpro.application.Features.NavigationCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Navigation;

/// <summary>
/// Provides navigation data for the authenticated application's hamburger menu.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NavigationController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationController"/> class.
    /// </summary>
    public NavigationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the authenticated user's lightweight, permission-filtered navigation hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>Backend endpoint: GET /api/navigation/my-menu.</para>
    /// <para>For Tenant users, navigation is resolved from the tenant entitlement snapshot and the user's effective role permissions.</para>
    /// </remarks>
    [HttpGet("my-menu")]
    public async Task<IActionResult> GetMyMenu(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyNavigationMenuQuery(), cancellationToken);
        return Ok(result);
    }
}
