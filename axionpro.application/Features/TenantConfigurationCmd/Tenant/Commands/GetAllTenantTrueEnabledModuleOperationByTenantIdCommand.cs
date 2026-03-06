using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands
{
    public class GetAllTenantTrueEnabledModuleOperationByTenantIdCommand : IRequest<ApiResponse<TenantEnabledModuleOperationsResponseDTO>>
    {

        public TenantEnabledModuleRequestDTO TenantEnabledModuleOperationsRequestDTO { get; set; }

        public GetAllTenantTrueEnabledModuleOperationByTenantIdCommand(TenantEnabledModuleRequestDTO TenantEnabledModuleOperationsRequestDTO)
        {
            this.TenantEnabledModuleOperationsRequestDTO = TenantEnabledModuleOperationsRequestDTO;
        }

    }
}
