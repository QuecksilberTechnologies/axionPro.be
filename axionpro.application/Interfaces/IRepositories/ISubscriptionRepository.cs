using axionpro.application.DTOs.SubscriptionModule;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ISubscriptionRepository
    {
        // 🎯 Add new subscription plan
        Task<int> AddSubscriptionPlanAsync(SubscriptionPlanRequestDTO dto);

        // 🔄 Update existing subscription plan
        Task<bool> UpdateSubscriptionPlanAsync(int id, SubscriptionPlanRequestDTO dto);

        // 📋 Get all active subscription plans
        Task<List<SubscriptionActivePlanDTO>> GetAllPlansAsync();

        // 🔍 Get plan by ID
        Task<SubscriptionPlanResponseDTO> GetPlanByIdAsync(int id);

      

    }

}
