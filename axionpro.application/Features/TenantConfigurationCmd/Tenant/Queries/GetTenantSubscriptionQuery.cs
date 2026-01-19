using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries
{
    public class GetTenantSubscriptionQuery : IRequest<ApiResponse<List<TenantSubscriptionPlanResponseDTO>>>
    {
        public TenantSubscriptionPlanRequestDTO? DTO { get; set; }

        public GetTenantSubscriptionQuery(TenantSubscriptionPlanRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }
}
