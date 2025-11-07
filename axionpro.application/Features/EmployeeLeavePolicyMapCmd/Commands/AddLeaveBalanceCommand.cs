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
    public class AddLeaveBalanceCommand : IRequest<ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>>
    {

        public AddLeaveBalanceToEmployeeRequestDTO DTO { get; set; }

        public AddLeaveBalanceCommand(AddLeaveBalanceToEmployeeRequestDTO addLeaveBalanceToEmployeeRequest)
        {
            this.DTO = addLeaveBalanceToEmployeeRequest;
        }
    }
}
