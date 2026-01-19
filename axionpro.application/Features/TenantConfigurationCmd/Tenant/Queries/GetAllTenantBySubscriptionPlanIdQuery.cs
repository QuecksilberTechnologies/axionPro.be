using axionpro.application.DTOs.Operation;
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
    public class GetAllTenantBySubscriptionPlanIdQuery : IRequest<ApiResponse<List<TenantResponseDTO>>>
    {
        public TenantRequestDTO? tenantRequestDTO { get; set; }

        public GetAllTenantBySubscriptionPlanIdQuery(TenantRequestDTO tenantRequestDTO)
        {
            this.tenantRequestDTO = tenantRequestDTO;
        }
    }
}
