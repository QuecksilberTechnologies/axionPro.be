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
        Task<ApiResponse<List<GetRoleOptionResponseDTO?>>> GetOptionAsync(GetRoleOptionRequestDTO dto, long tenantId);

        Task<bool> DeleteAsync(DeleteRoleRequestDTO requestDTO, long EmployeeId);
        Task<PagedResponseDTO<GetRoleResponseDTO>> CreateAsync(CreateRoleRequestDTO dto, long TenantId, long EmployeeId);
        Task<PagedResponseDTO<GetRoleResponseDTO>> GetAsync(GetRoleRequestDTO request, long tenantId , int Id);
              
        Task<bool> UpdateAsync(UpdateRoleRequestDTO requestDTO, long EmployeeId); 
        Task<Role> AutoCreatedSingleTenantRoleAsync(Role role);
        Task<int> AutoCreatedForTenantRoleAsync(List<Role> roles);
        Task<int> AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(long? TenantId, long employeeId, int role);
         
            
        }
     

 
}
