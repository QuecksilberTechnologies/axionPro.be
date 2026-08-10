using axionpro.application.DTOs.SubscriptionModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ISubscriptionRepository
    {
        // 🎯 Add new subscription plan
        Task<SubscriptionPlan> AddSubscriptionPlanAsync(SubscriptionPlan entity);

        // 🔄 Update existing subscription plan
        Task<SubscriptionPlan> UpdateSubscriptionPlanAsync(SubscriptionPlan entity);

        // 📋 Get all active subscription plans
        Task<List<SubscriptionActivePlanDTO>> GetAllPlansAsync();

        // 🔍 Get plan by ID
        Task<SubscriptionPlanResponseDTO> GetPlanByIdAsync(int id);

      

    }

}
