using axionpro.application.DTOs.Registration;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantRepository
    {
        Task<Tenant> GetByIdAsync(long? id);
        Task<bool> CheckTenantByEmail(string email);
        Task<Tenant> GetByCodeAsync(string tenantCode);
        Task<List<Tenant>> GetAllTenantBySubscriptionIdAsync(Tenant tenant);
        Task<long> AddTenantAsync(Tenant tenant);
        Task<long> AddTenantProfileAsync(TenantProfile tenantProfile);
        Task <Tenant> UpdateTenantAsync(Tenant? tenant);
        Task DeleteTenantAsync(Tenant tenant);
        
    }
}
