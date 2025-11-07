 
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Assets.Commands
{
    public class DeleteAssetCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteAssetReqestDTO DTO { get; set; }

        public DeleteAssetCommand(DeleteAssetReqestDTO dto)
        {
            DTO = dto;
        }

    }
     
}
