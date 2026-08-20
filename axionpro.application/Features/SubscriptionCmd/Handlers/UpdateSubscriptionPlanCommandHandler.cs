// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Host-authorized updates of subscription plans.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Handlers;

#region Command

/// <summary>
/// Represents the request to update a subscription plan selected by a route identifier.
/// </summary>
public sealed class UpdateSubscriptionPlanCommand : IRequest<ApiResponse<SubscriptionActivePlanDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSubscriptionPlanCommand"/> class.
    /// </summary>
    /// <param name="subscriptionPlanId">The subscription plan identifier from the route.</param>
    /// <param name="requestDTO">The editable subscription plan data.</param>
    public UpdateSubscriptionPlanCommand(
        long subscriptionPlanId,
        UpdateSubscriptionRequestDTO? requestDTO)
    {
        SubscriptionPlanId = subscriptionPlanId;
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the subscription plan identifier from the route.
    /// </summary>
    public long SubscriptionPlanId { get; }

    /// <summary>
    /// Gets the editable subscription plan data.
    /// </summary>
    public UpdateSubscriptionRequestDTO? RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles Host-authorized updates of existing, non-deleted subscription plans.
/// </summary>
public sealed class UpdateSubscriptionPlanCommandHandler
    : IRequestHandler<UpdateSubscriptionPlanCommand, ApiResponse<SubscriptionActivePlanDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSubscriptionPlanCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides subscription plan persistence operations.</param>
    /// <param name="mapper">Maps editable request values onto the loaded entity and maps the response.</param>
    /// <param name="commonRequestService">Validates the current authenticated Host request.</param>
    public UpdateSubscriptionPlanCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Updates an existing subscription plan without allowing client data to alter delete audit fields.
    /// </summary>
    /// <param name="request">The command to process.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The updated subscription plan response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the request or route identifier is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the plan is unavailable or already soft deleted.</exception>
    public async Task<ApiResponse<SubscriptionActivePlanDTO>> Handle(
        UpdateSubscriptionPlanCommand request,
        CancellationToken cancellationToken)
    {
        // Validate the authenticated Host request before processing client-provided data.
        await _commonRequestService.ValidateHostUserRequestAsync();
        cancellationToken.ThrowIfCancellationRequested();

        if (request?.RequestDTO is null ||
            request.SubscriptionPlanId <= 0 ||
            request.SubscriptionPlanId > int.MaxValue)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        // Load the existing non-deleted entity before mapping editable values.
        var subscriptionPlan = await _unitOfWork.SubscriptionRepository
            .GetNonDeletedSubscriptionPlanByIdAsync((int)request.SubscriptionPlanId, cancellationToken);

        if (subscriptionPlan is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.SubscriptionPlanNotFound);
        }

        // AutoMapper ignores the entity identifier and all server-controlled delete audit fields.
        _mapper.Map(request.RequestDTO, subscriptionPlan);

        var updatedSubscriptionPlan = await _unitOfWork.SubscriptionRepository
            .UpdateSubscriptionPlanAsync(subscriptionPlan, cancellationToken);

        // Map the persisted entity to the subscription response model.
        var response = _mapper.Map<SubscriptionActivePlanDTO>(updatedSubscriptionPlan);

        return ApiResponse<SubscriptionActivePlanDTO>.Success(
            response,
            AppConstants.SuccessMessages.SubscriptionPlanUpdatedSuccessfully);
    }

    #endregion
}

#endregion
