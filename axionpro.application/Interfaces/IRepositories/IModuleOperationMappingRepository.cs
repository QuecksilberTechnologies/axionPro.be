using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.ModuleOperation;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IModuleOperationMappingRepository  
    {

       
        // Save new mappings
        Task<GetModuleOperationMappingResponseDTO> SaveModuleOperationMappingsAsync(GetModuleOperationMappingRequestDTO dto);
        Task<ModuleOperationMapping> UpdateModuleOperationMappingsAsync(ModuleOperationMapping dto);
            
            // Get mappings for a specific product owner/module
            Task<List<ModuleOperationMapping>> GetModuleOperationMappings(List<Module> modules);
          
           Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesOperationRolePermissionResponses(GetTenantModuleOperationRolePermissionsRequestDTO request);

        // Optional: Delete or overwrite previous mappings


    }
}
