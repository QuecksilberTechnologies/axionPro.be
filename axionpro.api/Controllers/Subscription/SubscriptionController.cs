// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes HTTP endpoints for subscription plan management and delegates application logic through MediatR.
// ================================================================

using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Features.SubscriptionCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Subscription;

/// <summary>
/// Exposes HTTP transport endpoints for subscription plan queries and commands.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches subscription commands and queries.</param>
    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Subscription Plan Queries

    [AllowAnonymous]
    [HttpGet("get-all-subscription-plan")]
    public async Task<IActionResult> GetAllSubscriptionPlan(
        [FromQuery] SubscriptionPlanRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubscriptionPlanQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("get-all-host-subscription-plans")]
    public async Task<IActionResult> GetAllHostSubscriptionPlans(
        [FromBody] HostSubscriptionPlanListRequestDTO? requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHostSubscriptionPlansQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("get-tenant-subscription-plan-info")]
    public async Task<IActionResult> GetTenantSubscriptionPlanInfo(
        [FromQuery] TenantSubscriptionPlanRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantSubscriptionPlanQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("get-all-tenant-accessible-modules")]
    public async Task<IActionResult> GetAllTenantAccessibleModules(
        [FromQuery] PlanModuleMappingRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubscriptionPlanModulesQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Subscription Plan Commands

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateSubscriptionRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateSubscriptionPlanCommand(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for update subscription.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates subscription plan.</para>
    /// <para>Angular page(s): /app/subscriptions.</para>
    /// <para>Angular API service call(s): SubscriptionsApi.updateSubscriptionPlan (app/core/services/subscriptions-api.ts:94).</para>
    /// </remarks>
    [Authorize]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateSubscription(
        long id,
        [FromBody] UpdateSubscriptionRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateSubscriptionPlanCommand(id, requestDTO),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("delete-subscription-plan")]
    public async Task<IActionResult> DeleteSubscriptionPlan(
        [FromBody] DeleteSubscriptionPlanRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteSubscriptionPlanCommand(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
