using axionpro.application.DTOS.Tenant;
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
    public class TenantEmployeeCodePatternRepository : ITenantEmployeeCodePatternRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantEmployeeCodePatternRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;

        public TenantEmployeeCodePatternRepository(
            WorkforceDbContext context,
            ILogger<TenantEmployeeCodePatternRepository> logger,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<GetEmployeeCodePatternResponseDTO?> GetTenantEmployeeCodePatternAsync(long tenantId, bool isActive)
        {
            try
            {
                await using var db = await _contextFactory.CreateDbContextAsync();

                var result = (await db
                    .Set<GetEmployeeCodePatternResponseDTO>()
                    .FromSqlRaw("EXEC [AxionPro].[GetEmployeeCodePatternByTenant] @TenantId = {0}", tenantId)
                    .AsNoTracking()
                    .ToListAsync())
                    .FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching employee code pattern for TenantId {TenantId}", tenantId);
                throw;
            }
        }


        // ============================================================
        // 2️⃣ CREATE PATTERN
        // ============================================================
        public async Task<bool> CreatePatternAsync(EmployeeCodePattern entity)
        {
            try
            {
                await _context.EmployeeCodePatterns.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating pattern for TenantId {TenantId}", entity.TenantId);
                throw;
            }
        }

        // ============================================================
        // 3️⃣ UPDATE PATTERN
        // ============================================================
        public async Task<bool> UpdatePatternAsync(EmployeeCodePattern entity)
        {
            try
            {
                _context.EmployeeCodePatterns.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating employee code pattern Id {Id}", entity.Id);
                throw;
            }
        }

        // ============================================================
        // 4️⃣ DEACTIVATE ALL OTHER PATTERNS (only 1 active allowed)
        // ============================================================
        public async Task<bool> DeactivateExistingPatternsAsync(long tenantId)
        {
            try
            {
                var activePatterns = await _context.EmployeeCodePatterns
                    .Where(x => x.TenantId == tenantId && x.IsActive)
                    .ToListAsync();

                if (!activePatterns.Any())
                    return true;

                foreach (var p in activePatterns)
                {
                    p.IsActive = false;
                    p.UpdatedDateTime = DateTime.UtcNow;
                }

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deactivating existing patterns for TenantId {TenantId}", tenantId);
                throw;
            }
        }

        // ============================================================
        // 5️⃣ ATOMIC SEQUENCE INCREMENT (Transaction Safe)
        // ============================================================
        public async Task<int> IncrementAndGetNextRunningNumberAsync(long tenantId)
        {
            
            try
            {
                var pattern = await _context.EmployeeCodePatterns
                    .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);

                if (pattern == null)
                    throw new Exception("Active employee code pattern not found.");

                pattern.LastUsedNumber += 1;
                int next = pattern.LastUsedNumber;

                await _context.SaveChangesAsync();
               

                return next;
            }
            catch (Exception ex)
            {
             
                _logger.LogError(ex, "❌ Error incrementing running number for TenantId {TenantId}", tenantId);
                throw;
            }
        }

        // ============================================================
        // 6️⃣ GENERATE FINAL EMPLOYEE CODE + UPDATE LastUsedNumber
        // ============================================================
        public async Task<string> GenerateEmployeeCodeAsync(long tenantId, int? departmentId = null)
        {
            try
            {
                var pattern = await _context.EmployeeCodePatterns
                    .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);

                if (pattern == null)
                    throw new Exception("Employee code pattern not configured for tenant.");

                // SINGLE INCREMENT HERE (Correct)
                pattern.LastUsedNumber += 1;
                int nextSeq = pattern.LastUsedNumber;

                pattern.UpdatedDateTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                List<string> parts = new();

                if (!string.IsNullOrWhiteSpace(pattern.Prefix))
                    parts.Add(pattern.Prefix);

                if (pattern.IncludeYear)
                    parts.Add(DateTime.UtcNow.Year.ToString());

                if (pattern.IncludeMonth)
                    parts.Add(DateTime.UtcNow.ToString("MMM").ToUpper());

                if (pattern.IncludeDepartment && departmentId.HasValue)
                    parts.Add(departmentId.Value.ToString());

                parts.Add(nextSeq.ToString().PadLeft(pattern.RunningNumberLength, '0'));

                return string.Join(pattern.Separator, parts);
            }
            catch
            {
                throw;
            }
        }

    }

}
