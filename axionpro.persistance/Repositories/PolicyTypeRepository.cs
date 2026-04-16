using AutoMapper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class PolicyTypeRepository : IPolicyTypeRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyTypeRepository> _logger;
       
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public PolicyTypeRepository(WorkforceDbContext context, IMapper mapper, ILogger<PolicyTypeRepository> logger,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        // ============================================
        // 🔹 CREATE
        // ============================================
        public async Task<GetPolicyTypeResponseDTO> CreatePolicyTypeAsync(
            PolicyType policyType)
        {
            try
            {
                await _context.PolicyTypes.AddAsync(policyType);
                await _context.SaveChangesAsync();

                // Entity → DTO
                return _mapper.Map<GetPolicyTypeResponseDTO>(policyType);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while creating PolicyType. Name: {PolicyName}",
                    policyType.PolicyName);

                return new GetPolicyTypeResponseDTO();
            }
        }

        // ============================================
        // 🔹 UPDATE (normal update)
        // ============================================
        public async Task<bool> UpdatePolicyTypeAsync(PolicyType policyType)
        {
            try
            {
                // 🔹 Only UPDATE – no checks here
                _context.PolicyTypes.Update(policyType);

                var rows = await _context.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while updating PolicyType. Id: {PolicyId}",
                    policyType.Id);

                return false;
            }
        }

        // ============================================
        // 🔹 SOFT DELETE (UPDATE ONLY)
        // ============================================
        public async Task<bool> SoftDeletePolicyTypeAsync(PolicyType policyType)
        {
            try
            {
                // 🔹 SAME CONTEXT se entity load karo
                var existing = await _context.PolicyTypes
                    .FirstOrDefaultAsync(x => x.Id == policyType.Id);

                if (existing == null)
                    return false;

                // 🔹 SOFT DELETE FLAGS (ONLY ON EXISTING)
                existing.IsSoftDelete = true;
                existing.IsActive = false;
                existing.SoftDeleteById = policyType.SoftDeleteById;
                existing.SoftDeleteDateTime = policyType.SoftDeleteDateTime;

                // ❌ Attach / Entry.State ki zarurat nahi
                // EF already TRACK kar raha hai

                var rows = await _context.SaveChangesAsync();

                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while soft deleting PolicyType. Id: {PolicyId}",
                    policyType.Id);

                return false;
            }
        }



        // ============================================
        // 🔹 GET BY ID (used by handler)
        // ============================================
        public async Task<PolicyType?> GetPolicyTypeByIdAsync(int id, bool? isActive)
        {
            try
            {
                return await _context.PolicyTypes
                    .FirstOrDefaultAsync(pt =>
                        pt.Id == id &&
                        (isActive == null || pt.IsActive == isActive) &&
                        (pt.IsSoftDelete == false || pt.IsSoftDelete == null)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching PolicyType by Id: {PolicyId}",
                    id);

                return null;
            }
        }

        // ============================================
        // 🔹 GET ALL (Tenant wise / Active)
        // ============================================

        public async Task<IEnumerable<GetPolicyTypeResponseDTO>> GetPolicyTypesAsync(long tenantId, bool isActive)
        {
            try
            {
                var query = _context.PolicyTypes.AsQueryable();

                // 🔹 Tenant filter (MANDATORY)
                query = query.Where(pt => pt.TenantId == tenantId);

                // 🔹 Active filter
                if (isActive)
                {
                    query = query.Where(pt =>
                        pt.IsActive == true &&
                        (pt.IsSoftDelete == false || pt.IsSoftDelete == null));
                }

                var list = await query
                    .Select(pt => new GetPolicyTypeResponseDTO
                    {
                        Id = pt.Id,
                        
                        PolicyName = pt.PolicyName ?? string.Empty,
                        IsActive = pt.IsActive ?? false,
                        IsStructured = pt.IsStructured,
                        Description = pt.Description ?? string.Empty,
                        PolicyTypeEnumVal = pt.PolicyTypeEnumVal,

                        // 🔥 FIX: SET REQUIRED FIELD
                        EmployeeTypeIds = _context.UnStructuredPolicyTypeMappingWithEmployeeTypes
                            .Where(m => m.PolicyTypeId == pt.Id
                              && m.TenantId == tenantId
                        && !m.IsSoftDeleted)
                        .Select(m => m.EmployeeTypeId)
                        .ToList(),

                        // 🔥 DOCUMENTS INCLUDE
                        DocDetails = pt.PolicyTypeDocument
                            .Where(d => d.IsSoftDeleted == false && d.IsActive == true)
                            .Select(d => new GetPolicyTypeDocumentResponseDTO
                            {
                                Id = d.Id,
                                PolicyTypeId = d.PolicyTypeId,
                                DocumentTitle = d.DocumentTitle ?? string.Empty,
                                FileName = d.FileName ?? string.Empty,
                                FilePath = d.FilePath ?? string.Empty,
                                IsActive = d.IsActive
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching PolicyTypes");
                return new List<GetPolicyTypeResponseDTO>();
            }
        }

        public async Task<ApiResponse<List<GetAllPolicyTypeResponseDTO>>> GetAllPolicyTypesAsync(long tenantId, bool isActive, int enumval)
        {
            try
            {
                enumval = 1;
                // --------------------------------------------------
                // 1️⃣ Base query (Tenant mandatory)
                // --------------------------------------------------
                var query = _context.PolicyTypes
                    .AsNoTracking() // ✅ fast read-only
                    .Where(pt => pt.TenantId == tenantId && pt.PolicyTypeEnumVal==enumval);

                // --------------------------------------------------
                // 2️⃣ Active + SoftDelete filter
                // --------------------------------------------------
                query = isActive ? query.Where(pt => pt.IsActive == true && pt.IsSoftDelete!=true)  
                    : query.Where(pt =>  pt.IsActive == false && (pt.IsSoftDelete == false || pt.IsSoftDelete == null));

                // --------------------------------------------------
                // 3️⃣ Projection → DDL DTO
                // --------------------------------------------------
                var list = await query
                    .OrderBy(pt => pt.PolicyName) // ✅ DDL friendly
                    .Select(pt => new GetAllPolicyTypeResponseDTO
                    {
                        Id = pt.Id,
                        PolicyName = pt.PolicyName ?? string.Empty
                    })
                    .ToListAsync();

                // --------------------------------------------------
                // 4️⃣ No data → UI wants false + message
                // --------------------------------------------------
                if (!list.Any())
                {
                    return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                        .Fail("No policy types found.");
                }

                // --------------------------------------------------
                // 5️⃣ Success
                // --------------------------------------------------
                return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                    .Success(list, "Policy types loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching PolicyTypes. TenantId: {TenantId}",
                    tenantId);

                // --------------------------------------------------
                // 6️⃣ Hard failure
                // --------------------------------------------------
                return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                    .Fail("Failed to load policy types.");
            }
        }

        public async Task<List<PolicyType>> AutoCreatePolicyTypesAsync(List<PolicyType> policyTypes)
        {
            if (policyTypes == null || !policyTypes.Any())
                throw new ArgumentException("PolicyTypes list cannot be null or empty");

            await _context.PolicyTypes.AddRangeAsync(policyTypes);

            return policyTypes;
        }

        public Task<PolicyType?> GetPolicyTypeOnlyDocByIdAsync(long Id)
        {
            throw new NotImplementedException();
        }
    }
}
