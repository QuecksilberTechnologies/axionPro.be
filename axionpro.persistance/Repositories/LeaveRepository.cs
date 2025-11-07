using axionpro.application.Constants;
using axionpro.application.DTOs.Leave;
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
    public class LeaveRepository : ILeaveRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<LeaveRepository> _logger;

        public LeaveRepository(WorkforceDbContext context, ILogger<LeaveRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<LeaveType>> GetAllLeaveAsync(bool? IsActive,long? tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching leave types from the database...");

                IQueryable<LeaveType> query = _context.LeaveTypes;

                if (IsActive.HasValue)
                {
                    // Agar true/false diya gaya hai
                    query = query.Where(l => tenantId==l.TenantId && l.IsActive == IsActive.Value
                                          && (l.IsSoftDeleted == false || l.IsSoftDeleted == null));
                }
                else
                {
                    // Agar null hai → default inactive + not soft deleted
                    query = query.Where(l => tenantId == l.TenantId && l.IsActive == false 
                                          && (l.IsSoftDeleted == false || l.IsSoftDeleted == null));
                }

                var leaveTypes = await query.ToListAsync();

                if (!leaveTypes.Any())
                {
                    _logger.LogWarning("⚠️ No leave types found in the database.");
                }

                return leaveTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An error occurred while fetching leave types.");
                return new List<LeaveType>();
            }
        }

        public async Task<List<PolicyLeaveTypeMapping>> GetAllLeavePolicyByTenantIdAsync(long tenantId, bool isActive)
        {
            try
            {
                _logger.LogInformation("🔍 Fetching LeavePolicies for TenantId={TenantId}, IsActive={IsActive}", tenantId, isActive);

                var leavePolicies = await _context.PolicyLeaveTypeMappings
                    .Where(lp => lp.TenantId == tenantId
                              && lp.IsActive == isActive
                              && (lp.IsSoftDeleted == false || lp.IsSoftDeleted == null))
                    .OrderByDescending(lp => lp.AddedDateTime)
                    .ToListAsync();

                _logger.LogInformation("✅ {Count} records found for TenantId={TenantId}, IsActive={IsActive}", leavePolicies.Count, tenantId, isActive);

                return leavePolicies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching leave policies for TenantId={TenantId}", tenantId);
                return new List<PolicyLeaveTypeMapping>();
            }
        }



        public async Task<List<GetLeaveTypeWithPolicyMappingResponseDTO>> GetAllLeavePolicyByEmployeeTypeIdAsync(GetPolicyLeaveTypeByEmpTypeIdRequestDTO dto)
        {
            List<GetLeaveTypeWithPolicyMappingResponseDTO> response = null;
            try
            {
                var result = await _context.PolicyLeaveTypeMappings
                    .Where(m =>
                        m.TenantId == dto.TenantId &&
                        (m.IsSoftDeleted == false || m.IsSoftDeleted == null) &&
                        m.EmployeeTypeId == dto.EmployeeTypeId &&
                        // ✅ TodaysDate validation
                        dto.TodaysDate >= m.EffectiveFrom &&
                        (m.EffectiveTo == null || dto.TodaysDate <= m.EffectiveTo) &&
                        // ✅ IsMarriedApplicable validation
                        (m.IsMarriedApplicable == dto.IsMarriedApplicable || m.IsMarriedApplicable == null) &&
                        // ✅ Gender validation
                        (m.ApplicableGenderId == dto.ApplicableGenderId || m.ApplicableGenderId == null)
                    )
                    .Join(_context.PolicyTypes,
                          m => m.PolicyTypeId,
                          pt => pt.Id,
                          (m, pt) => new { m, pt })
                    .Join(_context.LeaveTypes,
                          x => x.m.LeaveTypeId,
                          lt => lt.Id,
                          (x, lt) => new { x.m, x.pt, lt })
                    .Join(_context.EmployeeTypes,
                          x => x.m.EmployeeTypeId,
                          et => et.Id,
                          (x, et) => new GetLeaveTypeWithPolicyMappingResponseDTO
                          {
                              Id = x.m.Id,
                              TenantId = x.m.TenantId,
                              PolicyTypeName = x.pt.PolicyName,
                              LeaveTypeId = x.m.LeaveTypeId,
                              LeaveTypeName = x.lt.LeaveName,
                              EmployeeTypeId = x.m.EmployeeTypeId,
                              EmployeeTypeName = et.TypeName,
                              ApplicableGenderId = x.m.ApplicableGenderId,
                              IsMarriedApplicable = x.m.IsMarriedApplicable,
                              TotalLeavesPerYear = x.m.TotalLeavesPerYear,
                              MonthlyAccrual = x.m.MonthlyAccrual,
                              IsEmployeeMapped = x.m.IsEmployeeMapped,
                              CarryForward = x.m.CarryForward,
                              Encashable = x.m.Encashable,
                              EffectiveFrom = x.m.EffectiveFrom,
                              EffectiveTo = x.m.EffectiveTo,
                              IsActive = x.m.IsActive,
                              Remark = x.m.Remark
                          })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAllPolicyLeaveTypeMappingsAsync (TenantId: {dto.TenantId})");
                return response;
            }
        }





        public async Task<List<LeaveType>> CreateLeaveTypeAsync(LeaveType leaveType)
        {
            if (leaveType == null)
                throw new ArgumentNullException(nameof(leaveType), "LeaveType entity cannot be null.");

            try
            {
                // Entity add karo
                await _context.LeaveTypes.AddAsync(leaveType);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ LeaveType '{LeaveName}' created successfully with Id: {Id}", leaveType.LeaveName, leaveType.Id);

                // Saare active leave types return karo

                return await GetAllLeaveAsync(leaveType.IsActive, leaveType.TenantId);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while creating LeaveType: {LeaveName}", leaveType?.LeaveName);
                throw new InvalidOperationException("Failed to save LeaveType to database. See inner exception for details.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating LeaveType: {LeaveName}", leaveType?.LeaveName);
                throw;
            }
        }


        public async Task<bool> DeleteLeaveAsync(int leaveId)
        {
            try
            {
                var leave = await _context.LeaveTypes.FindAsync(leaveId);
                if (leave == null)
                {
                    _logger.LogWarning("Leave type with ID {LeaveId} not found.", leaveId);
                    return false;
                }

                _context.LeaveTypes.Remove(leave);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Leave type with ID {LeaveId} deleted successfully.", leaveId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting leave type with ID {LeaveId}.", leaveId);
                return false;
            }
        }

        public async Task<LeaveType?> GetLeaveByIdAsync(int leaveId)
        {
            return await _context.LeaveTypes.FindAsync(leaveId);
        }

        public async Task<PolicyLeaveTypeMapping> GetLeavePolicyByIdAsync(long leavePolicyId)
        {
            return await _context.PolicyLeaveTypeMappings.FindAsync(leavePolicyId);

        }
      

        public async Task<bool> UpdateLeavTypeAsync(LeaveType leaveType, long userId)
        {
            try
            {
                if (leaveType == null)
                {
                    throw new ArgumentNullException(nameof(leaveType), "LeaveType object cannot be null.");
                }

                // 🔹 Sirf wahi record fetch hoga jo delete nahi hua hai
                var existingLeave = await _context.LeaveTypes
                    .FirstOrDefaultAsync(l => l.Id == leaveType.Id &&
                                              (l.IsSoftDeleted == ConstantValues.IsByDefaultFalse || l.IsSoftDeleted == null));

                if (existingLeave == null)
                {
                    _logger.LogWarning("⚠️ Leave type with ID {LeaveId} not found or it is soft deleted.", leaveType.Id);
                    return false; // Empty return instead of fetching all
                }

                // 🔹 Validation: Name empty nahi hona chahiye
                if (string.IsNullOrWhiteSpace(leaveType.LeaveName))
                {
                    _logger.LogWarning("⚠️ Leave type name cannot be empty. ID: {LeaveId}", leaveType.Id);
                    return false;
                }

                // 🔹 Update allowed fields
                existingLeave.LeaveName = leaveType.LeaveName.Trim();
                existingLeave.Description = leaveType.Description?.Trim();
                existingLeave.IsActive = leaveType.IsActive;
                existingLeave.UpdateById = userId;
                existingLeave.UpdateDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Leave type with ID {LeaveId} updated successfully.", leaveType.Id);

                // 🔹 Return fresh list
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating leave type with ID {LeaveId}.", leaveType.Id);
                return false;
            }
        }

        public async Task<bool> DeleteLeaveAsync(LeaveType leaveType)
        {
            if (leaveType == null)
                throw new ArgumentNullException(nameof(leaveType), "LeaveType cannot be null.");

            try
            {
                var existingLeave = await _context.LeaveTypes
                    .FirstOrDefaultAsync(l => l.Id == leaveType.Id);

                if (existingLeave == null)
                {
                    _logger.LogWarning("⚠️ LeaveType with ID {LeaveId} not found.", leaveType.Id);
                    return false;
                }

                // Null ko "false" treat karenge (not deleted)
                bool isDeleted = existingLeave.IsSoftDeleted ?? false;

                if (isDeleted)
                {
                    _logger.LogWarning("⚠️ LeaveType with ID {LeaveId} is already soft-deleted.", existingLeave.Id);
                    return false;
                }

                // 2️⃣ Soft Delete set
                existingLeave.IsSoftDeleted = ConstantValues.IsByDefaultTrue;
                existingLeave.SoftDeletedDateTime = DateTime.UtcNow;
                existingLeave.SoftDeletedBy = leaveType.SoftDeletedBy;

                // 3️⃣ DB update
                _context.LeaveTypes.Update(existingLeave);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ LeaveType with ID {LeaveId} successfully soft-deleted by UserId {UserId}.",
                    existingLeave.Id,
                    existingLeave.SoftDeletedBy
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error occurred while soft-deleting LeaveType with ID {LeaveId} by UserId {UserId}.",
                    leaveType?.Id,
                    leaveType?.SoftDeletedBy
                );
                throw; // Exception propagate karna best hai
            }
        }




        public async Task<bool> DeleteLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy)
        {
            if (leavePolicy == null)
                throw new ArgumentNullException(nameof(leavePolicy), "Leave policy entity cannot be null.");

            try
            {
                // 🔹 1️⃣ Handle nullable IsSoftDeleted safely
                bool alreadyDeleted = leavePolicy.IsSoftDeleted ?? false;

                // 🔹 2️⃣ Check if already deleted
                if (alreadyDeleted)
                {
                    _logger.LogWarning(
                        "⚠️ Leave policy with ID {LeavePolicyId} is already soft-deleted.",
                        leavePolicy.Id
                    );
                    return false;
                }

                // 🔹 3️⃣ Mark as soft deleted
                leavePolicy.IsSoftDeleted = true;
                leavePolicy.SoftDeleteDateTime = DateTime.UtcNow;

                // ⚙️ Caller (Service/Handler) should set SoftDeleteById before calling this method
                if (leavePolicy.SoftDeleteById == null)
                {
                    _logger.LogWarning(
                        "⚠️ SoftDeleteById not set for LeavePolicy ID {LeavePolicyId}. Consider setting the user ID in handler.",
                        leavePolicy.Id
                    );
                }

                // 🔹 4️⃣ Update in database
                _context.PolicyLeaveTypeMappings.Update(leavePolicy);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Leave policy with ID {LeavePolicyId} soft-deleted successfully by UserId {UserId}.",
                    leavePolicy.Id,
                    leavePolicy.SoftDeleteById
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Exception occurred while soft-deleting leave policy with ID {LeavePolicyId} by UserId {UserId}.",
                    leavePolicy?.Id,
                    leavePolicy?.SoftDeleteById
                );

                throw; // propagate exception
            }
        }


        public async Task<List<PolicyLeaveTypeMapping>> CreateLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy)
        {
            if (leavePolicy == null)
                throw new ArgumentNullException(nameof(leavePolicy), "LeavePolicy entity cannot be null.");

            try
            {
                // Entity add karo
                await _context.PolicyLeaveTypeMappings.AddAsync(leavePolicy);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ LeavePolicy created successfully with Id: {Id}, PolicyTypeId: {PolicyTypeId}, LeaveTypeId: {LeaveTypeId}",
                    leavePolicy.Id, leavePolicy.PolicyTypeId, leavePolicy.LeaveTypeId);

                // Saare active/inactive leave policies return karo (IsDeleted = false by default)
                return await GetAllLeavePolicyByTenantIdAsync(leavePolicy.TenantId, leavePolicy.IsActive);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "❌ Database update failed while creating LeavePolicy (PolicyTypeId: {PolicyTypeId}, LeaveTypeId: {LeaveTypeId})",
                    leavePolicy?.PolicyTypeId, leavePolicy?.LeaveTypeId);

                throw new InvalidOperationException("Failed to save LeavePolicy to database. See inner exception for details.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Unexpected error occurred while creating LeavePolicy (PolicyTypeId: {PolicyTypeId}, LeaveTypeId: {LeaveTypeId})",
                    leavePolicy?.PolicyTypeId, leavePolicy?.LeaveTypeId);

                throw;
            }
        }

        public async Task<List<PolicyLeaveTypeMapping>> GetAllLeavePolicyAsync(bool IsActive)
        {
            try
            {
                _logger.LogInformation("Fetching leave policies with IsActive = {IsActive} and not soft-deleted...", IsActive);

                var leavePolicies = await _context.PolicyLeaveTypeMappings
                    .Where(lp => lp.IsActive == IsActive && lp.IsSoftDeleted!=true  ) // ✅ Active filter + Not soft deleted
                    .OrderByDescending(lp => lp.AddedDateTime) // ✅ Latest first
                    .ToListAsync();

                if (!leavePolicies.Any())
                {
                    _logger.LogWarning("⚠️ No leave policies found with IsActive = {IsActive}.", IsActive);
                }

                return leavePolicies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching leave policies with IsActive = {IsActive}.", IsActive);
                return new List<PolicyLeaveTypeMapping>();
            }
        }

        //public async Task<List<PolicyLeaveTypeMapping>> UpdateLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy, long userId)
        //{
        //    if (leavePolicy == null)
        //        throw new ArgumentNullException(nameof(leavePolicy), "LeavePolicy cannot be null.");

        //    if (leavePolicy.Id <= 0)
        //        throw new ArgumentException("Invalid LeavePolicy Id.", nameof(leavePolicy.Id));

        //    if (userId <= 0)
        //        throw new ArgumentException("Invalid UserId.", nameof(userId));

        //    try
        //    {
        //        // fetch existing non-deleted record
        //        var existing = await _context.PolicyLeaveTypeMappings
        //            .FirstOrDefaultAsync(lp => lp.Id == leavePolicy.Id && !lp.IsSoftDeleted);

        //        if (existing == null)
        //        {
        //            _logger.LogWarning("LeavePolicy with ID {LeavePolicyId} not found or is soft-deleted.", leavePolicy.Id);
        //            return new List<PolicyLeaveTypeMapping>();
        //        }

        //        // validate effective date range
        //        if (leavePolicy.EffectiveTo.HasValue && leavePolicy.EffectiveFrom > leavePolicy.EffectiveTo.Value)
        //        {
        //            _logger.LogWarning("Invalid effective date range for LeavePolicy {LeavePolicyId}. From: {From}, To: {To}",
        //                leavePolicy.Id, leavePolicy.EffectiveFrom, leavePolicy.EffectiveTo);
        //            throw new InvalidOperationException("EffectiveFrom cannot be greater than EffectiveTo.");
        //        }

        //        // Prevent duplicate active policy for same PolicyType+LeaveType+(EmployeeType)
        //        // Only check when request is trying to set IsActive = true
        //        if (leavePolicy.IsActive)
        //        {
        //            var duplicateExists = await _context.PolicyLeaveTypeMappings.AnyAsync(lp =>
        //                lp.Id != leavePolicy.Id &&
        //                lp.PolicyTypeId == leavePolicy.PolicyTypeId &&
        //                lp.LeaveTypeId == leavePolicy.LeaveTypeId &&
        //                (lp.EmployeeTypeId == leavePolicy.EmployeeTypeId ||
        //                 (lp.EmployeeTypeId == null && leavePolicy.EmployeeTypeId == null)) &&
        //                !lp.IsSoftDeleted &&
        //                lp.IsActive);

        //            if (duplicateExists)
        //            {
        //                _logger.LogWarning("Duplicate active leave policy exists for PolicyTypeId={PolicyTypeId}, LeaveTypeId={LeaveTypeId}, EmployeeTypeId={EmployeeTypeId}.",
        //                    leavePolicy.PolicyTypeId, leavePolicy.LeaveTypeId, leavePolicy.EmployeeTypeId);
        //                throw new InvalidOperationException("An active leave policy with the same PolicyType + LeaveType + EmployeeType already exists.");
        //            }
        //        }

        //        // Update allowed fields
        //        existing.PolicyTypeId = leavePolicy.PolicyTypeId;
        //        existing.LeaveTypeId = leavePolicy.LeaveTypeId;
        //        existing.EmployeeTypeId = leavePolicy.EmployeeTypeId;
        //        existing.ApplicableGender = leavePolicy.ApplicableGender;
        //        existing.ApplicableMaritalStatus = leavePolicy.ApplicableMaritalStatus;
        //        existing.TotalLeavesPerYear = leavePolicy.TotalLeavesPerYear;
        //        existing.MonthlyAccrual = leavePolicy.MonthlyAccrual;
        //        existing.CarryForward = leavePolicy.CarryForward;
        //        existing.MaxCarryForward = leavePolicy.MaxCarryForward;
        //        existing.CarryForwardExpiryMonths = leavePolicy.CarryForwardExpiryMonths;
        //        existing.Encashable = leavePolicy.Encashable;
        //        existing.MaxEncashable = leavePolicy.MaxEncashable;
        //        existing.IsProofRequired = leavePolicy.IsProofRequired;
        //        existing.ProofDocumentType = leavePolicy.ProofDocumentType;
        //        existing.EffectiveFrom = leavePolicy.EffectiveFrom;
        //        existing.EffectiveTo = leavePolicy.EffectiveTo;
        //        existing.IsActive = leavePolicy.IsActive;
        //        // Do not flip IsSoftDeleted here unless explicit delete is intended
        //        existing.Remark = leavePolicy.Remark;

        //        // Meta
        //        existing.UpdatedById = userId;
        //        existing.UpdatedDateTime = DateTime.UtcNow;

        //        _context.PolicyLeaveTypeMappings.Update(existing);
        //        await _context.SaveChangesAsync();

        //        _logger.LogInformation("LeavePolicy with ID {LeavePolicyId} updated successfully by user {UserId}.", existing.Id, userId);

        //        // Return fresh list; use the updated IsActive to filter so latest relevant list comes back
        //        return await GetAllLeavePolicyAsync(existing.IsActive);
        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        _logger.LogError(dbEx, "Database update failed while updating LeavePolicy with ID {LeavePolicyId}.", leavePolicy.Id);
        //        throw; // bubble up - calling layer can convert to user-friendly message
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error while updating LeavePolicy with ID {LeavePolicyId}.", leavePolicy.Id);
        //        throw;
        //    }
        //}

        public async Task<bool> UpdateLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy, long userId)
        {
            if (leavePolicy == null)
                throw new ArgumentNullException(nameof(leavePolicy), "LeavePolicy cannot be null.");

            if (leavePolicy.Id <= 0)
                throw new ArgumentException("Invalid LeavePolicy Id.", nameof(leavePolicy.Id));

            if (userId <= 0)
                throw new ArgumentException("Invalid UserId.", nameof(userId));

            try
            {
                // fetch existing non-deleted record
                var existing = await _context.PolicyLeaveTypeMappings.FirstOrDefaultAsync(lp => lp.Id == leavePolicy.Id && (lp.IsSoftDeleted == false || lp.IsSoftDeleted == null));

                if (existing == null)
                {
                    _logger.LogWarning("LeavePolicy with ID {LeavePolicyId} not found or is soft-deleted.", leavePolicy.Id);
                    return false;
                }

                // validate effective date range
                if (leavePolicy.EffectiveTo.HasValue && leavePolicy.EffectiveFrom > leavePolicy.EffectiveTo.Value)
                {
                    _logger.LogWarning("Invalid effective date range for LeavePolicy {LeavePolicyId}. From: {From}, To: {To}",
                        leavePolicy.Id, leavePolicy.EffectiveFrom, leavePolicy.EffectiveTo);
                    return false;
                }

                // Prevent duplicate active policy for same PolicyType+LeaveType+(EmployeeType)
                if (leavePolicy.IsActive)
                {
                    var duplicateExists = await _context.PolicyLeaveTypeMappings.AnyAsync(lp =>
                        lp.Id != leavePolicy.Id &&
                        lp.PolicyTypeId == leavePolicy.PolicyTypeId &&
                        lp.LeaveTypeId == leavePolicy.LeaveTypeId &&
                        (lp.EmployeeTypeId == leavePolicy.EmployeeTypeId ||
                         (lp.EmployeeTypeId == null && leavePolicy.EmployeeTypeId == null)) &&
                        lp.IsSoftDeleted != true &&
                        lp.IsActive);

                    if (duplicateExists)
                    {
                        _logger.LogWarning("Duplicate active leave policy exists for PolicyTypeId={PolicyTypeId}, LeaveTypeId={LeaveTypeId}, EmployeeTypeId={EmployeeTypeId}.",
                            leavePolicy.PolicyTypeId, leavePolicy.LeaveTypeId, leavePolicy.EmployeeTypeId);
                        return false;
                    }
                }

                // Update allowed fields
                existing.PolicyTypeId = leavePolicy.PolicyTypeId;
                existing.LeaveTypeId = leavePolicy.LeaveTypeId;
                existing.EmployeeTypeId = leavePolicy.EmployeeTypeId;
                existing.ApplicableGenderId = leavePolicy.ApplicableGenderId;
                existing.IsMarriedApplicable = leavePolicy.IsMarriedApplicable;
                existing.TotalLeavesPerYear = leavePolicy.TotalLeavesPerYear;
                existing.MonthlyAccrual = leavePolicy.MonthlyAccrual;
                existing.CarryForward = leavePolicy.CarryForward;
                existing.MaxCarryForward = leavePolicy.MaxCarryForward;
                existing.CarryForwardExpiryMonths = leavePolicy.CarryForwardExpiryMonths;
                existing.Encashable = leavePolicy.Encashable;
                existing.MaxEncashable = leavePolicy.MaxEncashable;
                existing.IsProofRequired = leavePolicy.IsProofRequired;
                existing.ProofDocumentType = leavePolicy.ProofDocumentType;
                existing.EffectiveFrom = leavePolicy.EffectiveFrom;
                existing.EffectiveTo = leavePolicy.EffectiveTo;
                existing.IsActive = leavePolicy.IsActive;
                existing.Remark = leavePolicy.Remark;

                // Meta
                existing.UpdatedById = userId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.PolicyLeaveTypeMappings.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("LeavePolicy with ID {LeavePolicyId} updated successfully by user {UserId}.", existing.Id, userId);

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while updating LeavePolicy with ID {LeavePolicyId}.", leavePolicy.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating LeavePolicy with ID {LeavePolicyId}.", leavePolicy.Id);
                return false;
            }
        }

      


       
        public async Task<bool> UpdateLeaveAssignOnlyAsync(long policyId, long userId, bool IsUpdate)
        {
            var existing = await _context.PolicyLeaveTypeMappings
                .FirstOrDefaultAsync(p => p.Id == policyId);

            if (existing == null)
                return false;

            // ✅ Sirf IsEmployeeMapped update karna hai
            existing.IsEmployeeMapped = IsUpdate;
            existing.UpdatedById = userId;
            existing.UpdatedDateTime = DateTime.UtcNow;

            _context.PolicyLeaveTypeMappings.Update(existing);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
