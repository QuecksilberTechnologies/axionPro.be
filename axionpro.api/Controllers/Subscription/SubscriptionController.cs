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
    /// Get All Subscription Plan.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all subscription plan.
    /// This endpoint allows anonymous access.
    /// </remarks>
    /// <param name="requestDTO">The query parameters used to get all subscription plan.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Get All Host Subscription Plans.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all host subscription plans.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to get all host subscription plans.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Get Tenant Subscription Plan Info.
    /// </summary>
    /// <remarks>
    /// Handles the request to get tenant subscription plan info.
    /// </remarks>
    /// <param name="requestDTO">The query parameters used to get tenant subscription plan info.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Get All Tenant Accessible Modules.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all tenant accessible modules.
    /// </remarks>
    /// <param name="requestDTO">The query parameters used to get all tenant accessible modules.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Create Subscription.
    /// </summary>
    /// <remarks>
    /// Handles the request to create subscription.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to create subscription.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Update Subscription.
    /// </summary>
    /// <remarks>
    /// Handles the request to update subscription.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="requestDTO">The request body used to update subscription.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Delete Subscription Plan.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete subscription plan.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to delete subscription plan.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
