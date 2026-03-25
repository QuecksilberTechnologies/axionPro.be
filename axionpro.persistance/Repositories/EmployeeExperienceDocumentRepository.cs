using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class EmployeeExperienceDocumentRepository : IEmployeeExperienceDocumentRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExperienceDocumentRepository> _logger;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        public EmployeeExperienceDocumentRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExperienceDocumentRepository> logger,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        // ===============================
        // 🔹 ADD SINGLE
        // ===============================
        public async Task AddAsync(EmployeeExperienceDocument entity)
        {
            await _context.EmployeeExperienceDocuments.AddAsync(entity);
        }

        // ===============================
        // 🔹 ADD RANGE
        // ===============================
        public async Task AddRangeAsync(List<EmployeeExperienceDocument> entities)
        {
            await _context.EmployeeExperienceDocuments.AddRangeAsync(entities);
        }

        // ===============================
        // 🔹 GET BY DETAIL ID
        // ===============================
        public async Task<List<EmployeeExperienceDocument>> GetByDetailIdAsync(long detailId)
        {
            return await _context.EmployeeExperienceDocuments
                .Where(x => x.EmployeeExperienceDetailId == detailId && !x.IsSoftDeleted)
                .ToListAsync();
        }

        // ===============================
        // 🔹 SOFT DELETE
        // ===============================
        public Task<bool> SoftDeleteAsync(EmployeeExperienceDocument entity)
        {
            entity.IsSoftDeleted = true;
            entity.IsActive = false;

            _context.EmployeeExperienceDocuments.Update(entity);

            return Task.FromResult(true); // ✅ SaveChanges UoW karega
        }
    }
}