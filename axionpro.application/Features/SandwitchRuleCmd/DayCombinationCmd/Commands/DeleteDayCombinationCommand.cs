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
    public class DeleteDayCombinationCommand : IRequest<ApiResponse<bool>>
    {

        public DeleteDayCombinationRequestDTO DTO { get; set; }

        public DeleteDayCombinationCommand(DeleteDayCombinationRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
