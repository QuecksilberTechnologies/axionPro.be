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
    public class UpdateStatusCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateStatusRequestDTO DTO { get; set; }

        public UpdateStatusCommand(UpdateStatusRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
}
