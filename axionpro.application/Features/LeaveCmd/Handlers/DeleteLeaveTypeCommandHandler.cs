using axionpro.application.Constants;
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
    public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteLeaveTypeCommandHandler> _logger;

        public DeleteLeaveTypeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteLeaveTypeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
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
                var leaveType = await _unitOfWork.LeaveRepository.GetLeaveByIdAsync(request.DTO.Id);

                if (leaveType == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "LeaveType not found.",
                        Data = false
                    };
                }

                // Soft delete
                leaveType.IsSoftDeleted = ConstantValues.IsByDefaultTrue;
                leaveType.SoftDeletedBy = request.DTO.EmployeeId; // Yaha aap userId pass kar sakte ho
                leaveType.SoftDeletedDateTime = DateTime.UtcNow;
                leaveType.IsActive = ConstantValues.IsByDefaultFalse; ;
                leaveType.IsSoftDeleted = ConstantValues.IsByDefaultTrue; ;

                await _unitOfWork.LeaveRepository.DeleteLeaveAsync(leaveType);

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
