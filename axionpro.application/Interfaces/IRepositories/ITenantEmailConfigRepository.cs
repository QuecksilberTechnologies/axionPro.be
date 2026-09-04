using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Provides Tenant-scoped SMTP configuration persistence. SMTP secrets are
/// handled only by command handlers and are never projected to an API response.
/// </summary>
public interface ITenantEmailConfigRepository
{
    Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? tenantId);
    Task<TenantEmailConfig?> UpdateEmailConfigAsync(TenantEmailConfig? config);
    Task<TenantEmailConfig?> InsertEmailConfigAsync(TenantEmailConfig? config);

    Task<TenantEmailConfig?> GetByIdAsync(long tenantId, int id, CancellationToken cancellationToken = default);
    Task<TenantEmailConfig?> GetForUpdateAsync(long tenantId, int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantEmailConfig>> GetAllAsync(long tenantId, CancellationToken cancellationToken = default);
    Task DeactivateOtherActiveAsync(long tenantId, int? excludedId, CancellationToken cancellationToken = default);
    Task AddAsync(TenantEmailConfig configuration, CancellationToken cancellationToken = default);
    void Remove(TenantEmailConfig configuration);
}
