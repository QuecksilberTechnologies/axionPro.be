using AutoMapper;

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICacheService;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
 
public class UnitOfWork : IUnitOfWork
{



    private readonly WorkforceDbContext _context;

    private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
    private readonly IQRService _qrService;
 
    private readonly IMapper _mapper;
    private readonly IEncryptionService _encriptionService;
    private readonly IIdEncoderService  _idEncoderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPermissionService _permissionService;
    
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _config;
    private readonly ICacheService _cacheService;
    private readonly ILoggerFactory _loggerFactory;
    private IDbContextTransaction? _currentTransaction;
    private IForgotPasswordOtpRepository _forgotPasswordOtpRepository;
    private ISandwitchRuleRepository _sandwitchRuleRepository;
    private ILeaveRuleRepository _leaveRuleRepository;

  

   
    private IHolidayCalandarRepository _holidayCalandarRepository;
    private ITenantModuleConfigurationRepository? _tenantModuleConfigurationRepository;
    private IPlanModuleMappingRepository? _planModuleMappingRepository;
    private ILeaveRepository? _leaveRepository;
    private ITicketTypeRepository? _ticketTypeRepository;
    private IReportingTypeRepository? _reportingTypeRepository;
    private IAssetStatusRepository? _assetStatusRepository;
    private IAssetTypeRepository? _assetTypeRepository;
    private IAssetCategoryRepository? _assetCategoryRepository;
    private ITicketClassificationRepository? _ticket_ClassificationRepository;
    private ITicketHeaderRepository? _ticketHeaderRepository;
    private IBaseEmployeeRepository? _baseEmployeeContactRepository;
    private IEmployeeContactRepository? _employeeContactRepository;
    private IEmployeeBankRepository? _employeeBankRepository;
    private IEmployeeEducationRepository? _employeeEducationRepository;
    private IEmployeeExpereinceRepository? _employeeExpereinceRepository;
    private IEmployeeIdentityRepository? _employeeIdentityRepository;
    private IEmployeeInsuranceRepository? _employeeInsuranceRepository;
    private IEmployeeDependentRepository? _employeeDependentRepository;

    private IWorkflowStagesRepository? _workflowStagesRepository;
    private ITenantRepository? _tenantRepository;
   
   
    private IUserRoleRepository? _userRoleRepository;
    private IRoleRepository? _roleRepository;
    private IEmployeeTypeRepository? _employeeTyperepository;
    private ICategoryRepository? _categoryRepository;
    private IEmployeeTypeBasicMenuRepository? _employeeTypeBasicMenurepository;
    private IUserRolesPermissionOnModuleRepository? _userRolesPermissionOnModuleRepository;
    private IAttendanceRepository? _attendanceRepository;
    private ICandidateRegistrationRepository? _candidateRegistrationRepository;
    private ICandidateCategorySkillRepository? _candidateCategorySkillRepository;
    private ILocationRepository? _countryRepository;
    private IEmployeeLeaveRepository? _employeeLeaveRepository;


    private IAssetRepository? _assetRepository;
    //private  INewTokenRepository _tokenService;
    private IRefreshTokenRepository _refreshTokenRepository;
    private ICommonRepository _commonRepository;
    private IUserLoginReopsitory? _userLoginReopsitory;
    private IEmailTemplateRepository _emailTemplateRepository;
    private ISubscriptionRepository? _subscriptionRepository;
    private ITenantSubscriptionRepository? _tenantSubscriptionRepository;
    private IModuleRepository? _moduleRepository;
    private IModuleOperationMappingRepository _moduleOperationMappingRepository;
    private IGenderRepository? _genderRepository;
    private ITenantEncryptionKeyRepository? _tenantEncryptionKeyRepository;
    private  IPermissionRepository _permissionRepository;


    // private IClientRepository? _clientRepository;



