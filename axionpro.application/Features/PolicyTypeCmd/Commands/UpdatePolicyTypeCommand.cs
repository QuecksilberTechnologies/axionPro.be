using axionpro.application.DTOs.PolicyType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.PolicyTypeCmd.Commands
{
    public class UpdatePolicyTypeCommand : IRequest<ApiResponse<bool>>
    {
        public UpdatePolicyTypeDTO DTO { get; set; }

        public UpdatePolicyTypeCommand(UpdatePolicyTypeDTO dTO)
        {
            this.DTO = dTO;
        }

    }
}
