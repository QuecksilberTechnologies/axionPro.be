using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    public class DeletePolicyLeaveTypeMappingHandler : IRequestHandler<DeleteLeavePolicyCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletePolicyLeaveTypeMappingHandler> _logger;

        public DeletePolicyLeaveTypeMappingHandler(IUnitOfWork unitOfWork, ILogger<DeletePolicyLeaveTypeMappingHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteLeavePolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
              int Id=  Convert.ToInt32(request.DTO.Id);
                if (Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid LeaveType Id.",
                        Data = false
                    };
                }

                // Repository se entity fetch karo
                var leavePolicyType = await _unitOfWork.LeaveRepository.GetLeavePolicyByIdAsync(Id);

                if (leavePolicyType == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "LeaveType not found.",
                        Data = false
                    };
                }

                // Soft delete
                leavePolicyType.IsSoftDeleted = true;
                leavePolicyType.SoftDeleteById = request.DTO.UserId; // Yaha aap userId pass kar sakte ho
                leavePolicyType.SoftDeleteDateTime = DateTime.UtcNow;
                leavePolicyType.IsActive = false;
                leavePolicyType.IsSoftDeleted = true;

                await _unitOfWork.LeaveRepository.DeleteLeavePolicyAsync(leavePolicyType);

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
