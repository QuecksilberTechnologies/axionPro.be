// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Host-user authorization and Host operational-access bootstrap endpoints.
// ================================================================

using axionpro.application.Features.HostCmd.Handler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Host;

/// <summary>
/// Exposes the current Host user's module and operation access bootstrap.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class HostAccessController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HostAccessController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches Host access bootstrap queries.</param>
    public HostAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Host Access Endpoints

    [HttpGet("bootstrap")]
    public async Task<IActionResult> GetBootstrap(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetHostAccessQuery(), cancellationToken);
        return Ok(result);
    }

    #endregion
}
