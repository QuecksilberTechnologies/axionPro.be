using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands
{
    public class UpdateLeaveBalanceCommand : IRequest<ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>>
    {

        public UpdateLeaveBalanceToEmployeeRequestDTO DTO { get; set; }

        public UpdateLeaveBalanceCommand(UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceToEmployeeRequest)
        {
            this.DTO = updateLeaveBalanceToEmployeeRequest;
        }
    }
}
