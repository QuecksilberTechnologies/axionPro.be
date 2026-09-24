using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

public sealed class TenantEmailTemplateRepository(WorkforceDbContext context) : ITenantEmailTemplateRepository
{
    public Task<TenantEmailTemplate?> GetActiveByCodeAsync(
        long tenantId,
        string templateCode,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(templateCode))
        {
            return Task.FromResult<TenantEmailTemplate?>(null);
        }

        var normalizedCode = templateCode.Trim().ToUpperInvariant();
        return context.TenantEmailTemplates
            .AsNoTracking()
            .Where(template =>
                template.TenantId == tenantId &&
                template.IsActive &&
                template.TemplateCode != null &&
                template.TemplateCode.ToUpper() == normalizedCode)
            .OrderByDescending(template => template.AddedDateTime)
            .ThenByDescending(template => template.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TenantEmailTemplate?> GetByIdAsync(long tenantId, int id, CancellationToken cancellationToken = default) =>
        context.TenantEmailTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);

    public Task<TenantEmailTemplate?> GetForUpdateAsync(long tenantId, int id, CancellationToken cancellationToken = default) =>
        context.TenantEmailTemplates.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);

    public async Task<PagedResponseDTO<TenantEmailTemplate>> GetPagedAsync(long tenantId, TenantEmailTemplateListRequestDTO filter, CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        var query = context.TenantEmailTemplates.AsNoTracking().Where(x => x.TenantId == tenantId);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.TemplateName, search) ||
                                     (x.TemplateCode != null && EF.Functions.ILike(x.TemplateCode, search)) ||
                                     EF.Functions.ILike(x.Subject, search) ||
                                     (x.Category != null && EF.Functions.ILike(x.Category, search)));
        }
        if (!string.IsNullOrWhiteSpace(filter.Category)) query = query.Where(x => x.Category != null && EF.Functions.ILike(x.Category, filter.Category.Trim()));
        if (!string.IsNullOrWhiteSpace(filter.LanguageCode)) query = query.Where(x => x.LanguageCode != null && EF.Functions.ILike(x.LanguageCode, filter.LanguageCode.Trim()));
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.IsActive).ThenBy(x => x.TemplateName).ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResponseDTO<TenantEmailTemplate>(data, totalCount, pageNumber, pageSize)
        {
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize))
        };
    }

    public Task<bool> TemplateCodeExistsAsync(long tenantId, string templateCode, int? excludedId = null, CancellationToken cancellationToken = default)
    {
        var code = templateCode.Trim().ToUpperInvariant();
        return context.TenantEmailTemplates.AnyAsync(x => x.TenantId == tenantId && x.TemplateCode != null &&
            x.TemplateCode.ToUpper() == code && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);
    }

    public async Task AddAsync(TenantEmailTemplate template, CancellationToken cancellationToken = default) =>
        await context.TenantEmailTemplates.AddAsync(template, cancellationToken);

    public void Remove(TenantEmailTemplate template) => context.TenantEmailTemplates.Remove(template);
}
