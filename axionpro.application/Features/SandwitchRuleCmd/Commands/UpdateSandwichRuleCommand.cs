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
    public class UpdateSandwichRuleCommand : IRequest<ApiResponse<bool>>
    {

        public UpdateLeaveSandwitchRuleRequestDTO DTO { get; set; }

        public UpdateSandwichRuleCommand(UpdateLeaveSandwitchRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
