using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveCmd.Commands
{
    public class DeleteLeavePolicyCommand : IRequest<ApiResponse<bool>>
    {
        public DeletePolicyLeaveTypeMappingRequestDTO DTO { get; set; }

        public DeleteLeavePolicyCommand(DeletePolicyLeaveTypeMappingRequestDTO deleteLeaveRequestDTO)
        {
            this.DTO = deleteLeaveRequestDTO;
        }
    }
}
