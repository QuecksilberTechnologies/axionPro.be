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
    public class DeletePolicyTypeCommand : IRequest<ApiResponse<bool>>
    {
        public DeletePolicyTypeDTO DTO { get; set; }

        public DeletePolicyTypeCommand(DeletePolicyTypeDTO dTO)
        {
            this.DTO = dTO;
        }

    }
}
