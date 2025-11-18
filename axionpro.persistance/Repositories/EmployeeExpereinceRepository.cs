
using AutoMapper;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Experience;
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
    public class EmployeeExpereinceRepository : IEmployeeExpereinceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExpereinceRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeExpereinceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExpereinceRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        // 1️⃣ Insert Parent Experience
        // ----------------------------------------------------
        public async Task<PagedResponseDTO<EmployeeExperience>> AddExperienceAsync(EmployeeExperience entity)
        {
            await _context.EmployeeExperiences.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new PagedResponseDTO<EmployeeExperience>
            {
                Items = new List<EmployeeExperience> { entity },
               
            };
        }

        // ----------------------------------------------------
        // 2️⃣ Bulk Insert ExperienceDetail
        // ----------------------------------------------------
        public async Task<PagedResponseDTO<EmployeeExperienceDetail>> AddDetailAsync(List<EmployeeExperienceDetail> entities)
        {
            await _context.EmployeeExperienceDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return new PagedResponseDTO<EmployeeExperienceDetail>
            {
                Items = entities,
               
            };
        }

        // ----------------------------------------------------
        // 3️⃣ Bulk Insert Payslip
        // ----------------------------------------------------
        public async Task<PagedResponseDTO<EmployeeExperiencePayslipUpload>> AddPayslipAsync(List<EmployeeExperiencePayslipUpload> entities)
        {
            await _context.EmployeeExperiencePayslipUploads.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return new PagedResponseDTO<EmployeeExperiencePayslipUpload>
            {
                Items = entities,
                 
            };
        }
        public Task<PagedResponseDTO<EmployeeExperience>> GetAllAsync(long employeeId)
        {
            throw new NotImplementedException();
        }
    }

}




 
 






