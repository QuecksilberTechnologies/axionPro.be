
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Commands
{
    public class AddHeaderCommand : IRequest<ApiResponse<List<GetHeaderResponseDTO>>>
    {
        public AddHeaderRequestDTO dto { get; set; }

        public AddHeaderCommand(AddHeaderRequestDTO dto)
        {
            this.dto = dto;
        }

    }



}
