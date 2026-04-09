using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
   public interface  ITenantModuleConfigurationRepository
    {
        Task  CreateByDefaultEnabledModulesAsync(long TenantId, List<TenantEnabledModule> moduleEntities, List<TenantEnabledOperation> operationEntities);
        
        //yeh function sirf enabled module or operation laata hai , login mei bhi used

        //Task<List<TenantEnabledModule>> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(long? TenantId);
        Task <GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto);
         
       

       
    }
}
