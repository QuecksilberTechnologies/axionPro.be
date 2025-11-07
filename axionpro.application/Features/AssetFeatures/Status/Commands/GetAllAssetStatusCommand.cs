using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Status.Commands
{
    public class GetAllAssetStatusCommand : IRequest<ApiResponse<List<GetStatusResponseDTO>>>
    {
        public GetStatusRequestDTO DTO { get; set; }

        public GetAllAssetStatusCommand(GetStatusRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
}
