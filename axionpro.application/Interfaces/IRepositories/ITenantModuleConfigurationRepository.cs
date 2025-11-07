using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
   public interface  ITenantModuleConfigurationRepository
    {
        Task  CreateByDefaultEnabledModulesAsync(long TenantId, List<TenantEnabledModule> moduleEntities, List<TenantEnabledOperation> operationEntities);
        
        //yeh function sirf enabled module or operation laata hai , login mei bhi used
        Task<List<TenantEnabledModule>> GetAllTenantEnabledModulesWithOperationsAsync(long? TenantId);
        //Task<List<TenantEnabledModule>> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(long? TenantId);
        Task <GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto);
         Task <TenantEnabledModuleOperationsResponseDTO> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(TenantEnabledModuleRequestDTO tenantEnabledModuleOperationsRequestDTO);
        Task <TenantEnabledModuleOperationsResponseDTO> GetEnabledModuleWithOperation(TenantEnabledModuleRequestDTO tenantEnabledModuleOperationsRequestDTO);
        /// <summary>
        /// Updates the module and operation enable/disable state for a given tenant.
        /// </summary>
        /// <param name="request">The request DTO containing module and operation status.</param>
        /// <returns>True if update successful, otherwise false.</returns>
        Task<bool> UpdateTenantModuleAndItsOperationsAsync(TenantModuleOperationsUpdateRequestDTO request);

       
    }
}
