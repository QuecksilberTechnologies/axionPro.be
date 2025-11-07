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
    public class DeletetTypeCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteTypeRequestDTO DTO { get; set; }

        public DeletetTypeCommand(DeleteTypeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

}