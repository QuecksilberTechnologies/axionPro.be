using axionpro.application.DTOs.Client;
 
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

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
