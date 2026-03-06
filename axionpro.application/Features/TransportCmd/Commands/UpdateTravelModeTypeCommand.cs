using axionpro.application.DTOs.Transport;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TransportCmd.Commands
{
    public class UpdateTravelModeTypeCommand : IRequest<ApiResponse<List<GetAllTravelModeDTO>>>
    {

        public UpdateTravelModeDTO updateTravelModeDTO { get; set; }

        public UpdateTravelModeTypeCommand(UpdateTravelModeDTO createClientTypeDTO)
        {
            this.updateTravelModeDTO = createClientTypeDTO;
        }

    }
}