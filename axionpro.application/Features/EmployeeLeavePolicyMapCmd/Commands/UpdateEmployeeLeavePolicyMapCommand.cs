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
    public class UpdateEmployeeLeavePolicyMapCommand : IRequest<ApiResponse<bool>>
    {

        public UpdateEmployeeLeavePolicyMappingRequestDTO DTO { get; set; }

        public UpdateEmployeeLeavePolicyMapCommand(UpdateEmployeeLeavePolicyMappingRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }
}

