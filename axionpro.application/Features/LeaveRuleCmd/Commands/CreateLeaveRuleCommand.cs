using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Commands
{
    public class CreateLeaveRuleCommand : IRequest<ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {

        public CreateLeaveRuleDTORequest DTO { get; set; }

        public CreateLeaveRuleCommand(CreateLeaveRuleDTORequest dto)
        {
            this.DTO = dto;
        }
    }
}
