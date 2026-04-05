using AutoMapper;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UnStructuredEmployeePolicyTypeMappingRepository
    : IUnStructuredEmployeePolicyTypeMappingRepository
{
    private readonly WorkforceDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UnStructuredEmployeePolicyTypeMappingRepository> _logger;

    public UnStructuredEmployeePolicyTypeMappingRepository(
        WorkforceDbContext context,
        ILogger<UnStructuredEmployeePolicyTypeMappingRepository> logger,
        IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    // ✅ GET ALL
    public async Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetAllAsync(
      long tenantId,
      int policyTypeId,
      bool isActive)
    {
        return await _context.UnStructuredPolicyTypeMappingWithEmployeeTypes
            .Include(x => x.EmployeeType)   // 🔥 IMPORTANT
            .Include(x => x.PolicyType)     // 🔥 IMPORTANT
            .Where(x =>
                x.TenantId == tenantId &&
                x.PolicyTypeId == policyTypeId &&               
                x.IsActive == isActive &&
                !x.IsSoftDeleted)
            .ToListAsync();
    }

    // ✅ GET BY EMPLOYEE TYPE
    public async Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetByEmployeeTypeIdAsync(int employeeTypeId, long tenantId)
    {
        return await _context.UnStructuredPolicyTypeMappingWithEmployeeTypes
            .Where(x => x.EmployeeTypeId == employeeTypeId
                        && x.TenantId == tenantId
                        && !x.IsSoftDeleted)
            .ToListAsync();
    }

    // ✅ GET BY ID
    public async Task<UnStructuredPolicyTypeMappingWithEmployeeType?> GetByIdAsync(long id)
    {
        return await _context.UnStructuredPolicyTypeMappingWithEmployeeTypes
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted);
    }

    // ✅ ADD
    public async Task<bool> AddRangeAsync(List<UnStructuredPolicyTypeMappingWithEmployeeType> entities)
    {
        await _context.UnStructuredPolicyTypeMappingWithEmployeeTypes.AddRangeAsync(entities);
        return await _context.SaveChangesAsync() > 0;
    }
    // ✅ UPDATE
    public async Task<bool> UpdateAsync(UnStructuredPolicyTypeMappingWithEmployeeType entity)
    {
        _context.UnStructuredPolicyTypeMappingWithEmployeeTypes.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    // ✅ SOFT DELETE
    public async Task<bool> SoftDeleteAsync(UnStructuredPolicyTypeMappingWithEmployeeType entity)
    {
        entity.IsSoftDeleted = true;
        entity.IsActive = false;
        entity.SoftDeletedDateTime = DateTime.UtcNow;

        _context.UnStructuredPolicyTypeMappingWithEmployeeTypes.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetByEmployeeTypeByPolicyTypeIdAsync(int policyTypeId, long tenantId)
    {
        return await _context.UnStructuredPolicyTypeMappingWithEmployeeTypes
       .AsNoTracking()
       .Where(x =>
        x.PolicyTypeId == policyTypeId &&
        x.TenantId == tenantId &&
        x.IsActive == true &&
        (x.IsSoftDeleted == false || x.IsSoftDeleted == null))
    .ToListAsync();
    }
}