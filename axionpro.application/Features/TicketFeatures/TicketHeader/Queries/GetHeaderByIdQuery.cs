using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Queries
{
   
    public class GetHeaderByIdQuery : IRequest<ApiResponse<List<GetHeaderResponseDTO>>>
    {
        public GetHeaderRequestDTO  DTO { get; set; }

        public GetHeaderByIdQuery(GetHeaderRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
