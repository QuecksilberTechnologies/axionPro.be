using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Commands
{
    public class GetSubscriptionPlanCommand : IRequest<ApiResponse<List<SubscriptionActivePlanDTO>>>
    {

        public SubscriptionPlanRequestDTO subscriptionPlanRequestDTO { get; set; }

        public GetSubscriptionPlanCommand(SubscriptionPlanRequestDTO subscriptionPlanRequestDTO)
        {
            this.subscriptionPlanRequestDTO = subscriptionPlanRequestDTO;
        }

    }
}