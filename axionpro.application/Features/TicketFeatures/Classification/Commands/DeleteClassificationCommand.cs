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
    public class DeleteClassificationCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteClassificationRequestDTO DTO { get; set; }

        public DeleteClassificationCommand(DeleteClassificationRequestDTO dto)
        {
            DTO = dto;
        }

    }
     
}
