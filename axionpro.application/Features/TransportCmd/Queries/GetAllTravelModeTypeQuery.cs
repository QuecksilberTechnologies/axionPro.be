 
using axionpro.application.DTOs.Transport;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TransportCmd.Queries
{
    public class GetAllTravelModeTypeQuery : IRequest<ApiResponse<List<GetAllTravelModeDTO>>>
    {
        public TravelModeRequestDTO? travelModeRequestDTO { get; set; }

        public GetAllTravelModeTypeQuery(TravelModeRequestDTO clientTypeRequestDTO)
        {
            this.travelModeRequestDTO = travelModeRequestDTO;
        }
    }
    
}
