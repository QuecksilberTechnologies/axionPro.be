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
    public class AddStatusCommand : IRequest<ApiResponse<GetStatusResponseDTO>>
    {
        public CreateStatusRequestDTO DTO { get; set; }

        public AddStatusCommand(CreateStatusRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
}
