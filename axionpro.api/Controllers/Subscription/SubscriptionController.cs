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
    /// <summary>
    /// Retrieves subscription plans for the public pricing flow.
    /// </summary>
    /// <remarks>
    /// Supports the Angular pricing store call to
    /// <c>GET /api/Subscription/get-all-subscription-plan</c>.
    /// </remarks>
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
    /// <summary>Retrieves subscription plans for the authenticated Host user.</summary>
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

    /// <summary>Retrieves the current subscription plan information for a tenant.</summary>
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

    /// <summary>Retrieves modules accessible through a subscription plan.</summary>
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
    /// <summary>Creates a subscription plan for the authenticated Host user.</summary>
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
    /// Used-In-Angular: updates subscription plan.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): SubscriptionsApi.updateSubscriptionPlan (app/core/services/subscriptions-api.ts:95).</para>
    /// <para>Angular purpose: updates subscription plan.</para>
    /// <para>Integrated UI page(s): /app/subscriptions</para>
    /// <para>Angular UI component(s): SubscriptionPlanForm (app/features/host/subscriptions/subscription-plan-form/subscription-plan-form.ts); SubscriptionsStore (app/features/host/subscriptions/subscriptions.store.ts); Subscriptions (app/features/host/subscriptions/subscriptions.ts)</para>
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
    /// <summary>Soft deletes a subscription plan for the authenticated Host user.</summary>
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
