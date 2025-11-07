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
    public class GetAllClientTypeQuery : IRequest<ApiResponse<List<GetAllClientTypeDTO>>>
    {
        public ClientRequestTypeDTO clientTypeRequestDTO { get; set; }

        public GetAllClientTypeQuery(ClientRequestTypeDTO clientTypeRequestDTO)
        {
            this.clientTypeRequestDTO = clientTypeRequestDTO;
        }
    }
    
    
}
