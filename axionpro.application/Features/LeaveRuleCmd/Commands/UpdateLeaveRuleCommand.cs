using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

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