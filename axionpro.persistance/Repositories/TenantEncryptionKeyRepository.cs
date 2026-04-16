using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class TenantEncryptionKeyRepository : ITenantEncryptionKeyRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantEncryptionKeyRepository> _logger;

        public TenantEncryptionKeyRepository(
            WorkforceDbContext context,
            ILogger<TenantEncryptionKeyRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(TenantEncryptionKey tenantKey, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenantKey == null)
                {
                    throw new ArgumentNullException(nameof(tenantKey));
                }

                await _context.TenantEncryptionKeys.AddAsync(tenantKey, cancellationToken);

                _logger.LogInformation(
                    "Tenant encryption key added to DbContext successfully for TenantId: {TenantId}",
                    tenantKey.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error adding tenant encryption key to DbContext for TenantId: {TenantId}",
                    tenantKey?.TenantId);
                throw;
            }
        }

        public async Task<TenantEncryptionKey?> GetActiveKeyByTenantIdAsync(long tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var key = await _context.TenantEncryptionKeys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.TenantId == tenantId && x.IsActive == true,
                        cancellationToken);

                if (key == null)
                {
                    _logger.LogWarning("No active encryption key found for TenantId: {TenantId}", tenantId);
                    return null;
                }

                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching active encryption key for TenantId: {TenantId}",
                    tenantId);
                throw;
            }
        }

        public async Task UpdateAsync(TenantEncryptionKey tenantKey, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenantKey == null)
                {
                    throw new ArgumentNullException(nameof(tenantKey));
                }

                _context.TenantEncryptionKeys.Update(tenantKey);

                await Task.CompletedTask;

                _logger.LogInformation(
                    "Tenant encryption key marked for update successfully for TenantId: {TenantId}",
                    tenantKey.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating tenant encryption key for TenantId: {TenantId}",
                    tenantKey?.TenantId);
                throw;
            }
        }

         

        Task<TenantEncryptionKey?> ITenantEncryptionKeyRepository.GetActiveKeyByTenantIdAsync(long tenantId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}