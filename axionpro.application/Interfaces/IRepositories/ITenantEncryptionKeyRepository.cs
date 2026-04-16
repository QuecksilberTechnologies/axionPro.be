using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEncryptionKeyRepository
    {
        Task<TenantEncryptionKey?> GetActiveKeyByTenantIdAsync(long tenantId, CancellationToken cancellationToken = default);
        Task AddAsync(TenantEncryptionKey tenantKey, CancellationToken cancellationToken = default);
    //    Task UpdateAsync(TenantEncryptionKey tenantKey, CancellationToken cancellationToken = default);

    }
}