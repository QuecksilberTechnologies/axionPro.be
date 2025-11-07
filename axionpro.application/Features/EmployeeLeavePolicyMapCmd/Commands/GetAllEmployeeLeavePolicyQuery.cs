using axionpro.application.DTOs.Leave;
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
    public class GetAllEmployeeLeavePolicyQuery : IRequest<ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {
        public GetEmployeeLeavePolicyMappingRequestDTO DTO { get; set; }

        public GetAllEmployeeLeavePolicyQuery(GetEmployeeLeavePolicyMappingRequestDTO getAllLeavePolicy)
        {
            this.DTO = getAllLeavePolicy;
        }
    }
}
