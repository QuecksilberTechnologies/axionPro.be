using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEncryptionKeyRepository
    {
        Task<TenantEncryptionKeys?> GetActiveKeyByTenantIdAsync(long tenantId, CancellationToken cancellationToken = default);
        Task AddAsync(TenantEncryptionKeys tenantKey, CancellationToken cancellationToken = default);
       Task UpdateAsync(TenantEncryptionKeys tenantKey, CancellationToken cancellationToken = default);

    }
}