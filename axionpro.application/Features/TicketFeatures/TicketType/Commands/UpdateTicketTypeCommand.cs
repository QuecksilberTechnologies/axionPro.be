using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketType.Commands
{
    public class UpdateTicketTypeCommand : IRequest<ApiResponse<bool>>
    {
        
            public UpdateTicketTypeRequestDTO DTO { get; set; }

    public UpdateTicketTypeCommand(UpdateTicketTypeRequestDTO dTO)
    {
        DTO = dTO;
    }

}
     
}