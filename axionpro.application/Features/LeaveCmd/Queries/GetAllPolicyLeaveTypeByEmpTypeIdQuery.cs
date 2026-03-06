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
    public class GetAllPolicyLeaveTypeByEmpTypeIdQuery : IRequest<ApiResponse<List<GetLeaveTypeWithPolicyMappingResponseDTO>>>
    {
        public GetPolicyLeaveTypeByEmpTypeIdRequestDTO DTO { get; set; }

        public GetAllPolicyLeaveTypeByEmpTypeIdQuery(GetPolicyLeaveTypeByEmpTypeIdRequestDTO dto)
        {
            this.DTO = dto;
        }
    }
}
