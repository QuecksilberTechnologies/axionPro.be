
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TicketFeatures.Classification.Queries
{
    public class GetClassificationInfoByIdQuery : IRequest<ApiResponse<GetClassificationResponseDTO>>
    {
        public GetClassificationRequestDTO DTO { get; set; }

        public GetClassificationInfoByIdQuery(GetClassificationRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
