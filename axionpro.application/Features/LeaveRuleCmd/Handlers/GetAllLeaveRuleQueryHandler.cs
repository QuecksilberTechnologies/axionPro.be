using AutoMapper;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
    public class GetAllLeaveRuleQueryHandler : IRequestHandler<GetAllLeaveRuleQuery, ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllLeaveRuleQueryHandler> _logger;

        public GetAllLeaveRuleQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllLeaveRuleQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetLeaveRuleResponseDTO>>> Handle(GetAllLeaveRuleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Default IsActive to false if null
                

                // 🔹 Repository se data fetch karo
                var leaveRules = await _unitOfWork.LeaveRuleRepository.GetLeaveRuleAsync(request.DTO);

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
    }
}
