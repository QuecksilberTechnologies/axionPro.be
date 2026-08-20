// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Host-authorized soft deletion of subscription plans.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Handlers;

#region Command

/// <summary>
/// Represents the request to soft delete a subscription plan.
/// </summary>
public class DeleteSubscriptionPlanCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSubscriptionPlanCommand"/> class.
    /// </summary>
    /// <param name="request">The subscription plan deletion request.</param>
    public DeleteSubscriptionPlanCommand(DeleteSubscriptionPlanRequestDTO request)
    {
        Request = request;
    }

    /// <summary>
    /// Gets the subscription plan deletion request.
    /// </summary>
    public DeleteSubscriptionPlanRequestDTO Request { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles Host-authorized soft deletion of subscription plans.
/// </summary>
public class DeleteSubscriptionPlanCommandHandler
    : IRequestHandler<DeleteSubscriptionPlanCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSubscriptionPlanCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides subscription plan persistence operations.</param>
    /// <param name="commonRequestService">Validates the current authenticated Host user.</param>
    public DeleteSubscriptionPlanCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Soft deletes an unused subscription plan and records the authenticated Host user as the actor.
    /// </summary>
    /// <param name="request">The delete command to process.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A successful response when the plan is soft deleted.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the command contains an invalid identifier.</exception>
    /// <exception cref="NotFoundException">Thrown when the requested plan is unavailable or already deleted.</exception>
    /// <exception cref="ConflictException">Thrown when an active tenant still uses the plan.</exception>
    public async Task<ApiResponse<bool>> Handle(
        DeleteSubscriptionPlanCommand request,
        CancellationToken cancellationToken)
    {
        // Validate the authenticated Host user before processing client-provided data.
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();
        cancellationToken.ThrowIfCancellationRequested();

        if (request?.Request is null || request.Request.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        if (hostUserId <= 0 || hostUserId > int.MaxValue)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        // Load the active subscription plan targeted for deletion.
        var subscriptionPlan = await _unitOfWork.SubscriptionRepository
            .GetNonDeletedSubscriptionPlanByIdAsync(request.Request.Id, cancellationToken);

        if (subscriptionPlan is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.SubscriptionPlanNotFound);
        }

        // Prevent deletion while the plan is assigned to an active, legitimate tenant.
        var isAssignedToTenant = await _unitOfWork.SubscriptionRepository
            .IsSubscriptionPlanAssignedToAnyActiveTenantAsync(subscriptionPlan.Id, cancellationToken);

        if (isAssignedToTenant)
        {
            throw new ConflictException(AppConstants.ErrorMessages.SubscriptionPlanInUse);
        }

        // Apply server-controlled soft-delete audit values.
        subscriptionPlan.IsSoftDeleted = true;
        subscriptionPlan.DeletedById = (int)hostUserId;
        subscriptionPlan.DeletedDateTime = DateTime.UtcNow;

        // Persist the updated subscription plan entity without physically deleting it.
        await _unitOfWork.SubscriptionRepository
            .SoftDeleteSubscriptionPlanAsync(subscriptionPlan, cancellationToken);

        return ApiResponse<bool>.Success(
            true,
            AppConstants.SuccessMessages.SubscriptionPlanDeletedSuccessfully);
    }

    #endregion
}

#endregion
