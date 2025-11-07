using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
 
    public class DeleteLeaveRuleCommandHandler : IRequestHandler<DeleteLeaveRuleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteLeaveRuleCommandHandler> _logger;

        public DeleteLeaveRuleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteLeaveRuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteLeaveRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO.Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid LeaveType Id.",
                        Data = false
                    };
                }

                // Repository se entity fetch karo
                var leaveRule = await _unitOfWork.LeaveRuleRepository.GetLeaveRuleByIdAsync(request.DTO.Id);

                if (leaveRule == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "LeaveRuleType not found.",
                        Data = false
                    };
                }

                // Soft delete
                leaveRule.IsSoftDeleted = true;
                leaveRule.SoftDeleteById = request.DTO.EmployeeId; // Yaha aap userId pass kar sakte ho
                leaveRule.SoftDeleteDateTime = DateTime.UtcNow;
                leaveRule.IsActive = false;
               

                await _unitOfWork.LeaveRuleRepository.DeleteLeaveRuleAsync(leaveRule);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("LeaveType with Id {Id} deleted successfully.", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "LeaveType deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting LeaveType Id {Id}", request.DTO.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while deleting LeaveType: {ex.Message}",
                    Data = false
                };
            }
        }
    }

}
