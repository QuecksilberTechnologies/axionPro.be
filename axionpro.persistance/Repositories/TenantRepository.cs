using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantRepository> _logger;

        public TenantRepository(
            WorkforceDbContext context,
            ILogger<TenantRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Tenant>> GetAllTenantBySubscriptionIdAsync(
            Tenant tenant,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant filter is null while fetching tenants.");
                    return new List<Tenant>();
                }

                IQueryable<Tenant> query = _context.Tenants
                    .AsNoTracking()
                    .Where(x => x.IsActive == true && x.IsSoftDeleted != true);

                if (!string.IsNullOrWhiteSpace(tenant.CompanyName))
                    query = query.Where(x => x.CompanyName.Contains(tenant.CompanyName));

                if (!string.IsNullOrWhiteSpace(tenant.CompanyEmailDomain))
                    query = query.Where(x => x.CompanyEmailDomain.Contains(tenant.CompanyEmailDomain));

                if (!string.IsNullOrWhiteSpace(tenant.TenantEmail))
                    query = query.Where(x => x.TenantEmail.Contains(tenant.TenantEmail));

                if (!string.IsNullOrWhiteSpace(tenant.ContactPersonName))
                    query = query.Where(x => x.ContactPersonName.Contains(tenant.ContactPersonName));

                if (!string.IsNullOrWhiteSpace(tenant.ContactNumber))
                    query = query.Where(x => x.ContactNumber.Contains(tenant.ContactNumber));

                if (!string.IsNullOrWhiteSpace(tenant.TenantCode))
                    query = query.Where(x => x.TenantCode.Contains(tenant.TenantCode));

                if (tenant.CountryId > 0)
                    query = query.Where(x => x.CountryId == tenant.CountryId);

                var result = await query.ToListAsync(cancellationToken);

                _logger.LogInformation("Fetched {Count} tenants matching the criteria.", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenant list.");
                throw;
            }
        }

        public async Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant entity is null in AddTenantAsync.");
                    throw new ArgumentNullException(nameof(tenant));
                }

                tenant.IsActive = true;
                tenant.AddedDateTime = DateTime.UtcNow;

                // IMPORTANT:
                // Do not set AddedById = tenant.Id here because Id is not generated yet.
                // Set AddedById in handler if a valid creator id exists.

                await _context.Tenants.AddAsync(tenant, cancellationToken);

                _logger.LogInformation("Tenant added to DbContext successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding tenant.");
                throw;
            }
        }

        public async Task AddTenantProfileAsync(TenantProfile tenantProfile, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenantProfile == null)
                {
                    _logger.LogWarning("TenantProfile entity is null in AddTenantProfileAsync.");
                    throw new ArgumentNullException(nameof(tenantProfile));
                }

                await _context.TenantProfiles.AddAsync(tenantProfile, cancellationToken);

                _logger.LogInformation("TenantProfile added to DbContext successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding tenant profile.");
                throw;
            }
        }

        public async Task<bool> CheckTenantByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                return await _context.Tenants
                    .AsNoTracking()
                    .AnyAsync(t => t.TenantEmail == email, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tenant email existence.");
                throw;
            }
        }

        public async Task DeleteTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenant == null)
                {
                    throw new ArgumentNullException(nameof(tenant));
                }

                _context.Tenants.Remove(tenant);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting tenant.");
                throw;
            }
        }

        public async Task<Tenant?> GetByCodeAsync(string tenantCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantCode))
                {
                    return null;
                }

                return await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.TenantCode == tenantCode &&
                             x.IsActive == true &&
                             x.IsSoftDeleted != true,
                        cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tenant by code.");
                throw;
            }
        }

        public async Task<Tenant?> GetByIdAsync(long? id, bool isActive)
        {
            try
            {
                if (!id.HasValue || id.Value <= 0)
                {
                    return null;
                }

                return await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.Id == id.Value &&
                        t.IsActive == isActive &&
                        t.IsSoftDeleted != true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tenant by id.");
                throw;
            }
        }

        public async Task<Tenant?> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenant == null)
                {
                    throw new ArgumentNullException(nameof(tenant));
                }

                var existingTenant = await _context.Tenants
                    .FirstOrDefaultAsync(x => x.Id == tenant.Id, cancellationToken);

                if (existingTenant == null)
                {
                    _logger.LogWarning("Tenant not found for update. TenantId: {TenantId}", tenant.Id);
                    return null;
                }

                existingTenant.IsActive = tenant.IsActive;
                existingTenant.IsVerified = tenant.IsVerified;
                existingTenant.UpdatedDateTime = DateTime.UtcNow;
                existingTenant.UpdatedById = tenant.UpdatedById;

                _context.Tenants.Update(existingTenant);

                _logger.LogInformation("Tenant updated in DbContext successfully. TenantId: {TenantId}", tenant.Id);

                return existingTenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating tenant. TenantId: {TenantId}", tenant?.Id);
                throw;
            }
        }
    }
}