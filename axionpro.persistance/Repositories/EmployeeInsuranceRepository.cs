

using AutoMapper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
  
   public class EmployeeInsuranceRepository : IEmployeeInsuranceRepository
    {
        private readonly WorkforcedbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeInsuranceRepository> _logger;
        private readonly IDbContextFactory<WorkforcedbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeInsuranceRepository(WorkforcedbContext context, IMapper mapper, ILogger<EmployeeInsuranceRepository> logger, IDbContextFactory<WorkforcedbContext> contextFactory,
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




 
 






