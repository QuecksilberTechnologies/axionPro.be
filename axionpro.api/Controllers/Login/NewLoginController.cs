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
    /// Authenticates a Host user or Tenant Employee and returns the applicable compact session bootstrap response.
    /// </summary>
    /// <param name="request">The existing login request contract shared with the legacy login endpoint.</param>
    /// <param name="cancellationToken">A token used to cancel request processing.</param>
    /// <returns>The compact authenticated Host or Tenant Employee session response.</returns>
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
