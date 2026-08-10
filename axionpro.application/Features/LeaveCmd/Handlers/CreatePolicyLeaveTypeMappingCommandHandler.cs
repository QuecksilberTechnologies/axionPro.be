// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to create a policy leave-type mapping.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the state-changing request to create a policy leave-type mapping.
    /// </summary>
    public class CreatePolicyLeaveTypeMappingCommand : IRequest<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        /// <summary>
        /// Gets or sets the policy leave-type mapping details to create.
        /// </summary>
        public GetPolicyLeaveTypeMappingRequestDTO createLeavePolicyTypeDTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePolicyLeaveTypeMappingCommand"/> class.
        /// </summary>
        /// <param name="createLeavePolicyTypeDTO">The policy leave-type mapping details to create.</param>
        public CreatePolicyLeaveTypeMappingCommand(GetPolicyLeaveTypeMappingRequestDTO createLeavePolicyTypeDTO)
        {
            this.createLeavePolicyTypeDTO = createLeavePolicyTypeDTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    #region Handler

    /// <summary>
    /// Handles the request to create a policy leave-type mapping.
    /// </summary>
    public class CreatePolicyLeaveTypeMappingCommandHandler : IRequestHandler<CreatePolicyLeaveTypeMappingCommand, ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILeaveRepository leaveRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePolicyLeaveTypeMappingCommandHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper used to convert policy leave-type mapping data.</param>
        /// <param name="unitOfWork">The unit of work used to commit the mapping.</param>
        /// <param name="leaveRepository">The repository used to create the mapping.</param>
        public CreatePolicyLeaveTypeMappingCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILeaveRepository leaveRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            this.leaveRepository = leaveRepository;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Creates a policy leave-type mapping using the supplied command.
        /// </summary>
        /// <param name="request">The command containing the mapping details.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A response containing the created policy leave-type mappings.</returns>
        public async Task<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>> Handle(CreatePolicyLeaveTypeMappingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. DTO → Entity
                var leavePolicy = _mapper.Map<PolicyLeaveTypeMapping>(request.createLeavePolicyTypeDTO);

                // 2. Common fields set karo
                leavePolicy.AddedById = request.createLeavePolicyTypeDTO.EmployeeId;
                leavePolicy.AddedDateTime = DateTime.UtcNow;

                // 3. Repository call
                var leavePolicies = await leaveRepository.CreateLeavePolicyAsync(leavePolicy);

                if (leavePolicies == null || !leavePolicies.Any())
                {
                    return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "❌ Leave Policy creation failed.",
                        Data = new List<GetLeaveTypeWithPolicyMappingResponseDTO>()
                    };
                }

                // 4. Commit transaction
                await _unitOfWork.CommitAsync();

                // 5. Mapping
                var dtoList = _mapper.Map<List<GetLeaveTypeWithPolicyMappingResponseDTO>>(leavePolicies);

                return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Leave Policy created successfully.",
                    Data = dtoList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while creating Leave Policy: {ex.Message}",
                    Data = new List<GetLeaveTypeWithPolicyMappingResponseDTO>()
                };
            }
        }

        #endregion
    }

    #endregion
}
