using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantCmd.Commands
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
