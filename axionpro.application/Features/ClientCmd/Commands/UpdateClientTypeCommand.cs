using axionpro.application.DTOs.Client;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ClientCmd.Commands
{
    public class UpdateClientTypeCommand : IRequest<ApiResponse<List<GetClientTypeDTO>>>
    {

        public UpdateClientTypeDTO updateClientTypeCommand { get; set; }

        public UpdateClientTypeCommand(UpdateClientTypeDTO updateClientTypeCommand)
        {
            this.updateClientTypeCommand = updateClientTypeCommand;
        }
    }
}
