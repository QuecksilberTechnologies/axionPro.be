using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.LeaveCmd.Queries
{
    public class GetAllLeavePolicyQuery : IRequest<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        public GetLeaveTypeWithPolicyMappingRequestDTO GetAllLeavePolicy { get; set; }

    public GetAllLeavePolicyQuery(GetLeaveTypeWithPolicyMappingRequestDTO getAllLeavePolicy)
    {
        this.GetAllLeavePolicy = getAllLeavePolicy;
    }
}
}
