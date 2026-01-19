using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    public class GetTenantInfoByIdQueryHandler : IRequestHandler<GetTenantInfoByIdQuery, ApiResponse<TenantResponseDTO>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTenantInfoByIdQueryHandler> _logger;

        public GetTenantInfoByIdQueryHandler(
            ITenantRepository tenantRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetTenantInfoByIdQueryHandler> logger)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<TenantResponseDTO>> Handle(GetTenantInfoByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching tenant information for TenantId: {TenantId}", request.TenantId);

                // ✅ Step 1: Repository call
                var tenantEntity = await _tenantRepository.GetByIdAsync(request.TenantId, true);

                // ✅ Step 2: Check null
                if (tenantEntity == null)
                {
                    _logger.LogWarning("No tenant found with TenantId: {TenantId}", request.TenantId);
                    return new ApiResponse<TenantResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = $"No tenant found with TenantId: {request.TenantId}",
                        Data = null
                    };
                }

                // ✅ Step 3: Map entity to DTO
                var tenantDTO = _mapper.Map<TenantResponseDTO>(tenantEntity);

                // ✅ Step 4: Logging and Return
                _logger.LogInformation("Successfully retrieved tenant information for TenantId: {TenantId}", request.TenantId);

                return new ApiResponse<TenantResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Tenant information fetched successfully.",
                    Data = tenantDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching tenant information for TenantId: {TenantId}", request.TenantId);
                return new ApiResponse<TenantResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching tenant information.",
                    Data = null
                };
            }
        }
    }
}
