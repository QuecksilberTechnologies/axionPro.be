using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    public class CreatePolicyLeaveTypeMappingCommandHandler : IRequestHandler<CreatePolicyLeaveTypeMappingCommand, ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILeaveRepository leaveRepository;

        public CreatePolicyLeaveTypeMappingCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILeaveRepository leaveRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            this.leaveRepository = leaveRepository;
           
        }

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
    }
}
