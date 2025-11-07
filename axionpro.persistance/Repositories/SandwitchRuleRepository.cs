using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Features.ReportTypeCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class SandwitchRuleRepository : ISandwitchRuleRepository
    {
        private readonly IMapper _mapper;
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly ILogger<SandwitchRuleRepository> _logger;

        public SandwitchRuleRepository(
            IMapper mapper,
            WorkforceDbContext context,
            IDbContextFactory<WorkforceDbContext> contextFactory,
            ILogger<SandwitchRuleRepository> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ------------------ ADD DAY COMBINATION ------------------
        public async Task<IEnumerable<GetDayCombinationResponseDTO>> AddDayCombinationAsync(CreateDayCombinationRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var entity = _mapper.Map<DayCombination>(request);
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                await context.DayCombinations.AddAsync(entity);
                await context.SaveChangesAsync();

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

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var entity = _mapper.Map<LeaveSandwichRule>(request);
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                await context.SandwitchRules.AddAsync(entity);
                await context.SaveChangesAsync();

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

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var existingCombination = await context.DayCombinations
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

                context.Update(existingCombination);
                await context.SaveChangesAsync();

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

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var existingRule = await context.SandwitchRules
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

                context.Update(existingRule);
                await context.SaveChangesAsync();

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
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var entities = await context.DayCombinations
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
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var entities = await context.SandwitchRules
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
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var existing = await context.DayCombinations
                    .FirstOrDefaultAsync(dc => dc.Id == request.Id && dc.TenantId == request.TenantId);

                if (existing == null)
                    return false;

                _mapper.Map(request, existing);
                existing.UpdatedDateTime = DateTime.UtcNow;

                context.Update(existing);
                await context.SaveChangesAsync();

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
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var existing = await context.SandwitchRules
                    .FirstOrDefaultAsync(r => r.Id == request.Id && r.TenantId == request.TenantId);

                if (existing == null)
                    return false;

                _mapper.Map(request, existing);
                existing.UpdatedDateTime = DateTime.UtcNow;

                context.Update(existing);
                await context.SaveChangesAsync();

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
