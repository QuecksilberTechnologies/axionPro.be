using axionpro.application.DTOs.Client;
 
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ClientCmd.Queries
{
    public class GetClientTypeQuery : IRequest<ApiResponse<List<GetClientTypeDTO>>>
    {
        public ClientRequestTypeDTO clientTypeRequestDTO { get; set; }

        public GetClientTypeQuery(ClientRequestTypeDTO clientTypeRequestDTO)
        {
            this.clientTypeRequestDTO = clientTypeRequestDTO;
        }
    }
    
    
}
