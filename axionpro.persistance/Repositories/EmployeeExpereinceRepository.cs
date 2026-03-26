
using AutoMapper;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class EmployeeExpereinceRepository : IEmployeeExperienceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExpereinceRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeExpereinceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExpereinceRepository> logger,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;

            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }
        // ===============================
        // 🔹 CREATE
        // ===============================
        public async Task AddAsync(EmployeeExperience entity)
        {
            await _context.EmployeeExperiences.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // ===============================
        // 🔹 UPDATE
        // ===============================
        public async Task<bool> UpdateAsync(EmployeeExperience entity)
        {
            _context.EmployeeExperiences.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ===============================
        // 🔹 DELETE (SOFT)
        // ===============================
        public async Task<bool> SoftDeleteAsync(EmployeeExperience entity)
        {
            entity.IsSoftDeleted = true;
            entity.IsActive = false;

            _context.EmployeeExperiences.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ===============================
        // 🔹 GET BY ID
        // ===============================
        public async Task<EmployeeExperience?> GetByIdAsync(long id, long tenantId)
        {
            return await _context.EmployeeExperiences
                .Include(x => x.EmployeeExperienceDetails)
                    .ThenInclude(d => d.EmployeeExperienceDocuments)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.EmployeeId == tenantId &&
                    !x.IsSoftDeleted);
        }

        // ===============================
        // 🔹 GET LIST (PAGINATED)
        // ===============================
        public async Task<EmployeeExperience?> GetByEmployeeIdWithDetailsAsync(long employeeId)
        {
            return await _context.EmployeeExperiences
                .Include(e => e.EmployeeExperienceDetails)
                    .ThenInclude(d => d.EmployeeExperienceDocuments)
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    !x.IsSoftDeleted);
        }
    }

}


 
 






