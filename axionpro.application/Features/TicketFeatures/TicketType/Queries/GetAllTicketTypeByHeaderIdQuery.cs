using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TicketFeatures.TicketType.Queries
{
    public class GetAllTicketTypeByHeaderIdQuery : IRequest<ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        public GetTicketTypeByHeaderIdRequestDTO DTO { get; set; }

        public GetAllTicketTypeByHeaderIdQuery(GetTicketTypeByHeaderIdRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
