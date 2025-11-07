using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
