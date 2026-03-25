

using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;



namespace axionpro.persistance.Repositories
{
  
   public class EmployeeInsuranceRepository : IEmployeeInsuranceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeInsuranceRepository> _logger;
       
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeInsuranceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeInsuranceRepository> logger, 
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        
    } 


}




 
 






