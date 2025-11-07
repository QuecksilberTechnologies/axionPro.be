using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Commands
{
    public class GetModuleOperationMappingRepositoryCommand : IRequest<ApiResponse<List<GetModuleOperationRolePermissionsResponseDTO>>>
    {

        public GetTenantModuleOperationRolePermissionsRequestDTO dto { get; set; }

        public GetModuleOperationMappingRepositoryCommand(GetTenantModuleOperationRolePermissionsRequestDTO dto)
        {
            this.dto = dto;
        }

    }
}