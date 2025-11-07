using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.SubscriptionCmd.Commands
{
    public class GetPlanModuleMappingCommand : IRequest<ApiResponse<PlanModuleMappingResponseDTO>>
    {

        public PlanModuleMappingRequestDTO planModuleMappingRequest { get; set; }

        public GetPlanModuleMappingCommand(PlanModuleMappingRequestDTO planModuleMappingRequest)
        {
            this.planModuleMappingRequest = planModuleMappingRequest;
        }

    }
}
