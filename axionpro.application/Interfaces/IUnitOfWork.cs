using axionpro.application.Interfaces.IContext;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
 
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repositories
      IModuleRepository ModuleRepository { get; }
      IEmployeeLeaveRepository EmployeeLeaveRepository { get; }
      ITenantEmployeeCodePatternRepository TenantEmployeeCodePatternRepository { get; }
     IInsuranceRepository InsuranceRepository { get; }
      IPolicyTypeInsuranceMappingRepository PolicyTypeInsuranceMappingRepository { get; }

    IPermissionRepository PermissionRepository { get; }
       IReportingTypeRepository ReportingTypeRepository { get; }
    IAssetCategoryRepository AssetCategoryRepository { get; }
    ITicketClassificationRepository TicketClassificationRepository { get; }    
     IAssetTypeRepository AssetTypeRepository { get; }
     ITicketHeaderRepository TicketHeaderRepository { get; }
    IWorkflowStagesRepository WorkflowStagesRepository { get; }
    IAssetStatusRepository AssetStatusRepository { get; }
    ITicketTypeRepository TicketTypeRepository { get; }
    IUserLoginReopsitory UserLoginRepository { get; }
    IForgotPasswordOtpRepository ForgotPasswordOtpRepository {  get; }
    ISandwitchRuleRepository SandwitchRuleRepository { get; }
  
    ITenantModuleConfigurationRepository   TenantModuleConfigurationRepository { get; }
    ITenantEncryptionKeyRepository TenantEncryptionKeyRepository { get; }
    IHolidayCalandarRepository HolidayCalandarRepository { get; }

     IDepartmentRepository DepartmentRepository { get; }


    IStoreProcedureRepository StoreProcedureRepository { get; }
    ILocationRepository LocationRepository { get; }
    //
    // INewTokenRepository newTokenRepository { get; } 

    ISubscriptionRepository SubscriptionRepository { get; }
    IPlanModuleMappingRepository PlanModuleMappingRepository { get; }

     ITenantRepository TenantRepository { get; }
    
    ITenantSubscriptionRepository TenantSubscriptionRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
   // IEmployeeRepository EmployeeRepository { get; }
    IAssetRepository AssetRepository { get; }
    IOperationRepository OperationRepository { get; }
    IDesignationRepository  DesignationRepository { get; }
    IBaseEmployeeRepository Employees { get; }
    IEmployeeContactRepository EmployeeContactRepository { get; }
     
    IEmployeeBankRepository EmployeeBankRepository { get; }
    IEmployeeEducationRepository EmployeeEducationRepository { get; }
    IEmployeeExpereinceRepository EmployeeExpereinceRepository { get; }
    IEmployeeIdentityRepository EmployeeIdentityRepository { get; }
    IEmployeeInsuranceRepository EmployeeInsuranceRepository { get; }
    IEmployeeDependentRepository EmployeeDependentRepository { get; }
 
    ITravelRepository TravelRepository { get; }
    IClientRepository ClientsRepository { get; }
    ICandidateRegistrationRepository CandidatesRegistrationRepository { get; }
    ICandidateCategorySkillRepository CandidateCategorySkillRepository { get; }
    IEmployeeTypeRepository EmployeeTypeRepository { get; }
    IEmailTemplateRepository EmailTemplateRepository { get; }
 
    IUserRoleRepository UserRoleRepository { get; }
    ICategoryRepository CategoryRepository { get; }
   // ITenderCategoryRespository TenderCategoryRepository { get; }
    IRoleRepository RoleRepository { get; }
    ILeaveRepository LeaveRepository { get; }
    ILeaveRuleRepository LeaveRuleRepository { get; }
    IGenderRepository GenderRepository { get; }
    IEmployeeTypeBasicMenuRepository EmployeeTypeBasicMenuRepository { get; }
 
    IUserRolesPermissionOnModuleRepository UserRolesPermissionOnModuleRepository { get; }
    IModuleOperationMappingRepository ModuleOperationMappingRepository { get; }  


    // Begin a transaction
    Task BeginTransactionAsync();

    // Commit a transaction
    Task CommitTransactionAsync();

    // Rollback a transaction
    Task RollbackTransactionAsync();

    // Save changes asynchronously
    Task<int> CommitAsync();
  
}

