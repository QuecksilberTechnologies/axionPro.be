using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class TenantEmailConfigRepository : ITenantEmailConfigRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantEmailConfigRepository> _logger;

        public TenantEmailConfigRepository(WorkforceDbContext context, ILogger<TenantEmailConfigRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? tenantId)
        {
            return await _context.TenantEmailConfigs
                .Include(x => x.Tenant)          // 🔥 YAHI LINE IMPORTANT HAI
                .Include(x => x.Tenant.TenantProfiles) // logo etc ke liye
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.IsActive == true);
        }

        public async Task<TenantEmailConfig?> UpdateEmailConfigAsync(TenantEmailConfig? config)
        {
            if (config == null)
                return null;

            var existing = await _context.TenantEmailConfigs
                .FirstOrDefaultAsync(x => x.Id == config.Id);

            if (existing == null)
                return null;

            existing.SmtpHost = config.SmtpHost;
            existing.SmtpPort = config.SmtpPort;
            existing.SmtpUsername = config.SmtpUsername;
            existing.SmtpPasswordEncrypted = config.SmtpPasswordEncrypted;
            existing.FromEmail = config.FromEmail;
            existing.FromName = config.FromName;
            existing.IsActive = config.IsActive;
            existing.SecrateKey = config.SecrateKey;
            await _context.SaveChangesAsync();

            return existing;
        }
        public async Task<TenantEmailConfig?> InsertEmailConfigAsync(TenantEmailConfig? config)
        {
            if (config == null)
                return null;

            await _context.TenantEmailConfigs.AddAsync(config);

            await _context.SaveChangesAsync();

            return config;
        }
    }

}
