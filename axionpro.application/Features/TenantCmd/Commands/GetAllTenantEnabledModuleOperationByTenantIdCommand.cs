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
    public class GetTenantEnabledModuleCommand : IRequest<ApiResponse<GetModuleHierarchyResponseDTO>>
    {

        public TenantEnabledModuleRequestDTO dto { get; set; }

        public GetTenantEnabledModuleCommand(TenantEnabledModuleRequestDTO dto)
        {
            this.dto = dto;
        }

    }
}