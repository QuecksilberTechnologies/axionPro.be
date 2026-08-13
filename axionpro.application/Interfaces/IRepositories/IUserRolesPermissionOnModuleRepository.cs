using axionpro.application.DTOs.BasicAndRoleBaseMenu;
using axionpro.application.DTOs.ModuleOperation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.domain.Entity; 
using MediatR;
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
        Task<int> BulkInsertAsync(  List<RoleModuleAndPermission> rolePermissions);
        Task<TenantEnabledOperationsResponseDTO> GetAllTenantModuleWithOperation(TenantEnabledOperation dto);
        Task<bool> UpdateTenantModuleAndItsOperationsAsync(TenantModuleOperationsUpdateRequestDTO request);
        Task<List<TenantEnabledModule>> GetAllTenantEnabledModulesWithOperationsAsync(long? TenantId);
        Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesOperationRolePermissionResponses(GetTenantModuleOperationRolePermissionsRequestDTO request);

        Task<List<RoleModuleAndPermission>> GetByRoleIdAsync(int roleId);

        Task BulkDeleteAsync(List<RoleModuleAndPermission> list);

        // Retained for the existing tenant-registration permission flow.
        Task<List<ModuleOperationMapping>> GetModuleOperationMappings(List<Module> modules);


    }
}
