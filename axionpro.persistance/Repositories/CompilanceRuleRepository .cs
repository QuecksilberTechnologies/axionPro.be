
using axionpro.application.DTOS.Compliances.ComplianceRule;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.infrastructure.Repositories
{
    public class CompilanceRuleRepository : ICompilanceRuleRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<CompilanceRuleRepository> _logger;

        public CompilanceRuleRepository(
            WorkforceDbContext context,
            ILogger<CompilanceRuleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 🔥 CORE ENGINE METHOD
        public async Task<ComplianceRule?> GetRuleAsync(
         int complianceTypeId,
         int countryId,
         int? stateId,
         long? tenantId,
         DateOnly effectiveFrom)
        {
            try
            {
                var query = _context.ComplianceRule
                    .Where(x =>
                        x.ComplianceTypeId == complianceTypeId &&
                        x.CountryId == countryId &&
                        (x.StateId == stateId || x.StateId == null) &&
                        (x.TenantId == tenantId || x.TenantId == null) &&
                        x.IsActive == true &&
                        x.IsSoftDeleted != true &&
                        x.EffectiveFrom <= effectiveFrom &&
                        (x.EffectiveTo == null || x.EffectiveTo >= effectiveFrom)
                    );

                var result = await query
                    .OrderByDescending(x => x.TenantId)   // tenant override
                    .ThenByDescending(x => x.StateId)     // state override
                    .ThenByDescending(x => x.Priority)    // priority
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching compliance rule");
                throw;
            }
        }
        public async Task<ComplianceRule> GetByIdAsync(long id)
        {
            return await _context.ComplianceRule
                .FirstOrDefaultAsync(x => x.Id == id && x.IsSoftDeleted != true);
        }

        public async Task AddAsync(ComplianceRule entity)
        {
            await _context.ComplianceRule.AddAsync(entity);
        }

        public async Task UpdateAsync(ComplianceRule entity)
        {
            _context.ComplianceRule.Update(entity);
        }

        public async Task<bool> ExistsAsync(int complianceTypeId, int countryId, int? stateId, long? tenantId, DateOnly effectiveFrom)
        {
            try
            {
                return await _context.ComplianceRule.AnyAsync(x =>
                    x.ComplianceTypeId == complianceTypeId &&
                    x.CountryId == countryId &&
                    x.StateId == stateId &&
                    x.TenantId == tenantId &&
                    x.EffectiveFrom == effectiveFrom &&
                    x.IsSoftDeleted != true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExistsAsync");
                throw;
            }
        }
    }
}