using axionpro.application.Constants;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class PolicyTypeRepository : IPolicyTypeRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<PolicyTypeRepository> _logger;

        public PolicyTypeRepository(WorkforceDbContext context, ILogger<PolicyTypeRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<GetPolicyTypeResponseDTO>> CreatePolicyTypeAsync(PolicyType request)
        {
            try
            {
                var entity = new PolicyType
                {
                    TenantId = request.TenantId,
                    PolicyName = request.PolicyName,
                    Description = request.Description,
                    IsActive = ConstantValues.IsByDefaultTrue,
                    IsSoftDelete = ConstantValues.IsByDefaultFalse,
                    AddedById = request.AddedById,
                    AddedDateTime = DateTime.UtcNow
                };

                await _context.PolicyTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ PolicyType created successfully with ID {PolicyTypeId} and Name {PolicyName}", entity.Id, entity.PolicyName);

                // Abhi-abhi insert hua record fetch karke DTO return karo
                var result = await GetPolicyTypeByTenantIdAsync(entity.TenantId);

                return result.ToList(); // IEnumerable ko List me convert
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding PolicyType: {PolicyName}", request.PolicyName);
                return new List<GetPolicyTypeResponseDTO>(); // empty list instead of null
            }
        }
                public Task<bool> DeletePolicyTypeAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<GetPolicyTypeResponseDTO>> GetAllPolicyTypesAsync(GetPolicyTypeResponseDTO getPolicyTypeDTO)
        {
            try
            {
                var query = _context.PolicyTypes.AsQueryable();

                if (getPolicyTypeDTO.IsActive)
                {
                    // Agar Active hai → Deleted false/null hi hona chahiye
                    query = query.Where(  pt => pt.TenantId== getPolicyTypeDTO.TenantId && ( pt.IsActive == true && (pt.IsSoftDelete == false || pt.IsSoftDelete == null)));
                }
                else
                {
                    // Agar Active false hai → Deleted kuch bhi ho sakta hai
                    query = query.Where(pt => pt.IsActive == false && (pt.IsSoftDelete == false || pt.IsSoftDelete == null));
                }

                var list = await query
                    .Select(pt => new GetPolicyTypeResponseDTO
                    {
                        Id = pt.Id,
                        PolicyName = pt.PolicyName ?? string.Empty,
                        TenantId = pt.TenantId ,
                        IsActive = pt.IsActive ?? false,
                        Description = pt.Description ?? string.Empty

                    })
                    .ToListAsync();

                return list ?? new List<GetPolicyTypeResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all PolicyTypes");
                return new List<GetPolicyTypeResponseDTO>();
            }
        }

        // Replace the GetPolicyTypeByIdAsync method with the following implementation


        public async Task<IEnumerable<GetPolicyTypeResponseDTO>> GetPolicyTypeByTenantIdAsync(long tenantId)
        {
            try
            {
                var policyTypes = await _context.PolicyTypes
                    .Where(p => p.TenantId == tenantId
                                && (p.IsSoftDelete == false || p.IsSoftDelete == null)
                                && p.IsActive == true) // sirf active
                    .OrderByDescending(p => p.Id) // latest first
                    .Select(p => new GetPolicyTypeResponseDTO
                    {
                        Id = p.Id,
                        PolicyName = p.PolicyName,
                        IsActive = p.IsActive ?? false,
                        TenantId = p.TenantId,
                    })
                    .ToListAsync();

                if (!policyTypes.Any())
                {
                    _logger.LogWarning("⚠️ No active PolicyType found for TenantId: {TenantId}", tenantId);
                }
                else
                {
                    _logger.LogInformation("✅ Fetched {Count} active PolicyType(s) for TenantId: {TenantId}", policyTypes.Count, tenantId);
                }

                return policyTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching PolicyTypes for TenantId: {TenantId}", tenantId);
                return new List<GetPolicyTypeResponseDTO>(); // Safe return
            }
        }


        public async Task<bool> UpdatePolicyTypeAsync(UpdatePolicyTypeDTO request)
        {
            try
            {
                // 🔹 Existing entity fetch karo
                var entity = await _context.PolicyTypes
                    .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsSoftDelete == false);

                if (entity == null)
                    return false; // Record nahi mila

                // 🔹 Unique name check karo (same tenant ke andar, aur apni ID ko exclude karo)
                var exists = await _context.PolicyTypes
                    .AnyAsync(pt => pt.PolicyName == request.PolicyName
                                    && pt.Id != request.Id
                                    && pt.IsSoftDelete == false);

                if (exists)
                    return false; // Duplicate name, update nahi hoga

                // 🔹 DTO se entity me map karo
                entity.PolicyName = request.PolicyName?.Trim();
                entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
                entity.IsActive = request.IsActive ?? true; // Default true
                entity.UpdateById = request.EmployeeId;
                entity.UpdateDateTime = DateTime.UtcNow;
                // 🔹 EF Core ko update bata do
                _context.PolicyTypes.Update(entity);

                // 🔹 Changes save karo
                var result = await _context.SaveChangesAsync();

                return result > 0; // Agar ek ya usse zyada rows affected → success
            }
            catch (Exception ex)
            {
                // 🔹 Logger use karo, ab error dikhega
                // Console.WriteLine(ex.Message); ya Serilog
                return false;
            }
        }


        public async Task<bool> SoftDeletePolicyTypeAsync(DeletePolicyTypeDTO request)
        {
            try
            {
                // 🔹 Existing entity fetch karo
                var entity = await _context.PolicyTypes
                    .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsSoftDelete == ConstantValues.IsByDefaultFalse);

                if (entity == null)
                    return false; // Record nahi mila

                entity.IsSoftDelete = ConstantValues.IsByDefaultTrue;
                entity.SoftDeleteById = request.EmployeeId;
                entity.SoftDeleteDateTime = DateTime.UtcNow;    
                entity.IsActive = ConstantValues.IsByDefaultFalse;
                _context.PolicyTypes.Update(entity);

                // 🔹 Changes save karo
                var result = await _context.SaveChangesAsync();

                return result > 0; 
            }
            catch (Exception ex)
            {
                // 🔹 Logger use karo, ab error dikhega
                // Console.WriteLine(ex.Message); ya Serilog
                return false;
            }
        }

        
    }


}
