using axionpro.application.DTOs.BasicAndRoleBaseMenu;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUserRolesPermissionOnModuleRepository
    {
        public Task<IEnumerable<UserRolesPermissionOnModuleDTO>> GetModuleListAndOperationByRollIdAsync(List<RoleInfoDTO> roleList, int? forPlatform);
        Task<int> AdminAssignModulePermissionAsync(List<RoleModuleAndPermission> insertRoleModulePermissionsRequestDTO);
    }
}
