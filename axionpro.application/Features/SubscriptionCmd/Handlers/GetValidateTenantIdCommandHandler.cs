using AutoMapper;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    public class GetValidateTenantIdCommandHandler : IRequestHandler<GetValidateTenantIdCommand, ApiResponse<TenantSubscriptionPlanResponseDTO>>
    {
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetValidateTenantIdCommandHandler> _logger;


        public GetValidateTenantIdCommandHandler(ITenantSubscriptionRepository tenantSubscriptionRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetValidateTenantIdCommandHandler> logger)
        {
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<TenantSubscriptionPlanResponseDTO>> Handle(GetValidateTenantIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Validation
                if (request == null)
                {
                    _logger.LogWarning("GetPlanModuleMappingCommand is null.");
                    return new ApiResponse<TenantSubscriptionPlanResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Request cannot be null.",
                        Data = null
                    };
                }

                // ✅ Get all plans
                var subscriptionPlans = await _unitOfWork.TenantSubscriptionRepository
                 .GetValidateTenantPlan(request.dto);

                if (subscriptionPlans == null)
                {
                    return new ApiResponse<TenantSubscriptionPlanResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "No active subscription plan found for the tenant.",
                        Data = null
                    };
                }

                return new ApiResponse<TenantSubscriptionPlanResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Subscription plan fetched successfully.",
                    Data = subscriptionPlans
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching plans.");
                return new ApiResponse<TenantSubscriptionPlanResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching plans.",
                    Data = null
                };
            }
        }
    }
}
