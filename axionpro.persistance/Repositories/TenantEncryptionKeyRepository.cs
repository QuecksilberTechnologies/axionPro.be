using AutoMapper;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.IContext;
using axionpro.domain.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using axionpro.persistance.Data.Context;

namespace axionpro.persistance.Repositories
{

    public class TenantEncryptionKeyRepository : ITenantEncryptionKeyRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantEncryptionKeyRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;

        public TenantEncryptionKeyRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<TenantEncryptionKeyRepository> logger,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        // ✅ Add new tenant encryption key
        public async Task<int> AddAsync(TenantEncryptionKey tenantKey)
        {
            try
            {
                await _context.AddAsync(tenantKey);
                int record = await _context.SaveChangesAsync();
                if(record <= 0)
                {
                    _logger.LogWarning("No records were added for TenantId: {TenantId}", tenantKey.TenantId);
                    throw new ApplicationException($"No records were added for TenantId {tenantKey.TenantId}");
                }
                _logger.LogInformation("Tenant encryption key added successfully for TenantId: {TenantId}", tenantKey.TenantId);
               
                   return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant encryption key for TenantId: {TenantId}", tenantKey.TenantId);
                throw new ApplicationException($"Failed to add tenant key for TenantId {tenantKey.TenantId}", ex);
            }
        }

        // ✅ Get active encryption key by tenant
        public async Task<TenantEncryptionKey> GetActiveKeyByTenantIdAsync(long tenantId)
        {
            try
            {
                var key = await _context.TenantEncryptionKeys
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive==true);

                if (key == null)
                {
                    _logger.LogWarning("No active encryption key found for TenantId: {TenantId}", tenantId);
                    throw new KeyNotFoundException($"No active key found for TenantId {tenantId}");
                }

                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active encryption key for TenantId: {TenantId}", tenantId);
                throw new ApplicationException($"Failed to get tenant key for TenantId {tenantId}", ex);
            }
        }

        // ✅ Update/Rotate existing tenant key
        public async Task UpdateAsync(TenantEncryptionKey tenantKey)
        {
            try
            {
                _context.TenantEncryptionKeys.Update(tenantKey);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant encryption key updated successfully for TenantId: {TenantId}", tenantKey.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant encryption key for TenantId: {TenantId}", tenantKey.TenantId);
                throw new ApplicationException($"Failed to update tenant key for TenantId {tenantKey.TenantId}", ex);
            }
        }
    }
}
