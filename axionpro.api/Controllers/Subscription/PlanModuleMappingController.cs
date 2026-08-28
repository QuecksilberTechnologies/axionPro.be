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
    /// Supports the Angular UI flow for save.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: performs save plan module mapping.</para>
    /// <para>Angular page(s): /app/subscriptions.</para>
    /// <para>Angular API service call(s): PlanModuleMappingApi.savePlanModuleMapping (app/core/services/plan-module-mapping-api.ts:32).</para>
    /// </remarks>
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
