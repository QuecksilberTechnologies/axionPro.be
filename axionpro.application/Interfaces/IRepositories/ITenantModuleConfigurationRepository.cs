// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Tenant Module Configuration Repository.
// ================================================================

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
   /// <summary>
   /// Defines the contract for TenantModuleConfigurationRepository.
   /// </summary>
   public interface  ITenantModuleConfigurationRepository
    {
        Task  CreateByDefaultEnabledModulesAsync(long TenantId, List<TenantEnabledModule> moduleEntities, List<TenantEnabledOperation> operationEntities);

        /// <summary>
        /// Stages only missing entitlement rows from the Tenant's active subscription plan.
        /// Existing Tenant rows are never changed or duplicated.
        /// </summary>
        /// <param name="tenantId">The authoritative Tenant identifier.</param>
        /// <param name="addedById">The authenticated Host user performing the explicit sync.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The staged synchronization counts, or <see langword="null"/> when no active plan exists.</returns>
        Task<TenantPlanEntitlementSyncResult?> SynchronizeMissingActivePlanEntitlementsAsync(
            long tenantId,
            long addedById,
            CancellationToken cancellationToken = default);
        
        //yeh function sirf enabled module or operation laata hai , login mei bhi used

        //Task<List<TenantEnabledModule>> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(long? TenantId);
        Task <GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto);
        Task<GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledOperation dto);
         
       

       
    }
}
