using AutoMapper;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class SandwitchRuleRepository : ISandwitchRuleRepository
    {
        private readonly IMapper _mapper;
        private readonly WorkforceDbContext _context;
       
        private readonly ILogger<SandwitchRuleRepository> _logger;

        public SandwitchRuleRepository(
            IMapper mapper,
            WorkforceDbContext context,
            
            ILogger<SandwitchRuleRepository> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
           
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ------------------ ADD DAY COMBINATION ------------------
        public async Task<IEnumerable<GetDayCombinationResponseDTO>> AddDayCombinationAsync(CreateDayCombinationRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            

            try
            {
                var entity = _mapper.Map<DayCombination>(request);
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                await _context.DayCombinations.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ DayCombination added successfully: {CombinationName}", entity.CombinationName);

                return await GetAllActiveDayCombinationsAsync(request.TenantId, request.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding DayCombination: {CombinationName}", request.CombinationName);
                throw;
            }
        }

        // ------------------ ADD SANDWICH RULE ------------------
        public async Task<IEnumerable<GetLeaveSandwitchRuleResponseDTO>> AddSandwichAsync(CreateLeaveSandwichRuleRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

           

            try
            {
                var entity = _mapper.Map<LeaveSandwichRule>(request);
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                await _context.SandwitchRules.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Leave Sandwich Rule added successfully: {RuleName}", entity.RuleName);

                return await GetAllActiveSandwichRulesAsync(request.TenantId, request.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding Leave Sandwich Rule: {RuleName}", request.RuleName);
                throw;
            }
        }

        // ------------------ DELETE DAY COMBINATION ------------------
        public async Task<bool> DeleteDayCombinationAsync(DeleteDayCombinationRequestDTO request)
        {
            if (request == null || request.Id <= 0)
                return false;

         

            try
            {
                var existingCombination = await _context.DayCombinations
                    .FirstOrDefaultAsync(dc => dc.Id == request.Id && dc.TenantId == request.TenantId && dc.IsSoftDeleted != true);

                if (existingCombination == null)
                {
                    _logger.LogWarning("⚠️ No DayCombination found with Id: {Id}", request.Id);
                    return false;
                }

                existingCombination.IsSoftDeleted = true;
                existingCombination.IsActive = false;
                existingCombination.SoftDeletedById = request.EmployeeId;
                existingCombination.SoftDeletedDateTime = DateTime.UtcNow;

                _context.Update(existingCombination);
                await _context.SaveChangesAsync();

                _logger.LogInformation("🗑️ DayCombination soft-deleted successfully: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting DayCombination: {Id}", request.Id);
                return false;
            }
        }

        // ------------------ DELETE SANDWICH RULE ------------------
        public async Task<bool> DeleteSandwichAsync(DeleteLeaveSandwitchRuleRequestDTO request)
        {
            if (request == null || request.Id <= 0)
                return false;

          

            try
            {
                var existingRule = await _context.SandwitchRules
                    .FirstOrDefaultAsync(r => r.Id == request.Id && r.TenantId == request.TenantId && r.IsSoftDeleted != true);

                if (existingRule == null)
                {
                    _logger.LogWarning("⚠️ No Leave Sandwich Rule found with Id: {Id}", request.Id);
                    return false;
                }

                existingRule.IsSoftDeleted = true;
                existingRule.IsActive = false;
                existingRule.SoftDeletedById = request.EmployeeId;
                existingRule.SoftDeletedDateTime = DateTime.UtcNow;

                _context.Update(existingRule);
                await _context.SaveChangesAsync();

                _logger.LogInformation("🗑️ Leave Sandwich Rule soft-deleted successfully: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting Leave Sandwich Rule: {Id}", request.Id);
                return false;
            }
        }

        // ------------------ GET ALL DAY COMBINATIONS ------------------
        public async Task<IEnumerable<GetDayCombinationResponseDTO>> GetAllActiveDayCombinationsAsync(long tenantId, bool isActive)
        { 
            try
            {
                var entities = await _context.DayCombinations
                    .Where(x => x.TenantId == tenantId && x.IsActive == isActive && (x.IsSoftDeleted == false || x.IsSoftDeleted == null))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<GetDayCombinationResponseDTO>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching DayCombinations for TenantId: {TenantId}", tenantId);
                throw;
            }
        }

        // ------------------ GET ALL SANDWICH RULES ------------------
        public async Task<IEnumerable<GetLeaveSandwitchRuleResponseDTO>> GetAllActiveSandwichRulesAsync(long tenantId, bool isActive)
        {
            

            try
            {
                var entities = await _context.SandwitchRules
                    .Where(x => x.TenantId == tenantId && x.IsActive == isActive && (x.IsSoftDeleted == false || x.IsSoftDeleted == null))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} Sandwich Rules for TenantId: {TenantId}", entities.Count, tenantId);

                return _mapper.Map<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching Sandwich Rules for TenantId: {TenantId}", tenantId);
                throw;
            }
        }

        // ------------------ UPDATE DAY COMBINATION ------------------
        public async Task<bool> UpdateDayCombinationAsync(UpdateDayCombinationRequestDTO request)
        {
             

            try
            {
                var existing = await _context.DayCombinations
                    .FirstOrDefaultAsync(dc => dc.Id == request.Id && dc.TenantId == request.TenantId);

                if (existing == null)
                    return false;

                _mapper.Map(request, existing);
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ DayCombination updated successfully: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating DayCombination: {Id}", request.Id);
                return false;
            }
        }

        // ------------------ UPDATE SANDWICH RULE ------------------
        public async Task<bool> UpdateSandwichAsync(UpdateLeaveSandwitchRuleRequestDTO request)
        {
           

            try
            {
                var existing = await _context.SandwitchRules
                    .FirstOrDefaultAsync(r => r.Id == request.Id && r.TenantId == request.TenantId);

                if (existing == null)
                    return false;

                _mapper.Map(request, existing);
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Leave Sandwich Rule updated successfully: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating Leave Sandwich Rule: {Id}", request.Id);
                return false;
            }
        }
    }
}
