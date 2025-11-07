using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using axionpro.application.DTOs.Tenant;

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    public class GetPlanModuleMappingCommandHandler :IRequestHandler<GetPlanModuleMappingCommand, ApiResponse<PlanModuleMappingResponseDTO>>
    {
        private readonly IPlanModuleMappingRepository _planModuleMappingRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPlanModuleMappingCommandHandler> _logger;


    public GetPlanModuleMappingCommandHandler(IPlanModuleMappingRepository planModuleMappingRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetPlanModuleMappingCommandHandler> logger)
    {
        _planModuleMappingRepository = planModuleMappingRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

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
}
}
