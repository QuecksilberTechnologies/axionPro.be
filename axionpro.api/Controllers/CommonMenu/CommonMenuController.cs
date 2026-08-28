// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated application-wide Common menu navigation for Host and Tenant users.
// ================================================================

using axionpro.application.Features.ModuleCmd.CommonMenu.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.CommonMenu;

/// <summary>
/// Retrieves the shared Common navigation hierarchy for an authenticated Host or Tenant user.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CommonMenuController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonMenuController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches authenticated Common-menu queries.</param>
    public CommonMenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Common Menu Endpoints

    /// <summary>
    /// Supports the Angular UI flow for get.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves common menu.</para>
    /// <para>Angular page(s): /auth/login; /app/admin-dashboard; /app/dashboard; /app/okr/dashboard; /app/okr/:id; /app/okr/my; /app/okr/team; /app/okr/company; and 18 more.</para>
    /// <para>Angular API service call(s): CommonMenuApi.getCommonMenu (app/core/services/common-menu-api.ts:19).</para>
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCommonMenuQuery(), cancellationToken);
        return Ok(result);
    }

    #endregion
}
