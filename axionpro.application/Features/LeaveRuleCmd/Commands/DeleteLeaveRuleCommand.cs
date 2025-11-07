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
    public class DeleteLeaveRuleCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteLeaveRuleDTO DTO { get; set; }

        public DeleteLeaveRuleCommand(DeleteLeaveRuleDTO dto)
        {
            DTO = dto;
        }
    }
}
