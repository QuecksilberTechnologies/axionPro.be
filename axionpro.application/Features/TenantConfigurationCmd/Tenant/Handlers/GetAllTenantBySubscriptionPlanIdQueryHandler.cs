// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get All Tenant By Subscription Plan Id.
// ================================================================

using axionpro.application.DTOs.Operation;
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
using axionpro.application.DTOs.Registration;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries
{
    #region Query

    /// <summary>
    /// Represents the read-only request to retrieve Get All Tenant By Subscription Plan Id.
    /// </summary>
public class GetAllTenantBySubscriptionPlanIdQuery : IRequest<ApiResponse<List<TenantResponseDTO>>>
    {
        public TenantRequestDTO? tenantRequestDTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTenantBySubscriptionPlanIdQuery"/> class.
        /// </summary>

        public GetAllTenantBySubscriptionPlanIdQuery(TenantRequestDTO tenantRequestDTO)
        {
            this.tenantRequestDTO = tenantRequestDTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    /// <summary>
    /// Handles the request to Get All Tenant By Subscription Plan Id.
    /// </summary>
public class GetAllTenantBySubscriptionPlanIdQueryHandler : IRequestHandler<GetAllTenantBySubscriptionPlanIdQuery, ApiResponse<List<TenantResponseDTO>>>
    {
        #region Fields

        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTenantBySubscriptionPlanIdQueryHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTenantBySubscriptionPlanIdQueryHandler"/> class.
        /// </summary>


        public GetAllTenantBySubscriptionPlanIdQueryHandler(
        ITenantRepository tenantRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<GetAllTenantBySubscriptionPlanIdQueryHandler> logger)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetAllTenantBySubscriptionPlanIdQuery.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<TenantResponseDTO>>> Handle(GetAllTenantBySubscriptionPlanIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // ✅ Mapping the DTO to entity
                var tenantDTO = _mapper.Map<axionpro.domain.Entity.Tenant>(request.tenantRequestDTO);

                // ✅ Fetching from DB
                List<axionpro.domain.Entity.Tenant> tenants = await _unitOfWork.TenantRepository.GetAllTenantBySubscriptionIdAsync(tenantDTO);

                // ✅ Mapping to response DTO
                var getAllTenantsDTOs = _mapper.Map<List<TenantResponseDTO>>(tenants);

                // ✅ Condition: if null or empty
                if (getAllTenantsDTOs == null || !getAllTenantsDTOs.Any())
                {
                    _logger.LogWarning("No tenants found.");
                    return new ApiResponse<List<TenantResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No tenants found.",
                        Data = new List<TenantResponseDTO>() // can also return null if needed
                    };
                }

                _logger.LogInformation("Successfully retrieved {Count} Tenants.", getAllTenantsDTOs.Count);

                return new ApiResponse<List<TenantResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Tenants fetched successfully.",
                    Data = getAllTenantsDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Tenants.");
                return new ApiResponse<List<TenantResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Tenants not fetched.",
                    Data = null
                };
            }
        }


    
        #endregion
}
}
