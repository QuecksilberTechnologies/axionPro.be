
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
    public class UpdateHeaderCommand : IRequest<ApiResponse<GetHeaderResponseDTO>>
    {
        public  UpdateHeaderRequestDTO DTO { get; set; }

        public UpdateHeaderCommand(UpdateHeaderRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }



}
