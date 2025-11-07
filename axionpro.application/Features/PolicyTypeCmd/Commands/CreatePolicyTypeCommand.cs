using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOs.Role;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.PolicyTypeCmd.Commands
{

    public class CreatePolicyTypeCommand : IRequest<ApiResponse<List<GetPolicyTypeResponseDTO>>>
    {
        public CreatePolicyTypeRequestDTO DTO { get; set; }

        public CreatePolicyTypeCommand(CreatePolicyTypeRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
}
