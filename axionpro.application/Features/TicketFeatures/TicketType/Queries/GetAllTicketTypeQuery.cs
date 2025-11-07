using axionpro.application.DTOs.Leave;
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
    public class GetAllTicketTypeQuery : IRequest<ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        public GetTicketTypeRequestDTO DTO { get; set; }

    public GetAllTicketTypeQuery(GetTicketTypeRequestDTO dTO)
    {
        DTO = dTO;
    }
}
}
