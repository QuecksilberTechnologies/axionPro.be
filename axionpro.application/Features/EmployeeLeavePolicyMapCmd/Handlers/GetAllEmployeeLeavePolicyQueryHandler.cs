using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Handlers;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Handlers
{
    public class GetAllEmployeeLeavePolicyQueryHandler : IRequestHandler<GetAllEmployeeLeavePolicyQuery, ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllEmployeeLeavePolicyQueryHandler> _logger;

        public GetAllEmployeeLeavePolicyQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllEmployeeLeavePolicyQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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

    }
}