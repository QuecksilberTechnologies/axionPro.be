// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve leave policies by employee type.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveCmd.Queries
{
    #region Query

    /// <summary>
    /// Represents the read-only request to retrieve policy leave-type mappings by employee type.
    /// </summary>
    public class GetAllPolicyLeaveTypeByEmpTypeIdQuery : IRequest<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        /// <summary>
        /// Gets or sets the criteria used to retrieve policy leave-type mappings.
        /// </summary>
        public GetPolicyLeaveTypeByEmpTypeIdRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPolicyLeaveTypeByEmpTypeIdQuery"/> class.
        /// </summary>
        /// <param name="dto">The criteria used to retrieve policy leave-type mappings.</param>
        public GetAllPolicyLeaveTypeByEmpTypeIdQuery(GetPolicyLeaveTypeByEmpTypeIdRequestDTO dto)
        {
            this.DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    #region Handler

    /// <summary>
    /// Handles the read-only request to retrieve policy leave-type mappings by employee type.
    /// </summary>
    public class GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler : IRequestHandler<GetAllPolicyLeaveTypeByEmpTypeIdQuery, ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper used to convert policy leave-type mapping data.</param>
        /// <param name="unitOfWork">The unit of work used to retrieve policy leave-type mappings.</param>
        /// <param name="logger">The logger used to record query results.</param>
        public GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Retrieves policy leave-type mappings by employee type using the supplied query.
        /// </summary>
        /// <param name="request">The query containing the policy leave-type mapping criteria.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response containing the matching policy leave-type mappings.</returns>
        public async Task<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>> Handle(GetAllPolicyLeaveTypeByEmpTypeIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetAllLeavePolicyQuery.");
                    return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Please provide valid filter criteria.",
                        Data = new List<GetLeaveTypeWithPolicyMappingResponseDTO>()
                    };
                }

                // 🔹 Simplified IsActive flag
                bool isActive;

                if (request.DTO.IsActive is bool activeFlag)
                    isActive = activeFlag;   // agar non-nullable hai ya value mili hai
                else
                    isActive = false;

                // 🔹 Repository call
                var leavePolicies = await _unitOfWork.LeaveRepository.GetAllLeavePolicyByEmployeeTypeIdAsync(request.DTO);

                if (leavePolicies == null || !leavePolicies.Any())
                {
                    _logger.LogWarning("⚠️ No Leave Policies found in database. IsActive filter = {IsActive}", isActive);

                    return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Leave Policies found for the given filter.",
                        Data = new List<GetLeaveTypeWithPolicyMappingResponseDTO>()
                    };
                }

                // 🔹 Mapping
                var leavePolicyDTOs = _mapper.Map<List<GetLeaveTypeWithPolicyMappingResponseDTO>>(leavePolicies);

                _logger.LogInformation("✅ Successfully retrieved {Count} Leave Policies (IsActive = {IsActive}).", leavePolicyDTOs.Count, isActive);

                return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"Successfully fetched {leavePolicyDTOs.Count} leave policies.",
                    Data = leavePolicyDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception occurred while fetching Leave Policies.");

                return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching leave policies. Please try again later.",
                    Data = new List<GetLeaveTypeWithPolicyMappingResponseDTO>() // Empty list instead of null for consistency
                };
            }
        }

        #endregion
    }

    #endregion
}
