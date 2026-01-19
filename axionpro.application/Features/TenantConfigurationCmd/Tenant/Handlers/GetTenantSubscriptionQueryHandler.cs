using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    public class GetTenantSubscriptionQueryHandler
        : IRequestHandler<GetTenantSubscriptionQuery, ApiResponse<List<TenantSubscriptionPlanResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTenantSubscriptionQueryHandler> _logger;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;

        public GetTenantSubscriptionQueryHandler(
            ITenantSubscriptionRepository tenantSubscriptionRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetTenantSubscriptionQueryHandler> logger)
        {
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<TenantSubscriptionPlanResponseDTO>>> Handle(GetTenantSubscriptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO == null)
                {
                    _logger.LogWarning("GetTenantSubscriptionQuery DTO is null.");
                    return new ApiResponse<List<TenantSubscriptionPlanResponseDTO>>()
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                _logger.LogInformation("Fetching tenant subscription plan info for TenantId: {TenantId}", request.DTO.TenantId);

                // ✅ Repository call (returns list)
                List <TenantSubscriptionPlanResponseDTO> responseDTOs = await _tenantSubscriptionRepository.GetTenantSubscriptionPlanInfoAsync(request.DTO);

                if (responseDTOs == null || responseDTOs.Count == 0)
                {
                    _logger.LogWarning("No tenant subscription plan found for TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<List<TenantSubscriptionPlanResponseDTO>>()
                    {
                        IsSucceeded = false,
                        Message = "No tenant subscription plan found.",
                        Data = null
                    };
                }

             
                _logger.LogInformation("Successfully retrieved {Count} tenant subscription records for TenantId: {TenantId}",
                    responseDTOs.Count, request.DTO.TenantId);

                return new ApiResponse<List<TenantSubscriptionPlanResponseDTO>>()
                {
                    IsSucceeded = true,
                    Message = "Tenant subscription plan(s) fetched successfully.",
                    Data = responseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching tenant subscription plan.");
                return new ApiResponse<List<TenantSubscriptionPlanResponseDTO>>()
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching tenant subscription plan.",
                    Data = null
                };
            }
        }
    }
}
