using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Role;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{


    public interface IRoleRepository
        {
        Task<GetSingleRoleResponseDTO?> GetByIdAsync1(GetSingleRoleRequestDTO dto);
        Task<ApiResponse<List<GetRoleOptionResponseDTO?>>> GetOptionAsync(GetRoleOptionRequestDTO dto);

        Task<bool> DeleteAsync(DeleteRoleRequestDTO requestDTO, long EmployeeId, int id);
        Task<PagedResponseDTO<GetRoleResponseDTO>> CreateAsync(CreateRoleRequestDTO dto);
        Task<PagedResponseDTO<GetRoleResponseDTO>> GetAsync(GetRoleRequestDTO dto);
        public Task<List<GetRoleResponseDTO>> GetRoleAsync(long tenantId, int roleTypeId, bool isActive);
        Task<bool> UpdateAsync(UpdateRoleRequestDTO requestDTO); 
        Task<Role> AutoCreatedSingleTenantRoleAsync(Role role);
        Task<int> AutoCreatedForTenantRoleAsync(List<Role> roles);
        Task<int> AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(long? TenantId, long employeeId, int role);
         
            
        }
     

 
}
