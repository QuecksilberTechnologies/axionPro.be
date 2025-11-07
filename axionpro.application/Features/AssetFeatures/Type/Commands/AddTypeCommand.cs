using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.AssetDTO.type;


namespace axionpro.application.Features.AssetFeatures.Type.Commands
{
    public class AddTypeCommand : IRequest<ApiResponse<List<GetTypeResponseDTO>>>
    {
        public AddTypeRequestDTO DTO { get; set; }

        public AddTypeCommand(AddTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }
}
