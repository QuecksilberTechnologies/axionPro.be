using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands
{

    public class TenantEnabledModuleOperationsUpdateCommand : IRequest<ApiResponse<bool>>
    {
        public TenantModuleOperationsUpdateRequestDTO RequestDTO { get; set; }

        public TenantEnabledModuleOperationsUpdateCommand(TenantModuleOperationsUpdateRequestDTO dto)
        {
            RequestDTO = dto;
        }
    }

}
