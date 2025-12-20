using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;

        public TenantRepository(WorkforceDbContext? context, ILogger<TenantRepository>? logger, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<List<Tenant>> GetAllTenantBySubscriptionIdAsync(Tenant tenant)
        {
            try
            {
                // ✅ Null check
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant filter is null while fetching tenants.");
                    return new List<Tenant>();
                }
                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Build base query
                IQueryable<Tenant> query = context.Tenants.Where(x => x.IsActive == true);

                // ✅ Apply dynamic filters based on non-null or non-empty values
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

              //  query = query.Where(x => x.IsActive == tenant.IsActive);
                //query = query.Where(x => x.IsVerified == tenant.IsVerified);

                // ✅ Execute query
                var result = await query.ToListAsync();

                _logger.LogInformation("Fetched {Count} tenants matching the criteria.", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenant list.");
                return new List<Tenant>();
            }
        }


        public async Task<long> AddTenantAsync(Tenant tenant)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                tenant.AddedById = tenant.Id;
                tenant.IsActive = true;
                tenant.AddedDateTime = DateTime.Now;
                if (context == null)
                {
                    _logger?.LogError("DbContext is null in AddAsync.");
                    throw new ArgumentNullException(nameof(context), "DbContext is not initialized.");
                }

                await context.Tenants.AddAsync(tenant); // `Tenants` is DbSet<Tenant>
                await context.SaveChangesAsync(); // Save changes to DB

                _logger?.LogInformation("Tenant created successfully with ID: {TenantId}", tenant.Id);

                return tenant.Id; // Tenant ID generated after SaveChanges
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while adding tenant.");
                throw; // Re-throw for handler to rollback transaction
            }
        }

        public async Task<long> AddTenantProfileAsync(TenantProfile tenantProfile)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                tenantProfile.Id = 0;
                if (context == null)
                {
                    _logger?.LogError("DbContext is null in AddAsync.");
                    throw new ArgumentNullException(nameof(context), "DbContext is not initialized.");
                }

                await context.TenantProfiles.AddAsync(tenantProfile); // `Tenants` is DbSet<Tenant>
                await context.SaveChangesAsync(); // Save changes to DB

                _logger?.LogInformation("TenantProfile created successfully with ID: {TenantId}", tenantProfile.Id);

                return tenantProfile.Id; // Tenant ID generated after SaveChanges
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while adding tenant.");
                throw; // Re-throw for handler to rollback transaction
            }
        }

        public async Task<bool> CheckTenantByEmail(string email)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                return await Task.FromResult(context.Tenants.Any(t => t.TenantEmail == email));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking tenant email existence.");
                throw;
            }
        }

        public Task DeleteTenantAsync(Tenant tenant)
        {
            throw new NotImplementedException();
        }

      

        public Task<Tenant> GetByCodeAsync(string tenantCode)
        {
            throw new NotImplementedException();
        }

        public async Task<Tenant> GetByIdAsync(long? id, bool i)
        {


            var tenant = await _context.Tenants
              .Where(t => t.Id == id
                       && (t.IsActive == i && t.IsSoftDeleted!=true))
              .FirstOrDefaultAsync();
 

            return tenant;
        }





        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            try
            {
                var existingTenant = await _context.Tenants
                    .FirstOrDefaultAsync(x => x.Id == tenant.Id);

                if (existingTenant == null)
                {
                    return null; // Tenant not found
                }

                existingTenant.IsActive = true;
                existingTenant.IsVerified = true;

                _context.Tenants.Update(existingTenant);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    // Update successful
                    return existingTenant;
                }
                else
                {
                    // Nothing updated (maybe values were already true)
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Optional: Agar aapke paas ILogger<TenantRepository> injected hai to use yahan log kar sakte ho
                Console.WriteLine($"Error while updating tenant: {ex.Message}");

                // Fail-safe: return null ya custom exception throw kar sakte ho
                return null;
            }
        }



    }
}
