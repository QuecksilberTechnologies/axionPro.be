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
    public class CreateTicketTypeCommand : IRequest<ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        
            public AddTicketTypeRequestDTO DTO { get; set; }

            public CreateTicketTypeCommand(AddTicketTypeRequestDTO dTO)
            {
                DTO = dTO;
            }

        }
     
}
