using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    public class GetAllTenantEnabledModuleOperationByTenantIdCommandHandler : IRequestHandler<GetTenantEnabledModuleCommand, ApiResponse<GetModuleHierarchyResponseDTO>>
    {
        private readonly ITenantModuleConfigurationRepository _tenantModuleConfigurationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTenantOperationsCommandHandler> _logger;

        public GetAllTenantEnabledModuleOperationByTenantIdCommandHandler(
            ITenantModuleConfigurationRepository tenantModuleConfigurationRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllTenantOperationsCommandHandler> logger)
        {
            _tenantModuleConfigurationRepository = tenantModuleConfigurationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetModuleHierarchyResponseDTO>> Handle(GetTenantEnabledModuleCommand request, CancellationToken cancellationToken)
           {
            try
              {
                // ✅ Request null check
                if (request == null)
                {
                    _logger.LogWarning("GetTenantEnabledModuleOperationCommand is null.");
                    return new ApiResponse<GetModuleHierarchyResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Request cannot be null.",
                        Data = null
                    };
                }

                // ✅ TenantId validation
                var TenantId = request.dto.Prop.TenantId;
                if (TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId: {TenantId}", TenantId);
                    return new ApiResponse<GetModuleHierarchyResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "TenantId is required and must be greater than 0.",
                        Data = null
                    };
                }

                // ✅ Get data from repository
                var responseDTO = await _unitOfWork.TenantModuleConfigurationRepository.GetAllTenantEnabledModulesAsync(request.dto);


                return new ApiResponse<GetModuleHierarchyResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Modules and operations fetched successfully.",
                    Data = responseDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching module operations for tenant.");
                return new ApiResponse<GetModuleHierarchyResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching module operations.",
                    Data = null
                };
            }
        }
    }

}

