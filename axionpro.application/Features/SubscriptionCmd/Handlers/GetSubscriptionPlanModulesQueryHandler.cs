// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles queries for modules available to a subscription plan.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Handlers;

#region Query

/// <summary>
/// Represents the request to retrieve modules available to a tenant subscription plan.
/// </summary>
public sealed class GetSubscriptionPlanModulesQuery
    : IRequest<ApiResponse<PlanModuleMappingResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSubscriptionPlanModulesQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The tenant and subscription plan criteria.</param>
    public GetSubscriptionPlanModulesQuery(PlanModuleMappingRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the tenant and subscription plan criteria.
    /// </summary>
    public PlanModuleMappingRequestDTO? RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles module queries for non-deleted subscription plans without changing tenant-facing access semantics.
/// </summary>
public sealed class GetSubscriptionPlanModulesQueryHandler
    : IRequestHandler<GetSubscriptionPlanModulesQuery, ApiResponse<PlanModuleMappingResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSubscriptionPlanModulesQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides subscription plan and module mapping query operations.</param>
    public GetSubscriptionPlanModulesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Retrieves modules for a tenant-selected, non-deleted subscription plan.
    /// </summary>
    /// <param name="request">The query to process.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The subscription plan module response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when tenant or plan criteria are invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the selected plan is unavailable or soft deleted.</exception>
    public async Task<ApiResponse<PlanModuleMappingResponseDTO>> Handle(
        GetSubscriptionPlanModulesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request?.RequestDTO is null ||
            request.RequestDTO.TenantId is null or <= 0 ||
            request.RequestDTO.SubscriptionPlanId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        // Prevent normal module queries from exposing modules for a soft-deleted plan.
        var subscriptionPlan = await _unitOfWork.SubscriptionRepository
            .GetNonDeletedSubscriptionPlanByIdAsync(
                request.RequestDTO.SubscriptionPlanId,
                cancellationToken);

        if (subscriptionPlan is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.SubscriptionPlanNotFound);
        }

        var modules = await _unitOfWork.PlanModuleMappingRepository
            .GetModulesBySubscriptionPlanIdAsync(request.RequestDTO.SubscriptionPlanId);

        return ApiResponse<PlanModuleMappingResponseDTO>.Success(
            modules,
            AppConstants.SuccessMessages.SubscriptionPlanModulesRetrievedSuccessfully);
    }

    #endregion
}

#endregion
