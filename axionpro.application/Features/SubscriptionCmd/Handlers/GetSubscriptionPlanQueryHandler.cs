// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles subscription-plan queries filtered by active status.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Handlers;

#region Query

/// <summary>
/// Represents the request to retrieve subscription plans for a requested active status.
/// </summary>
public sealed class GetSubscriptionPlanQuery : IRequest<ApiResponse<List<SubscriptionActivePlanDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSubscriptionPlanQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The subscription plan filter request.</param>
    public GetSubscriptionPlanQuery(SubscriptionPlanRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the subscription plan filter request.
    /// </summary>
    public SubscriptionPlanRequestDTO? RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles Host-authorized subscription plan queries.
/// </summary>
public sealed class GetSubscriptionPlanQueryHandler
    : IRequestHandler<GetSubscriptionPlanQuery, ApiResponse<List<SubscriptionActivePlanDTO>>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSubscriptionPlanQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides subscription plan query operations.</param>
    /// <param name="commonRequestService">Validates the current authenticated Host request.</param>
    public GetSubscriptionPlanQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Retrieves non-deleted subscription plans filtered by the requested active status.
    /// </summary>
    /// <param name="request">The query to process.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The filtered subscription plan response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the query request is missing.</exception>
    public async Task<ApiResponse<List<SubscriptionActivePlanDTO>>> Handle(
        GetSubscriptionPlanQuery request,
        CancellationToken cancellationToken)
    {
        // Validate the authenticated Host request before reading plan data.
        await _commonRequestService.ValidateHostUserRequestAsync();
        cancellationToken.ThrowIfCancellationRequested();

        if (request?.RequestDTO is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var plans = await _unitOfWork.SubscriptionRepository
            .GetAllPlansAsync(request.RequestDTO.IsActive, cancellationToken);

        // Build the application response in the handler layer.
        return ApiResponse<List<SubscriptionActivePlanDTO>>.Success(
            plans,
            AppConstants.SuccessMessages.SubscriptionPlansRetrievedSuccessfully);
    }

    #endregion
}

#endregion
