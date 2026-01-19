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
    public class GetTenantInfoByIdQuery : IRequest<ApiResponse<TenantResponseDTO>>
    {
        public long TenantId  { get; set; }

        public GetTenantInfoByIdQuery(long TenantId)
        {
            this.TenantId = TenantId;
        }
    }
}
