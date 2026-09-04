using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories;

public sealed class TenantEmailConfigRepository(
    WorkforceDbContext context,
    ILogger<TenantEmailConfigRepository> logger) : ITenantEmailConfigRepository
{
    public Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? tenantId) =>
        context.TenantEmailConfigs
            .Include(configuration => configuration.Tenant)
                .ThenInclude(tenant => tenant.TenantProfile)
            .AsNoTracking()
            .Where(configuration => configuration.TenantId == tenantId && configuration.IsActive)
            .OrderBy(configuration => configuration.Id)
            .FirstOrDefaultAsync();

    public async Task<TenantEmailConfig?> InsertEmailConfigAsync(TenantEmailConfig? config)
    {
        if (config is null)
        {
            return null;
        }

        await context.TenantEmailConfigs.AddAsync(config);
        logger.LogInformation("Tenant email configuration added to DbContext for TenantId: {TenantId}", config.TenantId);
        return config;
    }

    public async Task<TenantEmailConfig?> UpdateEmailConfigAsync(TenantEmailConfig? config)
    {
        if (config is null)
        {
            return null;
        }

        var existing = await context.TenantEmailConfigs
            .FirstOrDefaultAsync(configuration => configuration.Id == config.Id);
        if (existing is null)
        {
            return null;
        }

        existing.SmtpHost = config.SmtpHost;
        existing.SmtpPort = config.SmtpPort;
        existing.SmtpUsername = config.SmtpUsername;
        existing.SmtpPasswordEncrypted = config.SmtpPasswordEncrypted;
        existing.FromEmail = config.FromEmail;
        existing.FromName = config.FromName;
        existing.IsActive = config.IsActive;
        if (!string.IsNullOrWhiteSpace(config.SecrateKey))
        {
            existing.SecrateKey = config.SecrateKey;
        }

        logger.LogInformation("Tenant email configuration updated in DbContext for Id: {TenantEmailConfigId}", config.Id);
        return existing;
    }

    public Task<TenantEmailConfig?> GetByIdAsync(
        long tenantId,
        int id,
        CancellationToken cancellationToken = default) =>
        context.TenantEmailConfigs
            .Include(configuration => configuration.Tenant)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                configuration => configuration.TenantId == tenantId && configuration.Id == id,
                cancellationToken);

    public Task<TenantEmailConfig?> GetForUpdateAsync(
        long tenantId,
        int id,
        CancellationToken cancellationToken = default) =>
        context.TenantEmailConfigs.FirstOrDefaultAsync(
            configuration => configuration.TenantId == tenantId && configuration.Id == id,
            cancellationToken);

    public async Task<IReadOnlyList<TenantEmailConfig>> GetAllAsync(
        long tenantId,
        CancellationToken cancellationToken = default) =>
        await context.TenantEmailConfigs
            .Include(configuration => configuration.Tenant)
            .AsNoTracking()
            .Where(configuration => configuration.TenantId == tenantId)
            .OrderByDescending(configuration => configuration.IsActive)
            .ThenBy(configuration => configuration.Id)
            .ToListAsync(cancellationToken);

    public async Task DeactivateOtherActiveAsync(
        long tenantId,
        int? excludedId,
        CancellationToken cancellationToken = default)
    {
        var configurations = context.TenantEmailConfigs.Where(configuration =>
            configuration.TenantId == tenantId &&
            configuration.IsActive &&
            (!excludedId.HasValue || configuration.Id != excludedId.Value));

        var affected = await configurations.ExecuteUpdateAsync(
            setters => setters.SetProperty(configuration => configuration.IsActive, false),
            cancellationToken);

        if (affected > 0)
        {
            logger.LogInformation(
                "Deactivated {TenantEmailConfigCount} previous active SMTP configuration(s) for TenantId: {TenantId}",
                affected,
                tenantId);
        }
    }

    public async Task AddAsync(TenantEmailConfig configuration, CancellationToken cancellationToken = default) =>
        await context.TenantEmailConfigs.AddAsync(configuration, cancellationToken);

    public void Remove(TenantEmailConfig configuration) => context.TenantEmailConfigs.Remove(configuration);
}
