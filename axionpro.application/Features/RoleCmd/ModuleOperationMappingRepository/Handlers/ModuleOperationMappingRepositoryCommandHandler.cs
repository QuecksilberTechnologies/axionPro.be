using AutoMapper;
using AutoMapper.Execution;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Commands;
using axionpro.application.Features.TenantCmd.Commands;
using axionpro.application.Features.TenantCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Handlers
{
    public class ModuleOperationMappingRepositoryCommandHandler : IRequestHandler<GetModuleOperationMappingRepositoryCommand, ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>>
    {
        private readonly ITenantModuleConfigurationRepository repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<GetModuleOperationMappingRepositoryCommand> _logger;

        public ModuleOperationMappingRepositoryCommandHandler(
            ITenantModuleConfigurationRepository tenantModuleConfigurationRepository,
            IMapper mapper,
            ICommonRepository commonRepository,
            IUnitOfWork unitOfWork,
            ILogger<GetModuleOperationMappingRepositoryCommand> logger)
        {
            repository = tenantModuleConfigurationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonRepository = commonRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>> Handle(
            GetModuleOperationMappingRepositoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("GetModuleOperationConfigurationByTenantIdCommand request is null.");
                    return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Request cannot be null.",
                        Data = null
                    };
                }

                var tenantId = request.dto.TenantId;
                if (tenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId received: {TenantId}", tenantId);
                    return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "TenantId is required and must be greater than 0.",
                        Data = null
                    };
                }
               
                var subscriptionInfo = await _unitOfWork.TenantSubscriptionRepository.GetValidateTenantPlan(new TenantSubscriptionPlanRequestDTO()
                {
                    TenantId = request.dto.TenantId,
                    IsActive = true
                });

                // ✅ Check if subscription info exists and EndDate > today
                if (subscriptionInfo == null ||
                             !subscriptionInfo.SubscriptionEndDate.HasValue ||
                             subscriptionInfo.SubscriptionEndDate.Value.Date < DateTime.Today)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogWarning("Subscription expired or missing for tenant {TenantId}", request.dto.TenantId);

                    return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Your subscription has expired. Please contact admin to renew the plan."
                    };
                }
                var PlanInfo = await _unitOfWork.SubscriptionRepository.GetPlanByIdAsync(subscriptionInfo.SubscriptionPlanId);
                if (PlanInfo == null)   
                {
                    return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Your no valid plan exist!"
                    };
                }
                request.dto.SubscriptionPlanId = subscriptionInfo.SubscriptionPlanId;
                var moduleOperationRolePermissionsResponseDTOs = await 
                    _unitOfWork.ModuleOperationMappingRepository.GetTenantModulesOperationRolePermissionResponses(request.dto);

                if(moduleOperationRolePermissionsResponseDTOs == null || moduleOperationRolePermissionsResponseDTOs.Count == 0)
                {
                    return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No module operations found for the tenant.",
                        Data = new List<GetModuleOperationRolePermissionsResponseDTO>()
                    };
                }   

                return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                   {
                    IsSucceeded = true,
                    Message = "Modules and operations fetched successfully.",
                    Data = moduleOperationRolePermissionsResponseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching module operations for tenant.");
                return new ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching module operations.",
                    Data = null
                };
            }
        }
    }
}