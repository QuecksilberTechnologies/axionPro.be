using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IModuleRepository ModuleRepository { get; }
        IEmployeeLeaveRepository EmployeeLeaveRepository { get; }
        ITenantEmailConfigRepository TenantEmailConfigRepository { get; }
        ITenantEmployeeCodePatternRepository TenantEmployeeCodePatternRepository { get; }
        IInsuranceRepository InsuranceRepository { get; }
        IPolicyTypeInsuranceMappingRepository PolicyTypeInsuranceMappingRepository { get; }
        IPolicyTypeRepository PolicyTypeRepository { get; }
        IPolicyTypeDocumentRepository PolicyTypeDocumentRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IEmployeePolicyEnrollmentRepository EmployeePolicyEnrollmentRepository { get; }
        IEmployeeDependentInsuranceMappingRepository EmployeeDependentInsuranceMappingRepository { get; }
        ITicketThreadRepository TicketThreadRepository { get; }
        ITicketHistoryRepository TicketHistoryRepository { get; }
        IThreadMessageRepository ThreadMessageRepository { get; }
        IReportingTypeRepository ReportingTypeRepository { get; }
        IAssetCategoryRepository AssetCategoryRepository { get; }
        ITicketClassificationRepository TicketClassificationRepository { get; }
        IEmployeeManagerMappingRepository EmployeeManagerMappingRepository { get; }    
        IAssetTypeRepository AssetTypeRepository { get; }
        ITicketHeaderRepository TicketHeaderRepository { get; }
        IWorkflowStagesRepository WorkflowStagesRepository { get; }
        IAssetStatusRepository AssetStatusRepository { get; }
        ITicketTypeRepository TicketTypeRepository { get; }
        ITicketAttachmentRepository TicketAttachmentRepository { get; }
        IUserLoginReopsitory UserLoginRepository { get; }
        IForgotPasswordOtpRepository ForgotPasswordOtpRepository { get; }
        ISandwitchRuleRepository SandwitchRuleRepository { get; }
        ITenantModuleConfigurationRepository TenantModuleConfigurationRepository { get; }
        ITenantEncryptionKeyRepository TenantEncryptionKeyRepository { get; }
        IHolidayCalandarRepository HolidayCalandarRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IStoreProcedureRepository StoreProcedureRepository { get; }
        ILocationRepository LocationRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }
        IPlanModuleMappingRepository PlanModuleMappingRepository { get; }
        ITenantRepository TenantRepository { get; }
        ITenantSubscriptionRepository TenantSubscriptionRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IAssetRepository AssetRepository { get; }
        IOperationRepository OperationRepository { get; }
        IDesignationRepository DesignationRepository { get; }
        IBaseEmployeeRepository Employees { get; }
        IEmployeeContactRepository EmployeeContactRepository { get; }
        IEmployeeBankRepository EmployeeBankRepository { get; }
        IEmployeeEducationRepository EmployeeEducationRepository { get; }

        IEmployeeExperienceRepository EmployeeExperienceRepository { get; }
 
        IEmployeeExperienceDocumentRepository EmployeeExperienceDocumentRepository { get; }
        IEmployeeIdentityRepository EmployeeIdentityRepository { get; }
       
       
        IEmployeeDependentRepository EmployeeDependentRepository { get; }
        ITravelRepository TravelRepository { get; }
        IClientRepository ClientsRepository { get; }
        ICandidateRegistrationRepository CandidatesRegistrationRepository { get; }
        ICandidateCategorySkillRepository CandidateCategorySkillRepository { get; }
        IEmployeeTypeRepository EmployeeTypeRepository { get; }
        IEmailTemplateRepository EmailTemplateRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IRoleRepository RoleRepository { get; }
        ILeaveRepository LeaveRepository { get; }
        ILeaveRuleRepository LeaveRuleRepository { get; }
        IGenderRepository GenderRepository { get; }
        IEmployeeTypeBasicMenuRepository EmployeeTypeBasicMenuRepository { get; }
        IUnStructuredEmployeePolicyTypeMappingRepository UnStructuredEmployeePolicyTypeMappingRepository { get; }
        IUserRolesPermissionOnModuleRepository UserRolesPermissionOnModuleRepository { get; }
         ITicketGenrationRepository TicketGenrationRepository { get; }
 
 
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
       

          
 

    }
}
