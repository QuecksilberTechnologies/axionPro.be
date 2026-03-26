using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace axionpro.persistance.Repositories
{
    public class EmployeeExperienceDetailRepository : IEmployeeExperienceDetailRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExperienceDetailRepository> _logger;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        public EmployeeExperienceDetailRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExperienceDetailRepository> logger,
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
        public async Task AddAsync(EmployeeExperience entity)
        {
            await _context.EmployeeExperienceDetails.AddAsync(entity);
        }

        // ===============================
        // 🔹 ADD RANGE
        // ===============================
        public async Task AddRangeAsync(List<EmployeeExperience> entities)
        {
            await _context.EmployeeExperienceDetails.AddRangeAsync(entities);
        }

        // ===============================
        // 🔹 GET BY EXPERIENCE ID
        // ===============================
        public async Task<List<EmployeeExperience>> GetByExperienceIdAsync(long experienceId)
        {
            return await _context.EmployeeExperienceDetails
                .Where(x => x.EmployeeId == experienceId && !x.IsSoftDeleted)
                .ToListAsync();
        }

        // ===============================
        // 🔹 UPDATE
        // ===============================
        public Task<bool> UpdateAsync(EmployeeExperience entity)
        {
            _context.EmployeeExperienceDetails.Update(entity);
            return Task.FromResult(true); // SaveChanges UoW karega
        }

        // ===============================
        // 🔹 SOFT DELETE
        // ===============================
        public Task<bool> SoftDeleteAsync(EmployeeExperience entity)
        {
            entity.IsSoftDeleted = true;
            entity.IsActive = false;

            _context.EmployeeExperienceDetails.Update(entity);

            return Task.FromResult(true);
        }

        public Task<EmployeeExperience?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}