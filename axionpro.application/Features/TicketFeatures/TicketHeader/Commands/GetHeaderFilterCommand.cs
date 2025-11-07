
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Commands
{
   
    public class GetHeaderFilterCommand : IRequest<ApiResponse<List<GetHeaderResponseDTO>>>
    {
        public GetHeaderRequestDTO  DTO { get; set; }

        public GetHeaderFilterCommand(GetHeaderRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
