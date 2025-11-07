
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
    public class UpdateAssetCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateAssetRequestDTO DTO { get; set; }

        public UpdateAssetCommand(UpdateAssetRequestDTO dTO)
        {
            DTO = dTO;
        }

    }



}
