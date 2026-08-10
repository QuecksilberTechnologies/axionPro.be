// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to retrieve all sandwich leave rules.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Queries
{
    #region Query

    /// <summary>
    /// Represents the request to retrieve all sandwich leave rules matching the supplied criteria.
    /// </summary>
    public class GetAllLeaveSandwichRuleIQuery : IRequest<ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        /// <summary>
        /// Gets or sets the criteria used to retrieve sandwich leave rules.
        /// </summary>
        public GetLeaveRuleRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllLeaveSandwichRuleIQuery"/> class.
        /// </summary>
        /// <param name="dTO">The criteria used to retrieve sandwich leave rules.</param>
        public GetAllLeaveSandwichRuleIQuery(GetLeaveRuleRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
    #region Handler

    /// <summary>
    /// Handles retrieval of sandwich leave rules.
    /// </summary>
    public class GetAllLeaveSandwichRuleIQueryHandler : IRequestHandler<GetAllLeaveSandwichRuleIQuery, ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllLeaveSandwichRuleIQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllLeaveSandwichRuleIQueryHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper supplied to this handler.</param>
        /// <param name="unitOfWork">The unit of work used to retrieve sandwich leave rules.</param>
        /// <param name="logger">The logger used to record query results.</param>
        public GetAllLeaveSandwichRuleIQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllLeaveSandwichRuleIQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Retrieves all sandwich leave rules using the supplied query.
        /// </summary>
        /// <param name="request">The query containing the sandwich leave-rule criteria.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response containing the matching sandwich leave rules.</returns>
        public async Task<ApiResponse<List<GetLeaveRuleResponseDTO>>> Handle(GetAllLeaveSandwichRuleIQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Default IsActive to false if null

                // 🔹 Repository se data fetch karo
                var leaveRules = await _unitOfWork.LeaveRuleRepository.GetLeaveRuleByIsSandwichAsync(request.DTO);

                // 🔹 Validation: Agar list null ya empty hai
                if (leaveRules == null || !leaveRules.Any())
                {
                    _logger.LogWarning("⚠️ No LeaveRules found for TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No LeaveRules found.",
                        Data = new List<GetLeaveRuleResponseDTO>()
                    };
                }

                //// 🔹 Map entity -> DTO
                //var leaveRuleDTOs = _mapper.Map<List<GetLeaveRuleResponseDTO>>(leaveRules);

                _logger.LogInformation("✅ Successfully retrieved {Count} LeaveRules for TenantId: {TenantId}", leaveRules.Count, request.DTO.TenantId);

                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "LeaveRules fetched successfully.",
                    Data = leaveRules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching LeaveRules for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching LeaveRules: {ex.Message}",
                    Data = new List<GetLeaveRuleResponseDTO>()
                };
            }
        }

        #endregion
    }

    #endregion
}
