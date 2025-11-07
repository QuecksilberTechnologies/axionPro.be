using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    public class GetSubscriptionPlanCommandHandler : IRequestHandler<GetSubscriptionPlanCommand, ApiResponse<List<SubscriptionActivePlanDTO>>>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetSubscriptionPlanCommandHandler> _logger;

      
        public GetSubscriptionPlanCommandHandler(ISubscriptionRepository subscriptionRepository, IMapper mapper, IUnitOfWork unitOfWork  , ILogger<GetSubscriptionPlanCommandHandler> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<SubscriptionActivePlanDTO>>> Handle(GetSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Validation
                if (request == null)
                {
                    _logger.LogWarning("SubscriptionPlanRequestDTO is null.");
                    return new ApiResponse<List<SubscriptionActivePlanDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Request cannot be null.",
                        Data = null
                    };
                }

                if (request.subscriptionPlanRequestDTO.TenantId == 0 || request.subscriptionPlanRequestDTO.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId: {TenantId}", request.subscriptionPlanRequestDTO.TenantId);
                    return new ApiResponse<List<SubscriptionActivePlanDTO>>
                    {
                        IsSucceeded = false,
                        Message = "TenantId is required and must be greater than 0.",
                        Data = null
                    };
                }

             //   var subscriptions = _mapper.Map<SubscriptionPlan>(request.subscriptionPlanRequestDTO);

                // ✅ Get all plans
                List<SubscriptionActivePlanDTO> subscriptionPlans = await _unitOfWork.SubscriptionRepository.GetAllPlansAsync();

               // _unitOfWork.PlanModuleMappingRepository.GetModulesBySubscriptionPlanIdAsync


              // List<SubscriptionPlanResponseDTO> mappedPlans = _mapper.Map < List < SubscriptionPlanResponseDTO >> (subscriptionPlans);
              var mappedPlans = _mapper.Map<List<SubscriptionActivePlanDTO>>(subscriptionPlans);


                return new ApiResponse<List<SubscriptionActivePlanDTO>>
                {
                    IsSucceeded = true,
                    Message = "Plans fetched successfully.",
                    Data = mappedPlans
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching plans.");
                return new ApiResponse<List<SubscriptionActivePlanDTO>>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching plans.",
                    Data = null
                };
            }
        }
    }
}
