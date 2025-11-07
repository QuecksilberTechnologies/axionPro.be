using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.Classification.Commands
{
    public class UpdateClassificationCommand : IRequest<ApiResponse<GetClassificationResponseDTO>>
    {
        public UpdateClassificationRequestDTO DTO { get; set; }

        public UpdateClassificationCommand(UpdateClassificationRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }



}
