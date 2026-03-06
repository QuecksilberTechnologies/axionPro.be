using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Commands
{
    public class CreateModuleOperationRolePermissionCommand : IRequest<ApiResponse<int>>
    {
        public CreateModuleOperationRolePermissionsRequestDTO dto { get; set; }

        public CreateModuleOperationRolePermissionCommand(CreateModuleOperationRolePermissionsRequestDTO dto)
        {
            this.dto = dto; 
        }

    }
}
