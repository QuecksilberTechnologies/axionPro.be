using axionpro.application.DTOs.Client;
 
using axionpro.application.Features.ClientCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ClientCmd.Commands
{
    public class CreateClientTypeCommand :  IRequest<ApiResponse<List<GetAllClientTypeDTO>>>
    {
        
            public CreateClientTypeDTO createClientTypeDTO { get; set; }

    public CreateClientTypeCommand(CreateClientTypeDTO createClientTypeDTO)
    {
        this.createClientTypeDTO = createClientTypeDTO;
    }
}
}

