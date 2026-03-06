using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.SubscriptionCmd.Commands
{
    public class GetValidateTenantIdCommand :IRequest<ApiResponse<TenantSubscriptionPlanResponseDTO>>
    {

        public TenantSubscriptionPlanRequestDTO dto { get; set; }

    public GetValidateTenantIdCommand(TenantSubscriptionPlanRequestDTO dto)
    {
        this.dto = dto;
    }

}
}
