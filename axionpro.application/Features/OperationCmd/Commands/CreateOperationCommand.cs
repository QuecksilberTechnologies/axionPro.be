 
using axionpro.application.DTOs.Operation;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.OperationCmd.Commands
{
    public class CreateOperationCommand : IRequest<ApiResponse<List<GetOperationResponseDTO>>>
    {

        public CreateOperationRequestDTO createOperationDTO { get; set; }

        public CreateOperationCommand(CreateOperationRequestDTO createOperationDTO)
        {
            this.createOperationDTO = createOperationDTO;
        }

    }
}
