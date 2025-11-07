using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantCmd.Queries;
using axionpro.domain.Entity;
 
using System;
using System.Collections.Generic;
    using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using System.Threading.Tasks;

    namespace axionpro.application.Interfaces.IRepositories
    {
        public interface ITenantSubscriptionRepository
        {
        /// <summary>
        /// Get subscription detail for a tenant based on filter.
        /// </summary>
        Task<List<TenantSubscriptionPlanResponseDTO>> GetTenantSubscriptionPlanInfoAsync(TenantSubscriptionPlanRequestDTO dto);

         /// <summary>
         /// Get all tenant subscriptions (optional filter).
         /// </summary>
         Task<List<TenantSubscription>> GetAllTenantSubscriptionsAsync(TenantSubscription? filter = null);

         Task<TenantSubscriptionPlanResponseDTO> GetValidateTenantPlan(TenantSubscriptionPlanRequestDTO dto);


        /// <summary>
        /// Add a new tenant subscription.
        /// </summary>
        Task<TenantSubscription> AddTenantSubscriptionAsync(TenantSubscription subscription);

            /// <summary>
            /// Update an existing tenant subscription.
            /// </summary>
            Task UpdateTenantSubscriptionAsync(TenantSubscription subscription);

            /// <summary>
            /// Soft delete a tenant subscription.
            /// </summary>
            Task<bool> DeleteTenantSubscriptionAsync(long id);
        }
    }

 
