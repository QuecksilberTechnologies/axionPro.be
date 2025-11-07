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
    public class DeleteHeaderCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteHeaderRequestDTO DTO { get; set; }

        public DeleteHeaderCommand(DeleteHeaderRequestDTO dto)
        {
            DTO = dto;
        }

    }
     
}
