using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Provides persistence operations for centrally managed email templates.
/// </summary>
public interface IEmailTemplateRepository
{
    /// <summary>
    /// Gets an active template by its business code for the existing mail-delivery flow.
    /// </summary>
    Task<EmailTemplate?> GetTemplateByCodeAsync(string templateCode, CancellationToken cancellationToken = default);

    Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmailTemplate?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponseDTO<EmailTemplate>> GetPagedAsync(
        EmailTemplateListRequestDTO filter,
        CancellationToken cancellationToken = default);
    Task<bool> TemplateCodeExistsAsync(
        string templateCode,
        int? excludedId = null,
        CancellationToken cancellationToken = default);
    Task<bool> HasEmailQueueEntriesAsync(int templateId, CancellationToken cancellationToken = default);
    Task AddAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    void Remove(EmailTemplate template);
}
