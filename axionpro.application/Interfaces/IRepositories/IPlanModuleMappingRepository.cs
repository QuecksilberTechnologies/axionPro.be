using axionpro.application.DTOs.Tenant;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPlanModuleMappingRepository
    {
        // 🔍 Get all modules for a specific plan
        Task<PlanModuleMappingResponseDTO> GetModulesBySubscriptionPlanIdAsync(int? subscriptionPlanId);
        Task<List<Module>> GetAllSubscribedModuleAsync(int? subscriptionPlanId);



        // ➕ Add a new plan-module mapping
        Task AddAsync(PlanModuleMapping planModuleMapping);

        // 🗑️ Delete a mapping by id
        Task<bool> DeleteByIdAsync(int id);

        // ✅ Check if a module is already mapped to a plan
        Task<bool> IsModuleMappedAsync(int subscriptionPlanId, int moduleId);

        // 📝 Update mapping (optional)
        Task UpdateAsync(PlanModuleMapping planModuleMapping);
    }
}
