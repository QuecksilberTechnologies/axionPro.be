using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;


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
