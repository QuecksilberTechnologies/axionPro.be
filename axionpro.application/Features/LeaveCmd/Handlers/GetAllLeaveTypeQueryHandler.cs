using AutoMapper;
using axionpro.application.DTOs.Leave;
 
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Features.RoleCmd.Handlers;
 
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    public class GetAllLeaveRuleQueryHandler  : IRequestHandler<GetAllLeaveTypeQuery, ApiResponse<List<GetLeaveTypResponseDTO>>>
    {
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllLeaveRuleQueryHandler> _logger;

    public GetAllLeaveRuleQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllLeaveRuleQueryHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

        public async Task<ApiResponse<List<GetLeaveTypResponseDTO>>> Handle(GetAllLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                bool?    IsActive = request.DTO.IsActive;
                if (IsActive == null)
                {
                    IsActive=false;
                }
                // 🔹 Repository se data fetch karo
                List<LeaveType> leaveTypes = await _unitOfWork.LeaveRepository.GetAllLeaveAsync(IsActive, request.DTO.TenantId);

                // 🔹 Validation: Agar list null ya empty hai
                if (leaveTypes == null || !leaveTypes.Any())
                {
                    _logger.LogWarning("⚠️ No LeaveTypes found in database.");

                    return new ApiResponse<List<GetLeaveTypResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No LeaveTypes found.",
                        Data = new List<GetLeaveTypResponseDTO>()
                    };
                }

                // 🔹 Map entity -> DTO
                var leaveTypeDTOs = _mapper.Map<List<GetLeaveTypResponseDTO>>(leaveTypes);

                _logger.LogInformation("✅ Successfully retrieved {Count} LeaveTypes.", leaveTypeDTOs.Count);

                return new ApiResponse<List<GetLeaveTypResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "LeaveTypes fetched successfully.",
                    Data = leaveTypeDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching LeaveTypes.");

                return new ApiResponse<List<GetLeaveTypResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching LeaveTypes: {ex.Message}",
                    Data = new List<GetLeaveTypResponseDTO>() // null ke bajaye empty list bhejna better hai
                };
            }
        }


    }
}
    