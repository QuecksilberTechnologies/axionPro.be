using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands
{
    public class CreateDayCombinationCommand : IRequest<ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>>
    {

        public CreateDayCombinationRequestDTO DTO { get; set; }

        public CreateDayCombinationCommand(CreateDayCombinationRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
