// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Tenant Info By Id.
// ================================================================

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
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries
{
    #region Query

    /// <summary>
    /// Represents the read-only request to retrieve Get Tenant Info By Id.
    /// </summary>
public class GetTenantInfoByIdQuery : IRequest<ApiResponse<TenantResponseDTO>>
    {
        public long TenantId  { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTenantInfoByIdQuery"/> class.
        /// </summary>

        public GetTenantInfoByIdQuery(long TenantId)
        {
            this.TenantId = TenantId;
        }
    }

    #endregion
}

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    /// <summary>
    /// Handles the request to Get Tenant Info By Id.
    /// </summary>
public class GetTenantInfoByIdQueryHandler : IRequestHandler<GetTenantInfoByIdQuery, ApiResponse<TenantResponseDTO>>
    {
        #region Fields

        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTenantInfoByIdQueryHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTenantInfoByIdQueryHandler"/> class.
        /// </summary>


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
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetTenantInfoByIdQuery.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


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
    
        #endregion
}
}
