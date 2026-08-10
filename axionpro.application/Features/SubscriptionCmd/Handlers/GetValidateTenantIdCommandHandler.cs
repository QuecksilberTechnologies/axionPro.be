// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Validate Tenant Id.
// ================================================================

using axionpro.application.DTOs.SubscriptionModule;
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
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.SubscriptionCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Validate Tenant Id.
    /// </summary>
public class GetValidateTenantIdCommand :IRequest<ApiResponse<TenantSubscriptionPlanResponseDTO>>
    {

        public TenantSubscriptionPlanRequestDTO dto { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetValidateTenantIdCommand"/> class.
        /// </summary>

    public GetValidateTenantIdCommand(TenantSubscriptionPlanRequestDTO dto)
    {
        this.dto = dto;
    }

}

    #endregion
}

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get Validate Tenant Id.
    /// </summary>
public class GetValidateTenantIdCommandHandler : IRequestHandler<GetValidateTenantIdCommand, ApiResponse<TenantSubscriptionPlanResponseDTO>>
    {
        #region Fields

        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetValidateTenantIdCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetValidateTenantIdCommandHandler"/> class.
        /// </summary>



        public GetValidateTenantIdCommandHandler(ITenantSubscriptionRepository tenantSubscriptionRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetValidateTenantIdCommandHandler> logger)
        {
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetValidateTenantIdCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


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
    
        #endregion
}
}
