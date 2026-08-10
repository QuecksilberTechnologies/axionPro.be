// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to create a leave rule.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to create a leave rule from the supplied details.
    /// </summary>
    public class CreateLeaveRuleCommand : IRequest<ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        /// <summary>
        /// Gets or sets the leave-rule details to create.
        /// </summary>
        public CreateLeaveRuleDTORequest DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLeaveRuleCommand"/> class.
        /// </summary>
        /// <param name="dto">The leave-rule details used to create the rule.</param>
        public CreateLeaveRuleCommand(CreateLeaveRuleDTORequest dto)
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
    /// Handles creation of leave rules.
    /// </summary>
    public class CreateLeaveRuleCommandHandler : IRequestHandler<CreateLeaveRuleCommand, ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLeaveRuleCommandHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper used to convert leave-rule details to an entity.</param>
        /// <param name="unitOfWork">The unit of work used for leave-rule persistence.</param>
        public CreateLeaveRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Creates a leave rule using the supplied command.
        /// </summary>
        /// <param name="request">The command containing the leave-rule details.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response containing the created leave rules.</returns>
        public async Task<ApiResponse<List<GetLeaveRuleResponseDTO>>> Handle(CreateLeaveRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. DTO → Entity
                var leaveRulePolicy = _mapper.Map<LeaveRule>(request.DTO);

                // 2. Common fields set karo
                leaveRulePolicy.AddedById = request.DTO.EmployeeId;
                leaveRulePolicy.AddedDateTime = DateTime.UtcNow;

                // 3. Repository call
                var leaveRulePolicies = await _unitOfWork.LeaveRuleRepository.CreateLeaveRuleAsync(leaveRulePolicy);

                if (leaveRulePolicies == null || !leaveRulePolicies.Any())
                {
                    return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "❌ Leave Policy creation failed.",
                        Data = new List<GetLeaveRuleResponseDTO>()
                    };
                }

                // 4. Commit transaction
                await _unitOfWork.CommitAsync();

                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Leave Policy created successfully.",
                    Data = leaveRulePolicies
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while creating Leave Policy: {ex.Message}",
                    Data = new List<GetLeaveRuleResponseDTO>()
                };
            }
        }

        #endregion
    }

    #endregion
}
