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
/// Exposes HTTP transport endpoints for subscription plan operations.
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
    /// Returns non-deleted subscription plans filtered by their requested active status.
    /// </summary>
    /// <param name="requestDTO">The active-status filter for subscription plans.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The filtered subscription plan response.</returns>
    /// 
    [AllowAnonymous]
    [HttpPost("get-all-subscription-plan")]
    public async Task<IActionResult> GetAllSubscriptionPlan(
        [FromBody] SubscriptionPlanRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubscriptionPlanQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Returns subscription information for a tenant according to the supplied criteria.
    /// </summary>
    /// <param name="requestDTO">The tenant subscription query criteria.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The tenant subscription information response.</returns>
    [HttpPost("get-tenant-subscription-plan-info")]
    public async Task<IActionResult> GetTenantSubscriptionPlanInfo(
        [FromBody] TenantSubscriptionPlanRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantSubscriptionPlanQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Returns modules available to the specified tenant subscription plan.
    /// </summary>
    /// <param name="requestDTO">The tenant and subscription plan criteria.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The subscription plan module response.</returns>
    [HttpPost("get-all-tenant-accessible-modules")]
    public async Task<IActionResult> GetAllTenantAccessibleModules(
        [FromBody] PlanModuleMappingRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubscriptionPlanModulesQuery(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Subscription Plan Commands

    /// <summary>
    /// Creates a Host-managed subscription plan.
    /// </summary>
    /// <param name="requestDTO">The subscription plan data to create.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The created subscription plan response.</returns>
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
    /// Updates a Host-managed subscription plan.
    /// </summary>
    /// <param name="id">The subscription plan identifier from the route.</param>
    /// <param name="requestDTO">The editable subscription plan data.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The updated subscription plan response.</returns>
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

    /// <summary>
    /// Soft deletes a subscription plan when it is no longer assigned to an active tenant.
    /// </summary>
    /// <param name="requestDTO">The subscription plan selected for deletion.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The result of the soft-delete operation.</returns>
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
