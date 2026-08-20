// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles tenant subscription plan information queries.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Handlers;

#region Query

/// <summary>
/// Represents the request to retrieve subscription information for a tenant.
/// </summary>
public sealed class GetTenantSubscriptionPlanQuery
    : IRequest<ApiResponse<TenantSubscriptionPlanResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantSubscriptionPlanQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The tenant subscription query criteria.</param>
    public GetTenantSubscriptionPlanQuery(TenantSubscriptionPlanRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the tenant subscription query criteria.
    /// </summary>
    public TenantSubscriptionPlanRequestDTO? RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles tenant subscription plan information queries without changing their existing access semantics.
/// </summary>
public sealed class GetTenantSubscriptionPlanQueryHandler
    : IRequestHandler<GetTenantSubscriptionPlanQuery, ApiResponse<TenantSubscriptionPlanResponseDTO>>
{
    #region Fields

    private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantSubscriptionPlanQueryHandler"/> class.
    /// </summary>
    /// <param name="tenantSubscriptionRepository">Provides tenant subscription read operations.</param>
    public GetTenantSubscriptionPlanQueryHandler(
        ITenantSubscriptionRepository tenantSubscriptionRepository)
    {
        _tenantSubscriptionRepository = tenantSubscriptionRepository;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Retrieves the current subscription information for the requested tenant.
    /// </summary>
    /// <param name="request">The query to process.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The tenant subscription plan response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when tenant criteria are invalid.</exception>
    public async Task<ApiResponse<TenantSubscriptionPlanResponseDTO>> Handle(
        GetTenantSubscriptionPlanQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request?.RequestDTO is null || request.RequestDTO.TenantId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var subscriptionPlan = await _tenantSubscriptionRepository
            .GetValidateTenantPlan(request.RequestDTO);

        return ApiResponse<TenantSubscriptionPlanResponseDTO>.Success(
            subscriptionPlan,
            AppConstants.SuccessMessages.TenantSubscriptionPlanRetrievedSuccessfully);
    }

    #endregion
}

#endregion
