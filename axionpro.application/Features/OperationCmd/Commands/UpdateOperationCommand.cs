using axionpro.application.DTOs.Operation;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.OperationCmd.Commands
{
    public class UpdateOperationCommand : IRequest<ApiResponse<List<GetOperationResponseDTO>>>
    {

        public UpdateOperationRequestDTO updateOperationDTO { get; set; }

        public UpdateOperationCommand(UpdateOperationRequestDTO updateOperationDTO)
        {
            this.updateOperationDTO = updateOperationDTO;
        }

    }
}
