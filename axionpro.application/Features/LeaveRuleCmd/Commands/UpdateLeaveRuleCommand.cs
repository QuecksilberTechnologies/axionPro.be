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
    public class UpdateLeaveRuleCommand : IRequest<ApiResponse<GetLeaveRuleResponseDTO>>
    {

        public UpdateLeaveRuleRequestDTO DTO { get; set; }

        public UpdateLeaveRuleCommand(UpdateLeaveRuleRequestDTO dto)
        {
            this.DTO = dto;
        }

    }

}