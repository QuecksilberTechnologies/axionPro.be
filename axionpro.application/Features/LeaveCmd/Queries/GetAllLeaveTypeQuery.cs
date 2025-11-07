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
    public class GetAllLeaveTypeQuery : IRequest<ApiResponse<List<GetLeaveTypResponseDTO>>>
    {
        public GetLeaveTypeRequestDTO DTO { get; set; }

        public GetAllLeaveTypeQuery(GetLeaveTypeRequestDTO dto)
        {
            this.DTO = dto;
        }
    }
}
 
