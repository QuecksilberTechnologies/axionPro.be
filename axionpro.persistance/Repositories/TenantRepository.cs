// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides Tenant persistence operations and Host-managed Tenant lifecycle staging.
// ================================================================

using axionpro.application.Interfaces.IRepositories;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.Pagination;
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

        #region Host Management

        /// <summary>
        /// Retrieves a filtered, paged, non-soft-deleted Tenant collection for Host-side management.
        /// </summary>
        /// <param name="request">The Host Tenant list filters and paging values.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The requested page of Tenant entities.</returns>
        public async Task<PagedResponseDTO<Tenant>> GetHostManagedTenantsAsync(
            GetAllTenantsRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize is > 0 and <= 100 ? request.PageSize : 10;
            var query = _context.Tenants
                .AsNoTracking()
                .Where(tenant => tenant.IsSoftDeleted != true);

            if (request.IsActive.HasValue)
            {
                query = query.Where(tenant => tenant.IsActive == request.IsActive.Value);
            }

            if (request.IsVerified.HasValue)
            {
                query = query.Where(tenant => tenant.IsVerified == request.IsVerified.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var searchPattern = $"%{request.SearchKeyword.Trim()}%";
                query = query.Where(tenant =>
                    EF.Functions.ILike(tenant.CompanyName, searchPattern) ||
                    (tenant.TenantCode != null && EF.Functions.ILike(tenant.TenantCode, searchPattern)) ||
                    EF.Functions.ILike(tenant.CompanyEmailDomain, searchPattern) ||
                    EF.Functions.ILike(tenant.TenantEmail, searchPattern) ||
                    (tenant.ContactPersonName != null && EF.Functions.ILike(tenant.ContactPersonName, searchPattern)) ||
                    (tenant.ContactNumber != null && EF.Functions.ILike(tenant.ContactNumber, searchPattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var data = await query
                .OrderByDescending(tenant => tenant.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponseDTO<Tenant>(data, totalCount, pageNumber, pageSize)
            {
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        /// <summary>
        /// Retrieves a tracked, non-soft-deleted Tenant for a Host-managed operation.
        /// </summary>
        /// <param name="tenantId">The authoritative Tenant identifier.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The Tenant entity, or <see langword="null"/> when unavailable.</returns>
        public Task<Tenant?> GetHostManagedTenantByIdAsync(long tenantId, CancellationToken cancellationToken = default)
        {
            return _context.Tenants.FirstOrDefaultAsync(
                tenant => tenant.Id == tenantId && tenant.IsSoftDeleted != true,
                cancellationToken);
        }

        /// <summary>
        /// Determines whether another non-soft-deleted Tenant uses the supplied email address.
        /// </summary>
        /// <param name="tenantEmail">The email address to check.</param>
        /// <param name="excludedTenantId">The current Tenant excluded from the check.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns><see langword="true"/> when another Tenant uses the email.</returns>
        public Task<bool> IsTenantEmailInUseAsync(
            string tenantEmail,
            long excludedTenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.Tenants
                .AsNoTracking()
                .AnyAsync(
                    tenant =>
                        tenant.Id != excludedTenantId &&
                        tenant.IsSoftDeleted != true &&
                        tenant.TenantEmail == tenantEmail,
                    cancellationToken);
        }

        /// <summary>
        /// Determines whether another non-soft-deleted Tenant uses the supplied Tenant code.
        /// </summary>
        /// <param name="tenantCode">The Tenant code to check.</param>
        /// <param name="excludedTenantId">The current Tenant excluded from the check.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns><see langword="true"/> when another Tenant uses the code.</returns>
        public Task<bool> IsTenantCodeInUseAsync(
            string tenantCode,
            long excludedTenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.Tenants
                .AsNoTracking()
                .AnyAsync(
                    tenant =>
                        tenant.Id != excludedTenantId &&
                        tenant.IsSoftDeleted != true &&
                        tenant.TenantCode == tenantCode,
                    cancellationToken);
        }

        /// <summary>
        /// Retrieves the legitimate onboarding credential and Employee required by the existing Tenant welcome-email flow.
        /// </summary>
        /// <param name="tenantId">The Tenant identifier.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The onboarding credential with its Employee, or <see langword="null"/> when unavailable.</returns>
        public Task<LoginCredential?> GetTenantOnboardingCredentialAsync(
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.LoginCredentials
                .AsNoTracking()
                .Include(credential => credential.Employee)
                .Where(credential =>
                    credential.TenantId == tenantId &&
                    credential.IsOnboard &&
                    credential.IsSoftDeleted != true)
                .OrderBy(credential => credential.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Stages a Host-managed Tenant update prepared by the handler.
        /// </summary>
        /// <param name="tenant">The tracked Tenant entity with validated editable changes.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        public Task StageHostManagedUpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tenant);
            _context.Tenants.Update(tenant);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stages a Tenant status transition and synchronizes valid, non-soft-deleted credentials.
        /// </summary>
        /// <param name="tenant">The tracked Tenant whose active state was prepared by the handler.</param>
        /// <param name="hostUserId">The validated Host user used for credential audit fields.</param>
        /// <param name="utcNow">The single UTC audit timestamp captured for the Host request.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        public async Task SynchronizeTenantStatusAsync(
            Tenant tenant,
            long hostUserId,
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tenant);

            _context.Tenants.Update(tenant);

            var credentials = await _context.LoginCredentials
                .Where(credential =>
                    credential.TenantId == tenant.Id &&
                    credential.IsSoftDeleted != true)
                .ToListAsync(cancellationToken);

            foreach (var credential in credentials)
            {
                credential.IsActive = tenant.IsActive;
                credential.UpdatedById = hostUserId;
                credential.UpdatedDateTime = utcNow;
            }
        }

        /// <summary>
        /// Stages Tenant soft deletion and deactivates all related login credentials without deleting historical records.
        /// </summary>
        /// <param name="tenant">The tracked Tenant prepared for soft deletion.</param>
        /// <param name="hostUserId">The validated Host user used for credential audit fields.</param>
        /// <param name="utcNow">The single UTC audit timestamp captured for the Host request.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        public async Task SoftDeleteTenantAndDeactivateCredentialsAsync(
            Tenant tenant,
            long hostUserId,
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tenant);

            _context.Tenants.Update(tenant);

            var credentials = await _context.LoginCredentials
                .Where(credential => credential.TenantId == tenant.Id)
                .ToListAsync(cancellationToken);

            foreach (var credential in credentials)
            {
                // Preserve each credential's existing soft-delete state while deactivating access.
                credential.IsActive = false;
                credential.UpdatedById = hostUserId;
                credential.UpdatedDateTime = utcNow;
            }
        }

        #endregion
    }
}
