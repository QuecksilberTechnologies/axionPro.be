// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes streamlined Host and Tenant Employee authentication through MediatR.
// ================================================================

using axionpro.application.DTOs.UserLogin;
using axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Login;

/// <summary>
/// Exposes the compact Host and Tenant Employee login endpoint while preserving the existing Auth login endpoint.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class NewLoginController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="NewLoginController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches the streamlined login command.</param>
    public NewLoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Authentication Endpoints

    /// <summary>
    /// Used-In-Angular: authenticates the user.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): AuthApi.login (app/core/services/auth-api.ts:162).</para>
    /// <para>Angular purpose: authenticates the user.</para>
    /// <para>Integrated UI page(s): /auth/login</para>
    /// <para>Angular UI component(s): Login (app/features/authentication/login/login.ts)</para>
    /// </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDTO request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new NewLoginCommand(request),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
