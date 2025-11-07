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
    public class GetClassificationByIdQuery : IRequest<ApiResponse<GetClassificationResponseDTO>>
        {
            public GetClassificationRequestDTO Dto { get; set; }

            public GetClassificationByIdQuery(GetClassificationRequestDTO dto)
            {
                Dto = dto;
            }
        }
     
}
