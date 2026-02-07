using AutoMapper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
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
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public PolicyTypeRepository(WorkforceDbContext context, IMapper mapper, ILogger<PolicyTypeRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
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
        public async Task<bool> SoftDeletePolicyTypeAsync(
            PolicyType policyType)
        {
            try
            {
                // 🔹 Handler already:
                // - Checked existence
                // - Set IsSoftDelete / IsActive / audit fields
                // Repository only persists changes

                _context.PolicyTypes.Update(policyType);

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
        public async Task<PolicyType?> GetPolicyTypeByIdAsync(int id)
        {
            try
            {
                return await _context.PolicyTypes
                    .FirstOrDefaultAsync(pt =>
                        pt.Id == id &&
                        (pt.IsSoftDelete == false || pt.IsSoftDelete == null));
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
        public async Task<IEnumerable<GetPolicyTypeResponseDTO>>  GetAllPolicyTypesAsync(long tenantId,  bool isActive)
        {
            try
            {
                var query = _context.PolicyTypes.AsQueryable();

                // 🔹 Tenant filter (MANDATORY)
                query = query.Where(pt => pt.TenantId == tenantId);

                // 🔹 Active / Soft delete filter
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
                        Description = pt.Description ?? string.Empty
                    })
                    .ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching PolicyTypes");

                return new List<GetPolicyTypeResponseDTO>();
            }
        }
    }
}
