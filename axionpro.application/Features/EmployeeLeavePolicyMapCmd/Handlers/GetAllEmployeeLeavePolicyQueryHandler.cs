// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get All Employee Leave Policy.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Handlers;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Interfaces;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands
{
    #region Query

    /// <summary>
    /// Represents the read-only request to retrieve Get All Employee Leave Policy.
    /// </summary>
public class GetAllEmployeeLeavePolicyQuery : IRequest<ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {
        public GetEmployeeLeavePolicyMappingRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllEmployeeLeavePolicyQuery"/> class.
        /// </summary>

        public GetAllEmployeeLeavePolicyQuery(GetEmployeeLeavePolicyMappingRequestDTO getAllLeavePolicy)
        {
            this.DTO = getAllLeavePolicy;
        }
    }

    #endregion
}

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get All Employee Leave Policy.
    /// </summary>
public class GetAllEmployeeLeavePolicyQueryHandler : IRequestHandler<GetAllEmployeeLeavePolicyQuery, ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllEmployeeLeavePolicyQueryHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllEmployeeLeavePolicyQueryHandler"/> class.
        /// </summary>


        public GetAllEmployeeLeavePolicyQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllEmployeeLeavePolicyQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetAllEmployeeLeavePolicyQuery.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>> Handle(GetAllEmployeeLeavePolicyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetAllLeavePolicyQuery.");
                    return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Please provide valid filter criteria.",
                        Data = new List<GetEmployeeLeavePolicyMappingReponseDTO>()
                    };
                }

                // 🔹 Simplified IsActive flag
                bool isActive;

                if (request.DTO.IsActive is bool activeFlag)
                    isActive = activeFlag;   // agar non-nullable hai ya value mili hai
                else
                    isActive = false;

                // 🔹 Repository call
                var leavePolicies = await _unitOfWork.EmployeeLeaveRepository.GetAllEmployeeLeaveMap(request.DTO);

                if (leavePolicies == null || !leavePolicies.Any())
                {
                    _logger.LogWarning("⚠️ No Leave Policies found in database. IsActive filter = {IsActive}", isActive);

                    return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Leave Policies found for the given filter.",
                        Data = new List<GetEmployeeLeavePolicyMappingReponseDTO>()
                    };
                }

                // 🔹 Mapping
               

                _logger.LogInformation("✅ Successfully retrieved {Count} Leave Policies (IsActive = {IsActive}).", leavePolicies.Count, isActive);

                return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"Successfully fetched {leavePolicies.Count } leave policies.",
                    Data = leavePolicies
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception occurred while fetching Leave Policies.");

                return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching leave policies. Please try again later.",
                    Data = new List<GetEmployeeLeavePolicyMappingReponseDTO>() // Empty list instead of null for consistency
                };
            }
        }

    
        #endregion
}
}
