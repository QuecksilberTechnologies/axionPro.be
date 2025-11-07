using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.LeaveCmd.Commands
{

    public class CreatePolicyLeaveTypeMappingCommand : IRequest<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {

        public GetPolicyLeaveTypeMappingRequestDTO createLeavePolicyTypeDTO { get; set; }

        public CreatePolicyLeaveTypeMappingCommand(GetPolicyLeaveTypeMappingRequestDTO createLeavePolicyTypeDTO)
        {
            this.createLeavePolicyTypeDTO = createLeavePolicyTypeDTO;
        }
    }
     
}
