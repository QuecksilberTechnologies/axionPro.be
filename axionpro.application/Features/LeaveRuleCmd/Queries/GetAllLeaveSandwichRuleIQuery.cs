using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.LeaveRuleCmd.Queries
{
    public class GetAllLeaveSandwichRuleIQuery : IRequest<ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        public GetLeaveRuleRequestDTO DTO { get; set; }

    public GetAllLeaveSandwichRuleIQuery(GetLeaveRuleRequestDTO dTO)
    {
        this.DTO = dTO;
    }
}
}
