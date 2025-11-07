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
    public class EmployeeLeavePolicyMapCommand : IRequest<ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {

        public CreateEmployeeLeavePolicyMappingRequestDTO DTO { get; set; }

        public EmployeeLeavePolicyMapCommand(CreateEmployeeLeavePolicyMappingRequestDTO createLeavePolicyTypeDTO)
        {
            DTO = createLeavePolicyTypeDTO;
        }
    }
}