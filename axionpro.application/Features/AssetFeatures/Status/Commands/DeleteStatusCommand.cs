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
    public class DeleteStatusCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteStatusReqestDTO? deleteAssetStatusRequest { get; set; }

    public DeleteStatusCommand(DeleteStatusReqestDTO deleteAssetStatusRequest)
    {
        this.deleteAssetStatusRequest = deleteAssetStatusRequest;
    }

}
}
