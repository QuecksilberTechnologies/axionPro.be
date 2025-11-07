

using AutoMapper;
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
  
   public class EmployeeInsuranceRepository : IEmployeeInsuranceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeInsuranceRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeInsuranceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeInsuranceRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public Task<PagedResponseDTO<GetExperienceResponseDTO>> CreateAsync(EmployeeExperience entity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponseDTO<GetExperienceResponseDTO>> GetInfo(GetExperienceRequestDTO dto, string tenantKey)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    } 


}




 
 






