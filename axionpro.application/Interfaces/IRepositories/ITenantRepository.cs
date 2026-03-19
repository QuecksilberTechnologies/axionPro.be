using axionpro.application.DTOs.Registration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(long? id, bool isActive);
        Task<bool> CheckTenantByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Tenant?> GetByCodeAsync(string tenantCode, CancellationToken cancellationToken = default);
        Task<List<Tenant>> GetAllTenantBySubscriptionIdAsync(Tenant tenant, CancellationToken cancellationToken = default);

        Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
        Task AddTenantProfileAsync(TenantProfile tenantProfile, CancellationToken cancellationToken = default);

        Task<Tenant?> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
        Task DeleteTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    }

 }