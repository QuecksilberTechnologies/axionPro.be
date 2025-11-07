using axionpro.application.Constants;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class LeaveRuleRepository : ILeaveRuleRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<LeaveRuleRepository> _logger;

        public LeaveRuleRepository(WorkforceDbContext context, ILogger<LeaveRuleRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
         
         
       
        public async Task<LeaveRule?> GetLeaveRuleByIdAsync(long leaveRuleId)
        {
            try
            {
                var existing = await _context.LeaveRules
                    .FirstOrDefaultAsync(lr => lr.Id == leaveRuleId                                           
                                               && !lr.IsSoftDeleted);

                return existing;
            }
            catch (Exception ex)
            {
                // log error agar logger available ho
                Console.WriteLine($"Error fetching LeaveRule: {ex.Message}");
                return null;
            }
        }

     
    

        public async Task<bool> DeleteLeaveRuleAsync(LeaveRule leaveRule)
        {
            if (leaveRule == null)
                throw new ArgumentNullException(nameof(leaveRule), "Leave rule cannot be null.");

            try
            {
                // 1️⃣ Already deleted check
                if (leaveRule.IsSoftDeleted == ConstantValues.IsByDefaultTrue)
                {
                    _logger.LogWarning("⚠️ Leave rule with ID {LeaveId} is already soft-deleted.", leaveRule.Id);
                    return false;
                }

                // 2️⃣ Soft Delete set
                leaveRule.IsSoftDeleted = true;
                leaveRule.SoftDeleteDateTime = DateTime.UtcNow;

                // 3️⃣ DB update
                _context.LeaveRules.Update(leaveRule);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Leave rule with ID {LeaveId} soft-deleted by UserId {UserId}.",
                    leaveRule.Id,
                    leaveRule.SoftDeleteById
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error occurred while deleting Leave rule with ID {LeaveId} by UserId {UserId}.",
                    leaveRule?.Id,
                    leaveRule?.SoftDeleteById
                );
                throw; // Exception propagate kar do
            }
        }

     
        //}

     
        public async Task<List<GetLeaveRuleResponseDTO>> CreateLeaveRuleAsync(LeaveRule leaveRule)
        {
            try
            {
                // 🔹 Basic validation
                if (leaveRule == null)
                    throw new ArgumentNullException(nameof(leaveRule), "LeaveRule object cannot be null.");

                if (leaveRule.PolicyLeaveTypeId <= 0)
                    throw new ArgumentException("Invalid LeavePolicyId.");

                if (leaveRule.TenantId <= 0)
                    throw new ArgumentException("Invalid TenantId.");

                // 🔹 Set system fields
                leaveRule.AddedDateTime = DateTime.UtcNow;
                leaveRule.IsActive = true;

                // 🔹 Add to DbContext
                await _context.LeaveRules.AddAsync(leaveRule);

                // 🔹 Save changes
                await _context.SaveChangesAsync();
                GetLeaveRuleRequestDTO dTO = new GetLeaveRuleRequestDTO();
                   dTO.TenantId = leaveRule.TenantId;
                   dTO.IsActive = true;

                // 🔹 Return updated list (optimized single DB call)
                return await GetLeaveRuleAsync(dTO);
                    
            }
            catch (DbUpdateException dbEx)
            {
                // SQL/EF related errors
                _logger.LogError(dbEx, "Database update failed while creating LeaveRule.");
                throw new Exception("Database update failed. Please check logs for details.", dbEx);
            }
            catch (Exception ex)
            {
                // General errors
                _logger.LogError(ex, "Error occurred while creating LeaveRule.");
                throw;
            }
        }


        public async Task<LeaveRule?> UpdateLeaveRuleAsync(LeaveRule leaveRule, long userId)
        {
            try
            {
                var existing = await _context.LeaveRules.FirstOrDefaultAsync(lr => lr.PolicyLeaveTypeId == leaveRule.PolicyLeaveTypeId
                                 && lr.TenantId == leaveRule.TenantId
                                 && !lr.IsSoftDeleted);

                if (existing == null)
                    return null;

                existing.TenantId = leaveRule.TenantId;
                existing.PolicyLeaveTypeId = leaveRule.PolicyLeaveTypeId;
                existing.ApplySandwichRule = leaveRule.ApplySandwichRule;
                existing.IsHalfDayAllowed = leaveRule.IsHalfDayAllowed;
                existing.HalfDayNoticeHours = leaveRule.HalfDayNoticeHours;
                existing.NoticePeriodDays = leaveRule.NoticePeriodDays;
                existing.MaxContinuousLeaves = leaveRule.MaxContinuousLeaves;
                existing.MinGapBetweenLeaves = leaveRule.MinGapBetweenLeaves;
                existing.IsActive = leaveRule.IsActive;
                existing.IsSoftDeleted = ConstantValues.IsByDefaultFalse;
                existing.Remark = leaveRule.Remark;
                existing.UpdatedById = userId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.LeaveRules.Update(existing);
                await _context.SaveChangesAsync();

                return existing;
            }
            catch (DbUpdateException dbEx)
            {
                // database update errors (constraint violation, etc.)
                Console.WriteLine($"DB Update Error: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // any other error
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<GetLeaveRuleResponseDTO>> GetLeaveRuleAsync(GetLeaveRuleRequestDTO dTO)
        {
            if (dTO == null)
                throw new ArgumentNullException(nameof(dTO));

            try
            {
                var rules = await (from lr in _context.LeaveRules
                                   join plm in _context.PolicyLeaveTypeMappings
                                       on lr.Id equals plm.PolicyTypeId
                                   join lt in _context.LeaveTypes
                                       on plm.LeaveTypeId equals lt.Id
                                   where lr.TenantId == dTO.TenantId
                                         && lr.IsActive == true
                                         && (lr.IsSoftDeleted == false || lr.IsSoftDeleted == null)
                                         && plm.IsActive == true
                                         && (plm.IsSoftDeleted == false || plm.IsSoftDeleted == null)
                                         && lt.IsActive == true
                                         && (lt.IsSoftDeleted == false || lt.IsSoftDeleted == null)
                                         
                                   select new GetLeaveRuleResponseDTO
                                   {
                                       Id = lr.Id,
                                       TenantId = lr.TenantId,
                                       PolicyLeaveTypeId = plm.Id,
                                       LeaveTypeId = lt.Id,
                                       LeaveName = lt.LeaveName,
                                       ApplySandwichRule = lr.ApplySandwichRule,
                                       IsHalfDayAllowed = lr.IsHalfDayAllowed,
                                       HalfDayNoticeHours = lr.HalfDayNoticeHours,
                                       NoticePeriodDays = lr.NoticePeriodDays,
                                       MaxContinuousLeaves = lr.MaxContinuousLeaves,
                                       MinGapBetweenLeaves = lr.MinGapBetweenLeaves,
                                       IsActive = lr.IsActive,                                       
                                       Remark = lr.Remark,
                                       AddedById = lr.AddedById,
                                       AddedDateTime = lr.AddedDateTime,
                                       UpdatedById = lr.UpdatedById,
                                       UpdatedDateTime = lr.UpdatedDateTime
                                   }).ToListAsync();

                if (rules == null || rules.Count == 0)
                {
                    _logger?.LogWarning("⚠️ No LeaveRule found for TenantId: {TenantId}", dTO.TenantId);
                    return new List<GetLeaveRuleResponseDTO>();
                }

                _logger?.LogInformation("Fetched {Count} LeaveRules for TenantId: {TenantId}", rules.Count, dTO.TenantId);
                return rules;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error fetching LeaveRules for TenantId: {TenantId}", dTO.TenantId);
                throw;
            }
        }


        public async Task<List<GetLeaveRuleResponseDTO>> GetLeaveRuleByIsSandwichAsync(GetLeaveRuleRequestDTO dTO)
        {
            if (dTO == null)
                throw new ArgumentNullException(nameof(dTO));

            try
            {
                var rules = await (from lr in _context.LeaveRules
                                   join plm in _context.PolicyLeaveTypeMappings
                                       on lr.Id equals plm.PolicyTypeId
                                   join lt in _context.LeaveTypes
                                       on plm.LeaveTypeId equals lt.Id
                                   where lr.TenantId == dTO.TenantId
                                         && lr.IsActive == true
                                         && (lr.IsSoftDeleted == false || lr.IsSoftDeleted == null)
                                         && plm.IsActive == true
                                         && (plm.IsSoftDeleted == false || plm.IsSoftDeleted == null)
                                         && lt.IsActive == true
                                         && (lt.IsSoftDeleted == false || lt.IsSoftDeleted == null)
                                         &&(lr.ApplySandwichRule == true)
                                   select new GetLeaveRuleResponseDTO
                                   {
                                       Id = lr.Id,
                                       TenantId = lr.TenantId,
                                       PolicyLeaveTypeId = plm.Id,
                                       LeaveTypeId = lt.Id,
                                       LeaveName = lt.LeaveName,
                                       ApplySandwichRule = lr.ApplySandwichRule,
                                       IsHalfDayAllowed = lr.IsHalfDayAllowed,
                                       HalfDayNoticeHours = lr.HalfDayNoticeHours,
                                       NoticePeriodDays = lr.NoticePeriodDays,
                                       MaxContinuousLeaves = lr.MaxContinuousLeaves,
                                       MinGapBetweenLeaves = lr.MinGapBetweenLeaves,
                                       IsActive = lr.IsActive,
                                       Remark = lr.Remark,
                                       AddedById = lr.AddedById,
                                       AddedDateTime = lr.AddedDateTime,
                                       UpdatedById = lr.UpdatedById,
                                       UpdatedDateTime = lr.UpdatedDateTime
                                   }).ToListAsync();

                if (rules == null || rules.Count == 0)
                {
                    _logger?.LogWarning("⚠️ No LeaveRule found for TenantId: {TenantId}", dTO.TenantId);
                    return new List<GetLeaveRuleResponseDTO>();
                }

                _logger?.LogInformation("Fetched {Count} LeaveRules for TenantId: {TenantId}", rules.Count, dTO.TenantId);
                return rules;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error fetching LeaveRules for TenantId: {TenantId}", dTO.TenantId);
                throw;
            }
        }


    }
}
