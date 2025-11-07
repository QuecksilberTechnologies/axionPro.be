using AutoMapper;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Features.SubscriptionCmd.Handlers;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantCmd.Commands;

namespace axionpro.application.Features.TenantCmd.Handlers
{

    public class GetAllTenantTrueEnabledModuleOperationByTenantIdCommandHandler : IRequestHandler<GetAllTenantTrueEnabledModuleOperationByTenantIdCommand, ApiResponse<TenantEnabledModuleOperationsResponseDTO>>
    {
        private readonly ITenantModuleConfigurationRepository _tenantModuleConfigurationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTenantTrueEnabledModuleOperationByTenantIdCommandHandler> _logger;

        public GetAllTenantTrueEnabledModuleOperationByTenantIdCommandHandler(
            ITenantModuleConfigurationRepository tenantModuleConfigurationRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllTenantTrueEnabledModuleOperationByTenantIdCommandHandler> logger)
        {
            _tenantModuleConfigurationRepository = tenantModuleConfigurationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<TenantEnabledModuleOperationsResponseDTO>> Handle(
            GetAllTenantTrueEnabledModuleOperationByTenantIdCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Request null check
                if (request == null)
                {
                    _logger.LogWarning("GetTenantEnabledModuleOperationCommand is null.");
                    return new ApiResponse<TenantEnabledModuleOperationsResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Request cannot be null.",
                        Data = null
                    };
                }

                // ✅ TenantId validation
                var TenantId = request.TenantEnabledModuleOperationsRequestDTO.TenantId;
                if (TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId: {TenantId}", TenantId);
                    return new ApiResponse<TenantEnabledModuleOperationsResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "TenantId is required and must be greater than 0.",
                        Data = null
                    };
                }

                // ✅ Get data from repository
                var responseDTO = await _unitOfWork.TenantModuleConfigurationRepository.GetEnabledModuleWithOperation(request.TenantEnabledModuleOperationsRequestDTO);


                return new ApiResponse<TenantEnabledModuleOperationsResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Modules and operations fetched successfully.",
                    Data = responseDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching module operations for tenant.");
                return new ApiResponse<TenantEnabledModuleOperationsResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching module operations.",
                    Data = null
                };
            }
        }
    }

}

