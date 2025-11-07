using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.Commands
{
    public class DeleteSandwichRuleCommand : IRequest<ApiResponse<bool>>
    {

        public DeleteLeaveSandwitchRuleRequestDTO DTO { get; set; }

        public DeleteSandwichRuleCommand(DeleteLeaveSandwitchRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
