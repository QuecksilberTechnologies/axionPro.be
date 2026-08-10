// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to update a leave rule.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update a leave rule from the supplied details.
    /// </summary>
    public class UpdateLeaveRuleCommand : IRequest<ApiResponse<GetLeaveRuleResponseDTO>>
    {
        /// <summary>
        /// Gets or sets the leave-rule details to update.
        /// </summary>
        public UpdateLeaveRuleRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLeaveRuleCommand"/> class.
        /// </summary>
        /// <param name="dto">The leave-rule details used to update the rule.</param>
        public UpdateLeaveRuleCommand(UpdateLeaveRuleRequestDTO dto)
        {
            this.DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
    #region Handler

    /// <summary>
    /// Handles updates to leave rules.
    /// </summary>
    public class UpdateLeaveRuleCommandHandler : IRequestHandler<UpdateLeaveRuleCommand, ApiResponse<GetLeaveRuleResponseDTO>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLeaveRuleCommandHandler"/> class.
        /// </summary>
        /// <param name="leaveRepository">The leave repository dependency supplied to this handler.</param>
        /// <param name="mapper">The mapper used to convert leave-rule details to an entity.</param>
        /// <param name="unitOfWork">The unit of work used for leave-rule persistence.</param>
        public UpdateLeaveRuleCommandHandler(ILeaveRepository leaveRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
           ;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Updates a leave rule using the supplied command.
        /// </summary>
        /// <param name="request">The command containing the leave-rule details.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response containing the updated leave-rule details.</returns>
        public async Task<ApiResponse<GetLeaveRuleResponseDTO>> Handle(UpdateLeaveRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                LeaveRule leaveRuleEntity = _mapper.Map<LeaveRule>(request.DTO);
                long userId = request.DTO.EmployeeId;

                LeaveRule updatedEntity = await _unitOfWork.LeaveRuleRepository.UpdateLeaveRuleAsync(leaveRuleEntity, userId);

                if (updatedEntity == null)
                {
                    return new ApiResponse<GetLeaveRuleResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "No LeaveRule was updated.",
                        Data = null
                    };
                }

                GetLeaveRuleResponseDTO leaveRuleDTO = _mapper.Map<GetLeaveRuleResponseDTO>(updatedEntity);

                return new ApiResponse<GetLeaveRuleResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "LeaveRule updated successfully.",
                    Data = leaveRuleDTO
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetLeaveRuleResponseDTO>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion
    }

    #endregion
}
