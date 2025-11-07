using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Type.Commands
{
    public class GetAllTypeCommand : IRequest<ApiResponse<List<GetTypeResponseDTO>>>
    {
        public GetTypeRequestDTO DTO { get; set; }

        public GetAllTypeCommand(GetTypeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
}
