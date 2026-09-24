using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

public interface ITenantEmailTemplateRepository
{
    Task<TenantEmailTemplate?> GetActiveByCodeAsync(long tenantId, string templateCode, CancellationToken cancellationToken = default);
    Task<TenantEmailTemplate?> GetByIdAsync(long tenantId, int id, CancellationToken cancellationToken = default);
    Task<TenantEmailTemplate?> GetForUpdateAsync(long tenantId, int id, CancellationToken cancellationToken = default);
    Task<PagedResponseDTO<TenantEmailTemplate>> GetPagedAsync(long tenantId, TenantEmailTemplateListRequestDTO filter, CancellationToken cancellationToken = default);
    Task<bool> TemplateCodeExistsAsync(long tenantId, string templateCode, int? excludedId = null, CancellationToken cancellationToken = default);
    Task<List<string>> GetTemplateCodesAsync(long tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(TenantEmailTemplate template, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TenantEmailTemplate> templates, CancellationToken cancellationToken = default);
    void Remove(TenantEmailTemplate template);
}
