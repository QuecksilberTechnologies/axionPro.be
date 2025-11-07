 
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Assets.Commands
{
    public class AddAssetCommand : IRequest<ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>>
    {
        public AddAssetRequestDTO dto { get; set; }

        public AddAssetCommand(AddAssetRequestDTO dto)
        {
            this.dto = dto;
        }

    }



}
