using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.LeaveRuleCmd.Queries
{
    public class GetAllLeaveRuleQuery : IRequest<ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        public GetLeaveRuleRequestDTO DTO { get; set; }

    public GetAllLeaveRuleQuery(GetLeaveRuleRequestDTO dTO)
    {
        this.DTO = dTO;
    }
}
}
