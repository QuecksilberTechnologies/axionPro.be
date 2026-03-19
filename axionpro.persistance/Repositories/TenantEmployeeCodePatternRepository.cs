using axionpro.application.DTOS.Tenant;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class TenantEmployeeCodePatternRepository : ITenantEmployeeCodePatternRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantEmployeeCodePatternRepository> _logger;
       

        public TenantEmployeeCodePatternRepository(
            WorkforceDbContext context,
            ILogger<TenantEmployeeCodePatternRepository> logger
             )
        {
            _context = context;
            _logger = logger;
            
        }

        public async Task<GetEmployeeCodePatternResponseDTO?> GetTenantEmployeeCodePatternAsync(long tenantId, bool isActive)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching employee code pattern for TenantId: {TenantId}", tenantId);

                var tenantParam = new NpgsqlParameter("p_tenantid", tenantId);

                var result = await _context
     .Set<GetEmployeeCodePatternResponseDTO>()
     .FromSqlRaw(
         @"SELECT * FROM axionpro.""GetEmployeeCodePatternByTenant""(@p_tenantid)",
         tenantParam)
     .AsNoTracking()
     .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching employee code pattern for TenantId: {TenantId}",
                    tenantId);

                return null;
            }
        }

        // ============================================================
        // 2️⃣ CREATE PATTERN
        // ============================================================
        public async Task<bool> CreatePatternAsync(EmployeeCodePattern entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                await _context.EmployeeCodePatterns.AddAsync(entity);

                _logger.LogInformation(
                    "Employee code pattern added to DbContext for TenantId: {TenantId}",
                    entity.TenantId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while adding employee code pattern to DbContext for TenantId: {TenantId}",
                    entity?.TenantId);
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
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _context.EmployeeCodePatterns.Update(entity);

                _logger.LogInformation(
                    "Employee code pattern marked for update for PatternId: {PatternId}",
                    entity.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while updating employee code pattern for PatternId: {PatternId}",
                    entity?.Id);
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

                _logger.LogInformation(
                    "Existing active employee code patterns marked inactive for TenantId: {TenantId}",
                    tenantId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deactivating existing patterns for TenantId: {TenantId}",
                    tenantId);
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
                pattern.UpdatedDateTime = DateTime.UtcNow;

                _logger.LogInformation(
                    "Employee code running number incremented in DbContext for TenantId: {TenantId}, NextNumber: {NextNumber}",
                    tenantId,
                    pattern.LastUsedNumber);

                return pattern.LastUsedNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error incrementing running number for TenantId: {TenantId}",
                    tenantId);
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

                pattern.LastUsedNumber += 1;
                pattern.UpdatedDateTime = DateTime.UtcNow;

                int nextSeq = pattern.LastUsedNumber;

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

                string employeeCode = string.Join(pattern.Separator, parts);

                _logger.LogInformation(
                    "Employee code generated in DbContext for TenantId: {TenantId}, EmployeeCode: {EmployeeCode}",
                    tenantId,
                    employeeCode);

                return employeeCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating employee code for TenantId: {TenantId}",
                    tenantId);
                throw;
            }
        }
    }

}
