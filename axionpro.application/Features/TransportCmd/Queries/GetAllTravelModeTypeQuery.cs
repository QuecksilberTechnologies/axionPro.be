 
using axionpro.application.DTOs.Transport;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

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
