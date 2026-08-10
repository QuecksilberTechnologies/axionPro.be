// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Plan Module Mapping.
// ================================================================

using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.SubscriptionCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Plan Module Mapping.
    /// </summary>
public class GetPlanModuleMappingCommand : IRequest<ApiResponse<PlanModuleMappingResponseDTO>>
    {

        public PlanModuleMappingRequestDTO planModuleMappingRequest { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPlanModuleMappingCommand"/> class.
        /// </summary>

        public GetPlanModuleMappingCommand(PlanModuleMappingRequestDTO planModuleMappingRequest)
        {
            this.planModuleMappingRequest = planModuleMappingRequest;
        }

    }

    #endregion
}

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get Plan Module Mapping.
    /// </summary>
public class GetPlanModuleMappingCommandHandler :IRequestHandler<GetPlanModuleMappingCommand, ApiResponse<PlanModuleMappingResponseDTO>>
    {
        #region Fields

        private readonly IPlanModuleMappingRepository _planModuleMappingRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPlanModuleMappingCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPlanModuleMappingCommandHandler"/> class.
        /// </summary>



    public GetPlanModuleMappingCommandHandler(IPlanModuleMappingRepository planModuleMappingRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetPlanModuleMappingCommandHandler> logger)
    {
        _planModuleMappingRepository = planModuleMappingRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetPlanModuleMappingCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


    public async Task<ApiResponse<PlanModuleMappingResponseDTO>> Handle(GetPlanModuleMappingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ Validation
            if (request == null)
            {
                _logger.LogWarning("GetPlanModuleMappingCommand is null.");
                return new ApiResponse<PlanModuleMappingResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Request cannot be null.",
                    Data = null
                };
            }

            if (request.planModuleMappingRequest.TenantId == 0 || request.planModuleMappingRequest.TenantId <= 0)
            {
                _logger.LogWarning("Invalid TenantId: {TenantId}", request.planModuleMappingRequest.TenantId);
                    return new ApiResponse<PlanModuleMappingResponseDTO>
                    {
                    IsSucceeded = false,
                    Message = "TenantId is required and must be greater than 0.",
                    Data = null
                };
            }

            //   var subscriptions = _mapper.Map<SubscriptionPlan>(request.subscriptionPlanRequestDTO);

            // ✅ Get all plans
            PlanModuleMappingResponseDTO subscriptionPlans = await _unitOfWork.PlanModuleMappingRepository.GetModulesBySubscriptionPlanIdAsync(request.planModuleMappingRequest.SubscriptionPlanId);


            // List<SubscriptionPlanResponseDTO> mappedPlans = _mapper.Map < List < SubscriptionPlanResponseDTO >> (subscriptionPlans);
            var mappedPlans = _mapper.Map<PlanModuleMappingResponseDTO>(subscriptionPlans);


                return new ApiResponse<PlanModuleMappingResponseDTO>
                {
                IsSucceeded = true,
                Message = "Plans fetched successfully.",
                Data = mappedPlans
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching plans.");
            return new ApiResponse<PlanModuleMappingResponseDTO>
            {
                IsSucceeded = false,
                Message = "Something went wrong while fetching plans.",
                Data = null
            };
        }
    }

        #endregion
}
}
