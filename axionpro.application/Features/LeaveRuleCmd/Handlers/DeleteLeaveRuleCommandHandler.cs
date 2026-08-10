// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to delete a leave rule.
// ================================================================

using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to delete a leave rule from the supplied details.
    /// </summary>
    public class DeleteLeaveRuleCommand : IRequest<ApiResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the leave-rule details used to delete the rule.
        /// </summary>
        public DeleteLeaveRuleDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteLeaveRuleCommand"/> class.
        /// </summary>
        /// <param name="dto">The leave-rule details used to delete the rule.</param>
        public DeleteLeaveRuleCommand(DeleteLeaveRuleDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
    #region Handler

    /// <summary>
    /// Handles deletion of leave rules.
    /// </summary>
    public class DeleteLeaveRuleCommandHandler : IRequestHandler<DeleteLeaveRuleCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteLeaveRuleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteLeaveRuleCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work used for leave-rule persistence.</param>
        /// <param name="logger">The logger used to record deletion results.</param>
        public DeleteLeaveRuleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteLeaveRuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Deletes a leave rule using the supplied command.
        /// </summary>
        /// <param name="request">The command containing the leave-rule details.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response indicating whether the leave rule was deleted.</returns>
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

        #endregion
    }

    #endregion
}
