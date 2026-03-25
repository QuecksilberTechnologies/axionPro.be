using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IContext;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;

using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance
{
    public static class ServiceExtentions
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Connection String is null or empty!");

            // common options
            Action<DbContextOptionsBuilder> dbOptions = options =>
                options.UseNpgsql(connectionString)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, LogLevel.Information);

            // ✔ Scoped DbContext
            services.AddDbContext<WorkforceDbContext>(dbOptions);
 

            // ✔ Interface mapping
            services.AddScoped<IWorkforcedbContext>(provider =>
                provider.GetRequiredService<WorkforceDbContext>());



            //// ✅ Connection string check
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            //if (string.IsNullOrEmpty(connectionString))
            //    throw new Exception("Connection String is null or empty!");

            //// ✅ Register DbContextFactory for safe multi-threading
            //services.AddDbContextFactory<WorkforceDbContext>(options =>
            //    options.UseNpgsql(connectionString)
            //           .EnableSensitiveDataLogging()
            //           .EnableDetailedErrors()
            //           .LogTo(Console.WriteLine, LogLevel.Information));

            //// ✅ Register DbContext using AddDbContextPool instead of AddDbContext
            //// DbContextPool is safe for root-level access and prevents "Cannot resolve scoped service" errors
            //// This solves the issue where Swagger/singleton middleware tries to access DbContext
            //services.AddDbContextPool<WorkforceDbContext>(options =>
            //    options.UseNpgsql(connectionString)
            //           .EnableSensitiveDataLogging()
            //           .EnableDetailedErrors()
            //           .LogTo(Console.WriteLine, LogLevel.Information),
            //    poolSize: 128);



            // ✅ Register IWorkforceDbContext (Interface based usage)
            // Use direct implementation registration to avoid resolving scoped DbContext from the root provider
            // which can cause "Cannot resolve scoped service ... from root provider" errors when a singleton
            // tries to capture the interface during startup.
            services.AddScoped<IWorkforcedbContext, WorkforceDbContext>();

            // ✅ Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ✅ All Repositories as Scoped
            services.AddScoped<IStoreProcedureRepository, StoreProcedureRepository>();
            services.AddScoped<ITenantEncryptionKeyRepository, TenantEncryptionKeyRepository>();

            
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IBaseEmployeeRepository, BaseEmployeeRepository>();
 
            services.AddScoped<IUserLoginReopsitory, UserLoginReopsitory>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IEmployeeTypeRepository, EmployeeTypeRepository>();
            services.AddScoped<IEmployeeTypeBasicMenuRepository, EmployeeTypeBasicMenuRepository>();
            services.AddScoped<IUserRolesPermissionOnModuleRepository, UserRolesPermissionOnModuleRepository>();
            services.AddScoped<IEmployeeExperienceRepository, EmployeeExpereinceRepository>();
            services.AddScoped<IEmployeeExperienceDetailRepository, EmployeeExperienceDetailRepository>();
            services.AddScoped<IEmployeeExperienceDocumentRepository, EmployeeExperienceDocumentRepository>();
            services.AddScoped<ICandidateRegistrationRepository, CandidateRegistrationRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ITravelRepository, TravelRepository>();
            services.AddScoped<IOperationRepository, OperationRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<ICandidateCategorySkillRepository, CandidateCategorySkillRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ITenantEmailConfigRepository, TenantEmailConfigRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
            services.AddScoped<IPlanModuleMappingRepository, PlanModuleMappingRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<ITenantModuleConfigurationRepository, TenantModuleConfigurationRepository>();
            services.AddScoped<IModuleOperationMappingRepository, ModuleOperationMappingRepository>();
            services.AddScoped<ICommonServiceSyncRepository, CommonServiceSyncRepository>();
            services.AddScoped<ITenantIndustryRepository, TenantIndustryRepository>();
            services.AddScoped<IPolicyTypeRepository, PolicyTypeRepository>();
            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<ISandwitchRuleRepository, SandwitchRuleRepository>();
            services.AddScoped<ILeaveRuleRepository, LeaveRuleRepository>();
            services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
            services.AddScoped<IWorkflowStagesRepository, WorkflowStageRepository>();
             services.AddScoped<IReportingTypeRepository, ReportingTypeRepository>();
             services.AddScoped<ITicketClassificationRepository, TicketClassificationRepository>();
             services.AddScoped<ITicketHeaderRepository, TicketHeaderRepository>();
             services.AddScoped<ITenantEmployeeCodePatternRepository, TenantEmployeeCodePatternRepository>();
             services.AddScoped<IInsuranceRepository, InsuranceRepository>();
             services.AddScoped<IPolicyTypeInsuranceMappingRepository, PolicyTypeInsuranceMappingRepository>();
             services.AddScoped<ICompanyPolicyDocumentRepository, CompanyPolicyDocumentRepository>();
            
            
            
            
            services.AddScoped<IEmployeeBankRepository, EmployeeBankRepository>();
            services.AddScoped<IEmployeeContactRepository, EmployeeContactRepository>();
            services.AddScoped<IEmployeeIdentityRepository, EmployeeIdentityRepository>();
           
            services.AddScoped<IEmployeeEducationRepository, EmployeeEducationRepository>();
            services.AddScoped<IBaseEmployeeRepository, BaseEmployeeRepository>();
            services.AddScoped<IEmployeeDependentRepository, EmployeeDependentRepository>();
            services.AddScoped<IEmployeeLeaveRepository, EmployeeLeaveRepository>();
            services.AddScoped<TenantEncryptionKeyRepository, TenantEncryptionKeyRepository>();
            services.AddScoped<IEmployeeCompletionRepository, EmployeeCompletionRepository>();
           




            // ✅ Log
            Console.WriteLine("✅ Persistence Layer Configured Successfully.");
        }
    }
}
