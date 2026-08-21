// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes Host-managed Subscription Plan to Module mapping endpoints.
// ================================================================

using axionpro.application.DTOs.PlanModule;
using axionpro.application.Features.PlanModuleMappingCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Subscription;

/// <summary>
/// Exposes HTTP transport endpoints for Host-managed Subscription Plan Module mappings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PlanModuleMappingController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanModuleMappingController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches Subscription Plan Module mapping commands and queries.</param>
    public PlanModuleMappingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Plan Module Mapping Endpoints

    /// <summary>
    /// Retrieves the eligible Module hierarchy and current active mapping state for a Subscription Plan.
    /// </summary>
    /// <param name="subscriptionPlanId">The Subscription Plan identifier.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The Module mapping popup options.</returns>
    [HttpGet("options/{subscriptionPlanId:int}")]
    public async Task<IActionResult> GetOptions(
        int subscriptionPlanId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPlanModuleMappingOptionsQuery(subscriptionPlanId),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Atomically synchronizes selected Modules for a Subscription Plan using an add, reactivate, and unmap delta.
    /// </summary>
    /// <param name="requestDTO">The Subscription Plan Module selection request.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The applied mapping delta summary.</returns>
    [HttpPost("save")]
    public async Task<IActionResult> Save(
        [FromBody] SavePlanModuleMappingRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SavePlanModuleMappingCommand(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
