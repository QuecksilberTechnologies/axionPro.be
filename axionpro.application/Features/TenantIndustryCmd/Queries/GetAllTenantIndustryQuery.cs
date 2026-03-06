using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.TenantIndustry;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TenantIndustryCmd.Queries
{
    public class GetAllTenantIndustryQuery : IRequest<ApiResponse<List<TenantIndustryResponseDTO>>>
    {
        public int PlanId { get; set; }

        public GetAllTenantIndustryQuery(int planId)
        {
            PlanId = planId;
        }
    }

}
