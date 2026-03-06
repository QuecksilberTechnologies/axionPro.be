using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.SandwitchRuleCmd.Commands
{
    public class GetSandwichRuleCommand : IRequest<ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>>
    {

        public GetLeaveSandwitchRuleRequestDTO DTO { get; set; }

        public GetSandwichRuleCommand(GetLeaveSandwitchRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
