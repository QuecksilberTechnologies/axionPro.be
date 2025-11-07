using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketType.Queries
{
    public class GetAllTicketTypeByRoleIdQuery : IRequest<ApiResponse<List<GetTicketTypeRoleResponseDTO>>>
    {
        public GetTicketTypeByRoleIdRequestDTO DTO { get; set; }

        public GetAllTicketTypeByRoleIdQuery(GetTicketTypeByRoleIdRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