    //public UnitOfWork(WorkforceDbContext context, IMapper mapper, ILoggerFactory loggerFactory)
    //{
    //    _context = context;
    //   // _mapper = mapper;
    //    _loggerFactory = loggerFactory;
    //}
    public UnitOfWork(
        WorkforceDbContext context,
        ILoggerFactory loggerFactory,
        IQRService qrService,
        IFileStorageService fileStorageService, IMapper mapper, IDbContextFactory<WorkforceDbContext> dbContextFactory, 
        IPasswordService passwordService, IConfiguration configuration,IPasswordService password , IPermissionService permissionService, ICacheService cacheService,
        IEncryptionService encryptionService, IIdEncoderService idEncoderService)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        _qrService = qrService;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _contextFactory = dbContextFactory;
         _passwordService = passwordService;
        _config = configuration;
        _permissionService = permissionService;
        _cacheService = cacheService;
        _encriptionService = encryptionService;
        _idEncoderService = idEncoderService;

    }

    // Repositories
    //public IAttendanceRepository AttendanceReopsitory
    //{
    //    get
    //    {
    //        return _attendanceRepository ??= new AttendanceRepository(_context, _loggerFactory.CreateLogger<AttendanceRepository>());
    //    }
    //}




    public IUserLoginReopsitory UserLoginReopsitory
    {
        get
        {
            return _userLoginReopsitory ??= new UserLoginReopsitory(_context, _loggerFactory.CreateLogger<UserLoginReopsitory>(), _mapper, _contextFactory, _passwordService,_config);
        }
    }

    public IPermissionRepository PermissionRepository
    {
        get
        {
            return _permissionRepository ??= new PermissionRepository(_contextFactory, _loggerFactory.CreateLogger<PermissionRepository>(), _cacheService);
        }
    }


    public ITicketTypeRepository TicketTypeRepository
    {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _ticketTypeRepository ??= new TicketTypeRepository(_context, _loggerFactory.CreateLogger<TicketTypeRepository>(), _mapper
               , _contextFactory
                );
        }
    }



    public IBaseEmployeeRepository Employees
    {
        get
        {
            return _baseEmployeeContactRepository ??= new BaseEmployeeRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<BaseEmployeeRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }
    
   

    public IEmployeeContactRepository EmployeeContactRepository
    {
        get
        {
            return _employeeContactRepository ??= new EmployeeContactRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeContactRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }



    public IEmployeeEducationRepository EmployeeEducationRepository
    {
        get
        {
            return _employeeEducationRepository ??= new EmployeeEducationRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeEducationRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }


    public IEmployeeExpereinceRepository EmployeeExpereinceRepository
    {
        get
        {
            return _employeeExpereinceRepository ??= new EmployeeExpereinceRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeExpereinceRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }

    public IEmployeeIdentityRepository EmployeeIdentityRepository
    {
        get
        {
            return _employeeIdentityRepository ??= new EmployeeIdentityRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeIdentityRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }

    public IEmployeeInsuranceRepository EmployeeInsuranceRepository
    {
        get
        {
            return _employeeInsuranceRepository ??= new EmployeeInsuranceRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeInsuranceRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }
 
    public IEmployeeBankRepository EmployeeBankRepository
    {
        get
        {
            return _employeeBankRepository ??= new EmployeeBankRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeBankRepository>(),
                _contextFactory,
                _passwordService, _encriptionService

            );
        }
    }
    
      
         public IEmployeeLeaveRepository EmployeeLeaveRepository
    {
        get
        {
            return _employeeLeaveRepository ??= new EmployeeLeaveRepository( _context,   _mapper,   _loggerFactory.CreateLogger<EmployeeLeaveRepository>(),
                _contextFactory, _passwordService, _encriptionService
            );
        }
    }
    public IEmployeeBankRepository EmployeeBanks
    {
        get
        {
            return _employeeBankRepository ??= new EmployeeBankRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeBankRepository>(),
                _contextFactory, _passwordService, _encriptionService
            );
        }
    }

    public IEmployeeEducationRepository EmployeeEducations
    {
        get
        {
            return _employeeEducationRepository ??= new EmployeeEducationRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeEducationRepository>(),
                _contextFactory, _passwordService, _encriptionService
            );
        }
    }
   
    public IEmployeeDependentRepository EmployeeDependentRepository
    {
        get
        {
            return _employeeDependentRepository ??= new EmployeeDependentRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeDependentRepository>(),
                _contextFactory, _passwordService, _encriptionService
            );
        }
    }
    public IEmployeeExpereinceRepository EmployeeExperiences
    {
        get
        {
            return _employeeExpereinceRepository ??= new EmployeeExpereinceRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeExpereinceRepository>(),
                _contextFactory, _passwordService, _encriptionService
            );
        }
    }

    public IEmployeeIdentityRepository EmployeeIdentities
    {
        get
        {
            return _employeeIdentityRepository ??= new EmployeeIdentityRepository(
                _context,
                _mapper,
                _loggerFactory.CreateLogger<EmployeeIdentityRepository>(),
                _contextFactory, _passwordService, _encriptionService



            );
        }
    }

    //public IEmployeeInsuranceRepository EmployeeInsurances
    //{
    //    get
    //    {
    //        return _employeeInsuranceRepository ??= new EmployeeInsuranceRepository(
    //            _context,
    //            _mapper,
    //            _loggerFactory.CreateLogger<EmployeeInsuranceRepository>(),
    //            _contextFactory, _passwordService
                
    //        );
    //    }
    //}
    public IModuleOperationMappingRepository ModuleOperationMappingRepository
    {
        get
        {
            return _moduleOperationMappingRepository ??= new ModuleOperationMappingRepository(_context, _loggerFactory.CreateLogger<ModuleOperationMappingRepository>(), _mapper, _contextFactory);
        }
    }
    public IGenderRepository GenderRepository
    {
        get
        {
            return _genderRepository ??= new GenderRepository(_context, _loggerFactory.CreateLogger<GenderRepository>(), _mapper, _contextFactory);
        }
    }
    public ILocationRepository LocationRepository
    {
        get
        {
            return _countryRepository ??= new LocationRepository(_context, _loggerFactory.CreateLogger<LocationRepository>(), _mapper, _contextFactory);
        }
    }
    public IHolidayCalandarRepository HolidayCalandarRepository
    {
        get
        {
            return _holidayCalandarRepository ??= new HolidayCalandarRepository(_context, _loggerFactory.CreateLogger<HolidayCalandarRepository>());
        }
    }
    public ITenantRepository TenantRepository
    {
        get
        {
            return _tenantRepository ??= new TenantRepository(_context, _loggerFactory.CreateLogger<TenantRepository>());
        }
    }



    public IForgotPasswordOtpRepository ForgotPasswordOtpRepository
    {
        get
        {
            return _forgotPasswordOtpRepository ??= new ForgotPasswordOtpRepository(_context, _loggerFactory.CreateLogger<ForgotPasswordOtpRepository>());
        }
    }
   
    /// <summary>
    /// UnitOfWork
    /// </summary>
   


    public ILeaveRuleRepository LeaveRuleRepository
    {
        get
        {
            return _leaveRuleRepository ??= new LeaveRuleRepository(_context, _loggerFactory.CreateLogger<LeaveRuleRepository>());
        }
    }



  

    public IPlanModuleMappingRepository PlanModuleMappingRepository
    {
        get
        {
            return _planModuleMappingRepository ??= new PlanModuleMappingRepository(_context, _loggerFactory.CreateLogger<PlanModuleMappingRepository>());
        }
    }



  

    public IEmailTemplateRepository EmailTemplateRepository
    {
        get
        {
            return _emailTemplateRepository ??= new EmailTemplateRepository(_context, _loggerFactory.CreateLogger<EmailTemplateRepository>());
        }
    }

    public ICandidateRegistrationRepository CandidatesRegistrationRepository
    {
        get
        {
            return _candidateRegistrationRepository ??= new CandidateRegistrationRepository(_context, _loggerFactory.CreateLogger<CandidateRegistrationRepository>());
        }
    }

    public ICandidateCategorySkillRepository CandidateCategorySkillRepository
    {
        get
        {
            return _candidateCategorySkillRepository ??= new CandidateCategorySkillRepository(_context, _loggerFactory.CreateLogger<CandidateCategorySkillRepository>());
        }
    }


  
    public IEmployeeTypeRepository EmployeeTypeRepository
    {
        get
        {
            return _employeeTyperepository ??= new EmployeeTypeRepository(_context, _loggerFactory.CreateLogger<EmployeeTypeRepository>());
        }
    }
    public IClientRepository ClientsRepository => new ClientRepository(_context, _loggerFactory.CreateLogger<ClientRepository>());
   
    //public INewTokenRepository NewTokenRepository
    //{
    //    get
    //    {
    //        return _tokenService ??= new NewTokenRepository(_context, _loggerFactory.CreateLogger<NewTokenRepository>());
    //    }
    //}
    //private INewTokenRepository _tokenService;
    //private IRefreshTokenRepository _refreshTokenRepository;
    //private ICommonRepository _commonRepository;
    //private IUserLoginReopsitory? _userLoginReopsitory;
    //public ICommonRepository CommonRepository => new CommonRepository(_context, _loggerFactory.CreateLogger<CommonRepository>());

    public IUserLoginReopsitory UserLoginRepository => new UserLoginReopsitory(_context, _loggerFactory.CreateLogger<UserLoginReopsitory>(), _mapper, _contextFactory, _passwordService, _config );

    public ITenantSubscriptionRepository TenantSubscriptionRepository
    {
        get
        {
            return _tenantSubscriptionRepository ??= new TenantSubscriptionRepository(_context, _loggerFactory.CreateLogger<TenantSubscriptionRepository>(), _mapper, _contextFactory);
        }
    }
    public IRefreshTokenRepository RefreshTokenRepository => new RefreshTokenRepository(_context, _loggerFactory.CreateLogger<RefreshTokenRepository>());
    public ISubscriptionRepository SubscriptionRepository => new SubscriptionRepository(_context, _loggerFactory.CreateLogger<SubscriptionRepository>(), _mapper);

   
    public IEmployeeTypeBasicMenuRepository EmployeeTypeBasicMenuRepository
    {
        get
        {
            return _employeeTypeBasicMenurepository ??= new EmployeeTypeBasicMenuRepository(_context, _loggerFactory.CreateLogger<EmployeeTypeBasicMenuRepository>(), _mapper, _contextFactory);
        }
    }
    public ICommonRepository CommonRepository
    {
        get
        {
            return _commonRepository ??= new CommonRepository(_context, _loggerFactory.CreateLogger<CommonRepository>(), _mapper, _contextFactory);
        }
    }
    public IModuleRepository ModuleRepository
    {
        get
        {
            return _moduleRepository ??= new ModuleRepository(_context, _loggerFactory.CreateLogger<ModuleRepository>(), _mapper, _contextFactory);
        }
    }
    public IAssetRepository AssetRepository => new AssetRepository(_context, _loggerFactory.CreateLogger<AssetRepository>(), 
         _qrService, _fileStorageService,
          _mapper,  _contextFactory  
    );



    public ITravelRepository TravelRepository => new TravelRepository(_context, _loggerFactory.CreateLogger<TravelRepository>());

    public IOperationRepository OperationRepository => new OperationRepository(_context, _loggerFactory.CreateLogger<OperationRepository>());
  

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository ??= new CategoryRepository(_context, _loggerFactory.CreateLogger<CategoryRepository>());
        }
    }


    public IUserRoleRepository UserRoleRepository
    {
        get
        {
            return _userRoleRepository ??= new UserRoleRepository(_context, _loggerFactory.CreateLogger<UserRoleRepository>());
        }
    }

    public IUserRolesPermissionOnModuleRepository UserRolesPermissionOnModuleRepository
    {
        get
        {
            return _userRolesPermissionOnModuleRepository ??= new UserRolesPermissionOnModuleRepository(_context, _loggerFactory.CreateLogger<UserRolesPermissionOnModuleRepository>());
        }
    }

   
    public ILeaveRepository LeaveRepository
    {
        get
        {
            return _leaveRepository ??= new LeaveRepository(_context, _loggerFactory.CreateLogger<LeaveRepository>());


        }
    }
    //public ITicketHeaderRepository TicketHeaderRepository
    //{
    //    get
    //    {
    //        return _ticketHeaderRepository ??= new TicketHeaderRepository(_context, _loggerFactory.CreateLogger<TicketHeaderRepository>());


    //    }
    //}

    public IWorkflowStagesRepository WorkflowStagesRepository
    {
        get
        {
            return _workflowStagesRepository ??= new WorkflowStageRepository(
                _context,
                _loggerFactory.CreateLogger<WorkflowStageRepository>(),  // ✅ only type needed
                _mapper,
                _contextFactory
            );
        }
    }
    public ITenantEncryptionKeyRepository TenantEncryptionKeyRepository
    {
        get
        {
            // Lazy initialization with all required dependencies
            return _tenantEncryptionKeyRepository ??= new TenantEncryptionKeyRepository(
                _context,         // DbContext
                _mapper,          // IMapper
                _loggerFactory.CreateLogger<TenantEncryptionKeyRepository>(), // ILogger
                _contextFactory   // IDbContextFactory
            );
        }
    }

    public ITenantModuleConfigurationRepository TenantModuleConfigurationRepository
    {
        get
        {
            return _tenantModuleConfigurationRepository ??= new TenantModuleConfigurationRepository(_context, _loggerFactory.CreateLogger<TenantModuleConfigurationRepository>(), _mapper, _contextFactory);
        }
    }
    
    public IReportingTypeRepository ReportingTypeRepository {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _reportingTypeRepository ??= new ReportingTypeRepository(_context, _loggerFactory.CreateLogger<ReportingTypeRepository>(), _mapper, _contextFactory); }
    }
    public IAssetStatusRepository AssetStatusRepository
    {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _assetStatusRepository ??= new AssetStatusRepository(_context, _loggerFactory.CreateLogger<AssetStatusRepository>(), _mapper, _contextFactory);
        }
    }
    public IAssetCategoryRepository AssetCategoryRepository
    {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _assetCategoryRepository ??= new AssetCategoryRepository(_context, _loggerFactory.CreateLogger<AssetCategoryRepository>(), _mapper, _contextFactory);
        }
    }
     
    


    public IDesignationRepository DesignationRepository => new DesignationRepository(_context, _loggerFactory.CreateLogger<DesignationRepository>(), _mapper, _contextFactory);
    public IDepartmentRepository DepartmentRepository => new DepartmentRepository(_context, _loggerFactory.CreateLogger<DepartmentRepository>(), _mapper, _contextFactory, _encriptionService);


    public IRoleRepository RoleRepository
    {
        get
        {
            return _roleRepository ??= new RoleRepository(_context, _loggerFactory.CreateLogger<RoleRepository>(), _mapper, _contextFactory , _encriptionService);
            //   return _roleRepository ??= new CandidateCategorySkillRepository(_context, _loggerFactory.CreateLogger<RoleRepository>());

        }
    }

    public ITicketClassificationRepository TicketClassificationRepository
    {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _ticket_ClassificationRepository ??= new TicketClassificationRepository(_context, _loggerFactory.CreateLogger<TicketClassificationRepository>(), _mapper, _contextFactory);
        }
    }
    public IAssetTypeRepository AssetTypeRepository
    {
        get
        {
            // Agar repository pehle se initialized nahi hai to create karo
            return _assetTypeRepository ??= new AssetTypeRepository(_context, _loggerFactory.CreateLogger<AssetTypeRepository>(), _mapper, _contextFactory);
        }
    }



    public ITicketHeaderRepository TicketHeaderRepository
    {
        get
        {
            // Fix: Use TicketHeaderRepository, not TicketClassificationRepository, and pass correct logger type
            return _ticketHeaderRepository ??= new TicketHeaderRepository(_context, _loggerFactory.CreateLogger<TicketHeaderRepository>(), _mapper, _contextFactory);
        }
    }
 
    public ISandwitchRuleRepository SandwitchRuleRepository
    {
        get
        {
            return _sandwitchRuleRepository ??= new SandwitchRuleRepository(
                _mapper,
                _context,
                _contextFactory,
                _loggerFactory.CreateLogger<SandwitchRuleRepository>()
            );
        }
    }


    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction == null) // Avoid nested transactions
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch (Exception)
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int Complete()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }

 }


