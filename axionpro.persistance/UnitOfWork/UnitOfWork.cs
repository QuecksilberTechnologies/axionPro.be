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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class UnitOfWork : IUnitOfWork
{
    private readonly WorkforceDbContext _context;
    private readonly IQRService _qrService;
    private readonly IMapper _mapper;
    private readonly IEncryptionService _encriptionService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPermissionService _permissionService;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _config;
    private readonly ICacheService _cacheService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<UnitOfWork> _logger;

    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    private IForgotPasswordOtpRepository? _forgotPasswordOtpRepository;
    private ISandwitchRuleRepository? _sandwitchRuleRepository;
    private ILeaveRuleRepository? _leaveRuleRepository;
    private IInsuranceRepository? _insuranceRepository;
    private IPolicyTypeRepository? _policyTypeRepository;
    private ICompanyPolicyDocumentRepository? _companyPolicyDocumentRepository;
    private ITenantEmailConfigRepository? _tenantEmailConfigRepository;
    private IHolidayCalandarRepository? _holidayCalandarRepository;
    private ITenantModuleConfigurationRepository? _tenantModuleConfigurationRepository;
    private IPlanModuleMappingRepository? _planModuleMappingRepository;
    private ILeaveRepository? _leaveRepository;
    private ITicketTypeRepository? _ticketTypeRepository;
    private IReportingTypeRepository? _reportingTypeRepository;
    private IAssetStatusRepository? _assetStatusRepository;
    private IAssetTypeRepository? _assetTypeRepository;
    private IAssetCategoryRepository? _assetCategoryRepository;
    private ITicketClassificationRepository? _ticketClassificationRepository;
    private ITicketHeaderRepository? _ticketHeaderRepository;
    private IBaseEmployeeRepository? _baseEmployeeRepository;
    private IEmployeeContactRepository? _employeeContactRepository;
    private IEmployeeBankRepository? _employeeBankRepository;
    private IEmployeeEducationRepository? _employeeEducationRepository;
    private IEmployeeExperienceRepository? _employeeExpereinceRepository;
    private IEmployeeExperienceDetailRepository? _employeeExperienceDetailRepository;
    private IEmployeeExperienceDocumentRepository? _employeeExpereinceDocumentRepository;
    private IEmployeeIdentityRepository? _employeeIdentityRepository;
    private IEmployeeInsuranceRepository? _employeeInsuranceRepository;
    private IEmployeeDependentRepository? _employeeDependentRepository;
    private IPolicyTypeInsuranceMappingRepository? _policyTypeInsuranceMappingRepository;
    private IWorkflowStagesRepository? _workflowStagesRepository;
    private ITenantRepository? _tenantRepository;
    private IUserRoleRepository? _userRoleRepository;
    private IRoleRepository? _roleRepository;
    private IEmployeeTypeRepository? _employeeTypeRepository;
    private ITenantEmployeeCodePatternRepository? _tenantEmployeeCodePatternRepository;
    private ICategoryRepository? _categoryRepository;
    private IEmployeeTypeBasicMenuRepository? _employeeTypeBasicMenuRepository;
    private IUserRolesPermissionOnModuleRepository? _userRolesPermissionOnModuleRepository;
    private ICandidateRegistrationRepository? _candidateRegistrationRepository;
    private ICandidateCategorySkillRepository? _candidateCategorySkillRepository;
    private ILocationRepository? _locationRepository;
    private IEmployeeLeaveRepository? _employeeLeaveRepository;
    private IAssetRepository? _assetRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private IStoreProcedureRepository? _commonRepository;
    private IUserLoginReopsitory? _userLoginRepository;
    private IEmailTemplateRepository? _emailTemplateRepository;
    private ISubscriptionRepository? _subscriptionRepository;
    private ITenantSubscriptionRepository? _tenantSubscriptionRepository;
    private IModuleRepository? _moduleRepository;
    private IModuleOperationMappingRepository? _moduleOperationMappingRepository;
    private IGenderRepository? _genderRepository;
    private ITenantEncryptionKeyRepository? _tenantEncryptionKeyRepository;
    private IPermissionRepository? _permissionRepository;
    private IDepartmentRepository? _departmentRepository;
    private IDesignationRepository? _designationRepository;
    private IOperationRepository? _operationRepository;
    private ITravelRepository? _travelRepository;
    private IClientRepository? _clientRepository;

    public UnitOfWork(
        WorkforceDbContext context,
        ILoggerFactory loggerFactory,
        IQRService qrService,
        IFileStorageService fileStorageService,
        IMapper mapper,
        IPasswordService passwordService,
        IConfiguration configuration,
        IPermissionService permissionService,
        ICacheService cacheService,
        IEncryptionService encryptionService,
        IIdEncoderService idEncoderService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = _loggerFactory.CreateLogger<UnitOfWork>();
        _qrService = qrService ?? throw new ArgumentNullException(nameof(qrService));
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _encriptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _idEncoderService = idEncoderService ?? throw new ArgumentNullException(nameof(idEncoderService));
    }

   
    public IUserLoginReopsitory UserLoginRepository =>
        _userLoginRepository ??= new UserLoginReopsitory(_context, _loggerFactory.CreateLogger<UserLoginReopsitory>(), _mapper, _passwordService, _config);

    public IPermissionRepository PermissionRepository =>
        _permissionRepository ??= new PermissionRepository(_context, _loggerFactory.CreateLogger<PermissionRepository>(), _cacheService);

    public ITicketTypeRepository TicketTypeRepository =>
        _ticketTypeRepository ??= new TicketTypeRepository(_context, _loggerFactory.CreateLogger<TicketTypeRepository>(), _mapper);

    public IBaseEmployeeRepository Employees =>
        _baseEmployeeRepository ??= new BaseEmployeeRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<BaseEmployeeRepository>(),
            _passwordService,
            _encriptionService);

    public ITenantEmployeeCodePatternRepository TenantEmployeeCodePatternRepository =>
        _tenantEmployeeCodePatternRepository ??= new TenantEmployeeCodePatternRepository(
            _context,
            _loggerFactory.CreateLogger<TenantEmployeeCodePatternRepository>());

    public IEmployeeContactRepository EmployeeContactRepository =>
        _employeeContactRepository ??= new EmployeeContactRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeContactRepository>(),
            _passwordService,
            _encriptionService);

    public IEmployeeEducationRepository EmployeeEducationRepository =>
        _employeeEducationRepository ??= new EmployeeEducationRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeEducationRepository>(),
            _passwordService,
            _encriptionService, _fileStorageService);
    public IEmployeeExperienceRepository EmployeeExperienceRepository =>      
        _employeeExpereinceRepository ??= new EmployeeExpereinceRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeExpereinceRepository>(),
            _passwordService,
            _encriptionService,_fileStorageService);
    
    public IEmployeeExperienceDocumentRepository EmployeeExperienceDocumentRepository =>
        _employeeExpereinceDocumentRepository ??= new EmployeeExperienceDocumentRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeExperienceDocumentRepository>(),
            _passwordService,
            _encriptionService);

    
    public IEmployeeIdentityRepository EmployeeIdentityRepository =>
        _employeeIdentityRepository ??= new EmployeeIdentityRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeIdentityRepository>(),
            _passwordService,
            _encriptionService);

    public IEmployeeInsuranceRepository EmployeeInsuranceRepository =>
        _employeeInsuranceRepository ??= new EmployeeInsuranceRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeInsuranceRepository>(),
            _passwordService,
            _encriptionService);

    public IPolicyTypeRepository PolicyTypeRepository =>
        _policyTypeRepository ??= new PolicyTypeRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<PolicyTypeRepository>(),
            _passwordService,
            _encriptionService);

    public ICompanyPolicyDocumentRepository CompanyPolicyDocumentRepository =>
        _companyPolicyDocumentRepository ??= new CompanyPolicyDocumentRepository(
            _context,
            _loggerFactory.CreateLogger<CompanyPolicyDocumentRepository>(),
            _mapper,
            _encriptionService);

    public IEmployeeBankRepository EmployeeBankRepository =>
        _employeeBankRepository ??= new EmployeeBankRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeBankRepository>(),
            _passwordService,
            _encriptionService, _fileStorageService);

    public IEmployeeLeaveRepository EmployeeLeaveRepository =>
        _employeeLeaveRepository ??= new EmployeeLeaveRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<EmployeeLeaveRepository>(),
            _passwordService,
            _encriptionService);

    public IPolicyTypeInsuranceMappingRepository PolicyTypeInsuranceMappingRepository =>
        _policyTypeInsuranceMappingRepository ??= new PolicyTypeInsuranceMappingRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<PolicyTypeInsuranceMappingRepository>(),
            _passwordService,
            _encriptionService);

    public IInsuranceRepository InsuranceRepository =>
        _insuranceRepository ??= new InsuranceRepository(
            _context,
            _mapper,
            _loggerFactory.CreateLogger<InsuranceRepository>(),
            _passwordService,
            _encriptionService);

    public IModuleOperationMappingRepository ModuleOperationMappingRepository =>
        _moduleOperationMappingRepository ??= new ModuleOperationMappingRepository(
            _context,
            _loggerFactory.CreateLogger<ModuleOperationMappingRepository>(),
            _mapper);

    public IGenderRepository GenderRepository =>
        _genderRepository ??= new GenderRepository(_context, _loggerFactory.CreateLogger<GenderRepository>(), _mapper);

    public ILocationRepository LocationRepository =>
        _locationRepository ??= new LocationRepository(_context, _loggerFactory.CreateLogger<LocationRepository>(), _mapper);

    public IHolidayCalandarRepository HolidayCalandarRepository =>
        _holidayCalandarRepository ??= new HolidayCalandarRepository(_context, _loggerFactory.CreateLogger<HolidayCalandarRepository>());

    public ITenantRepository TenantRepository =>
        _tenantRepository ??= new TenantRepository(_context, _loggerFactory.CreateLogger<TenantRepository>());

    public IForgotPasswordOtpRepository ForgotPasswordOtpRepository =>
        _forgotPasswordOtpRepository ??= new ForgotPasswordOtpRepository(_context, _loggerFactory.CreateLogger<ForgotPasswordOtpRepository>());

    public ILeaveRuleRepository LeaveRuleRepository =>
        _leaveRuleRepository ??= new LeaveRuleRepository(_context, _loggerFactory.CreateLogger<LeaveRuleRepository>());

    public IPlanModuleMappingRepository PlanModuleMappingRepository =>
        _planModuleMappingRepository ??= new PlanModuleMappingRepository(_context, _loggerFactory.CreateLogger<PlanModuleMappingRepository>());

    public IEmailTemplateRepository EmailTemplateRepository =>
        _emailTemplateRepository ??= new EmailTemplateRepository(_context, _loggerFactory.CreateLogger<EmailTemplateRepository>());

    public ICandidateRegistrationRepository CandidatesRegistrationRepository =>
        _candidateRegistrationRepository ??= new CandidateRegistrationRepository(_context, _loggerFactory.CreateLogger<CandidateRegistrationRepository>());

    public ICandidateCategorySkillRepository CandidateCategorySkillRepository =>
        _candidateCategorySkillRepository ??= new CandidateCategorySkillRepository(_context, _loggerFactory.CreateLogger<CandidateCategorySkillRepository>());

    public IEmployeeTypeRepository EmployeeTypeRepository =>
        _employeeTypeRepository ??= new EmployeeTypeRepository(_context, _loggerFactory.CreateLogger<EmployeeTypeRepository>());

    public IClientRepository ClientsRepository =>
        _clientRepository ??= new ClientRepository(_context, _loggerFactory.CreateLogger<ClientRepository>());

    public ITenantSubscriptionRepository TenantSubscriptionRepository =>
        _tenantSubscriptionRepository ??= new TenantSubscriptionRepository(_context, _loggerFactory.CreateLogger<TenantSubscriptionRepository>(), _mapper);

    public IRefreshTokenRepository RefreshTokenRepository =>
        _refreshTokenRepository ??= new RefreshTokenRepository(_context, _loggerFactory.CreateLogger<RefreshTokenRepository>());

    public ISubscriptionRepository SubscriptionRepository =>
        _subscriptionRepository ??= new SubscriptionRepository(_context, _loggerFactory.CreateLogger<SubscriptionRepository>(), _mapper);

    public IEmployeeTypeBasicMenuRepository EmployeeTypeBasicMenuRepository =>
        _employeeTypeBasicMenuRepository ??= new EmployeeTypeBasicMenuRepository(_context, _loggerFactory.CreateLogger<EmployeeTypeBasicMenuRepository>(), _mapper);

    public IStoreProcedureRepository StoreProcedureRepository =>
        _commonRepository ??= new StoreProcedureRepository(_context, _loggerFactory.CreateLogger<StoreProcedureRepository>(), _mapper);

    public IModuleRepository ModuleRepository =>
        _moduleRepository ??= new ModuleRepository(_context, _loggerFactory.CreateLogger<ModuleRepository>(), _mapper);

    public IAssetRepository AssetRepository =>
        _assetRepository ??= new AssetRepository(_context, _loggerFactory.CreateLogger<AssetRepository>(), _qrService, _fileStorageService, _mapper);

    public ITravelRepository TravelRepository =>
        _travelRepository ??= new TravelRepository(_context, _loggerFactory.CreateLogger<TravelRepository>());

    public IOperationRepository OperationRepository =>
        _operationRepository ??= new OperationRepository(_context, _loggerFactory.CreateLogger<OperationRepository>());

    public ICategoryRepository CategoryRepository =>
        _categoryRepository ??= new CategoryRepository(_context, _loggerFactory.CreateLogger<CategoryRepository>());

    public IUserRoleRepository UserRoleRepository =>
        _userRoleRepository ??= new UserRoleRepository(_context, _loggerFactory.CreateLogger<UserRoleRepository>());

    public IUserRolesPermissionOnModuleRepository UserRolesPermissionOnModuleRepository =>
        _userRolesPermissionOnModuleRepository ??= new UserRolesPermissionOnModuleRepository(_context, _loggerFactory.CreateLogger<UserRolesPermissionOnModuleRepository>());

    public ILeaveRepository LeaveRepository =>
        _leaveRepository ??= new LeaveRepository(_context, _loggerFactory.CreateLogger<LeaveRepository>());

    public IWorkflowStagesRepository WorkflowStagesRepository =>
        _workflowStagesRepository ??= new WorkflowStageRepository(_context, _loggerFactory.CreateLogger<WorkflowStageRepository>(), _mapper);

    public ITenantEncryptionKeyRepository TenantEncryptionKeyRepository =>
        _tenantEncryptionKeyRepository ??= new TenantEncryptionKeyRepository(_context, _loggerFactory.CreateLogger<TenantEncryptionKeyRepository>());

    public ITenantModuleConfigurationRepository TenantModuleConfigurationRepository =>
        _tenantModuleConfigurationRepository ??= new TenantModuleConfigurationRepository(_context, _loggerFactory.CreateLogger<TenantModuleConfigurationRepository>(), _mapper);

    public IReportingTypeRepository ReportingTypeRepository =>
        _reportingTypeRepository ??= new ReportingTypeRepository(_context, _loggerFactory.CreateLogger<ReportingTypeRepository>(), _mapper);

    public IAssetStatusRepository AssetStatusRepository =>
        _assetStatusRepository ??= new AssetStatusRepository(_context, _loggerFactory.CreateLogger<AssetStatusRepository>(), _mapper);

    public IAssetCategoryRepository AssetCategoryRepository =>
        _assetCategoryRepository ??= new AssetCategoryRepository(_context, _loggerFactory.CreateLogger<AssetCategoryRepository>(), _mapper);

    public ITenantEmailConfigRepository TenantEmailConfigRepository =>
        _tenantEmailConfigRepository ??= new TenantEmailConfigRepository(_context, _loggerFactory.CreateLogger<TenantEmailConfigRepository>());

    public IDesignationRepository DesignationRepository =>
        _designationRepository ??= new DesignationRepository(_context, _loggerFactory.CreateLogger<DesignationRepository>(), _mapper);

    public IDepartmentRepository DepartmentRepository =>
        _departmentRepository ??= new DepartmentRepository(_context, _loggerFactory.CreateLogger<DepartmentRepository>(), _mapper, _encriptionService);

    public IRoleRepository RoleRepository =>
        _roleRepository ??= new RoleRepository(_context, _loggerFactory.CreateLogger<RoleRepository>(), _mapper, _encriptionService);

    public ITicketClassificationRepository TicketClassificationRepository =>
        _ticketClassificationRepository ??= new TicketClassificationRepository(_context, _loggerFactory.CreateLogger<TicketClassificationRepository>(), _mapper);

    public IAssetTypeRepository AssetTypeRepository =>
        _assetTypeRepository ??= new AssetTypeRepository(_context, _loggerFactory.CreateLogger<AssetTypeRepository>(), _mapper);

    public ITicketHeaderRepository TicketHeaderRepository =>
        _ticketHeaderRepository ??= new TicketHeaderRepository(_context, _loggerFactory.CreateLogger<TicketHeaderRepository>(), _mapper);

    public ISandwitchRuleRepository SandwitchRuleRepository =>
        _sandwitchRuleRepository ??= new SandwitchRuleRepository(_mapper, _context, _loggerFactory.CreateLogger<SandwitchRuleRepository>());

    public IEmployeeDependentRepository EmployeeDependentRepository => throw new NotImplementedException();


    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _currentTransaction?.Dispose();
        _context.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private async Task SafeRollbackInternalAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rolling back transaction.");
            throw;
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
    
   
 

}