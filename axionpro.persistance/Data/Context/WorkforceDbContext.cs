using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Operation;

using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOS.StoreProcedureDTO;
using axionpro.application.Interfaces.IContext;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace axionpro.persistance.Data.Context
{
    public class WorkforceDbContext : DbContext, IWorkforceDbContext
    {


        public WorkforceDbContext(DbContextOptions<WorkforceDbContext> options) : base(options)
        {

        }

        public DbSet<RoleModuleOperationResponseDTO> RoleModuleOperationResponse { get; set; }


        public virtual DbSet<EmployeeContact> EmployeeContacts { get; set; }
        public virtual DbSet<District> Districts { get; set; }

        public virtual DbSet<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignations { get; set; }

        public virtual DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }

        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<AssetAssignment> AssetAssignments { get; set; }

        public virtual DbSet<AssetCategory> AssetCategories { get; set; }

        public virtual DbSet<AssetHistory> AssetHistories { get; set; }

        public virtual DbSet<AssetImage> AssetImages { get; set; }

        public virtual DbSet<AssetStatus> AssetStatuses { get; set; }

        public virtual DbSet<AssetTicketTypeDetail> AssetTicketTypeDetails { get; set; }

        public virtual DbSet<AssetType> AssetTypes { get; set; }

        public virtual DbSet<AssignmentStatus> AssignmentStatuses { get; set; }

        public virtual DbSet<Attendance> Attendances { get; set; }

        public virtual DbSet<AttendanceDeviceType> AttendanceDeviceTypes { get; set; }

        public virtual DbSet<AttendanceHistory> AttendanceHistories { get; set; }

        public virtual DbSet<AttendanceRequest> AttendanceRequests { get; set; }

        public virtual DbSet<BasicMenu> BasicMenus { get; set; }

        public virtual DbSet<Candidate> Candidates { get; set; }

        public virtual DbSet<CandidateCategorySkill> CandidateCategorySkills { get; set; }

        public virtual DbSet<CandidateHistory> CandidateHistories { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<ClientType> ClientTypes { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<DataViewStructure> DataViewStructures { get; set; }

        public virtual DbSet<DayCombination> DayCombinations { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Designation> Designations { get; set; }

        public virtual DbSet<EmailQueue> EmailQueues { get; set; }

        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

        public virtual DbSet<EmailsLog> EmailsLogs { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; }

        public virtual DbSet<EmployeeCategorySkill> EmployeeCategorySkills { get; set; }

        public virtual DbSet<EmployeeDailyAttendance> EmployeeDailyAttendances { get; set; }

        public virtual DbSet<EmployeeDependent> EmployeeDependents { get; set; }

        public virtual DbSet<EmployeeEducation> EmployeeEducations { get; set; }

        public virtual DbSet<EmployeeExperience> EmployeeExperiences { get; set; }

        public virtual DbSet<EmployeeExperienceDetail> EmployeeExperienceDetails { get; set; }

        public virtual DbSet<EmployeeExperiencePayslipUpload> EmployeeExperiencePayslipUploads { get; set; }

        public virtual DbSet<EmployeeImage> EmployeeImages { get; set; }

        public virtual DbSet<EmployeeInsurancePolicy> EmployeeInsurancePolicies { get; set; }

        public virtual DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }

        public virtual DbSet<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMappings { get; set; }

        public virtual DbSet<EmployeeManagerMapping> EmployeeManagerMappings { get; set; }

        public virtual DbSet<EmployeePersonalDetail> EmployeePersonalDetails { get; set; }

        public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }

        public virtual DbSet<EmployeeTypeBasicMenu> EmployeeTypeBasicMenus { get; set; }

        public virtual DbSet<EmployeesChangedTypeHistory> EmployeesChangedTypeHistories { get; set; }

        public virtual DbSet<ForgotPasswordOTPDetail> ForgotPasswordOTPDetails { get; set; }

        public virtual DbSet<Gender> Genders { get; set; }

        public virtual DbSet<HolidayMaster> HolidayMasters { get; set; }

        public virtual DbSet<InsurancePolicy> InsurancePolicies { get; set; }

        public virtual DbSet<InterviewFeedback> InterviewFeedbacks { get; set; }

        public virtual DbSet<InterviewPanel> InterviewPanels { get; set; }

        public virtual DbSet<InterviewPanelMember> InterviewPanelMembers { get; set; }

        public virtual DbSet<InterviewSchedule> InterviewSchedules { get; set; }

        public virtual DbSet<InterviewSdule> InterviewSdules { get; set; }

        public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

        public virtual DbSet<LeaveRule> LeaveRules { get; set; }

        public virtual DbSet<LeaveSandwichRule> SandwitchRules { get; set; }

        public virtual DbSet<LeaveSandwichRuleMapping> LeaveSandwichRuleMappings { get; set; }

        public virtual DbSet<LeaveTransactionLog> LeaveTransactionLogs { get; set; }

        public virtual DbSet<LeaveType> LeaveTypes { get; set; }

        //public virtual DbSet<License> Licenses { get; set; }

        public virtual DbSet<LoginCredential> LoginCredentials { get; set; }

        public virtual DbSet<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignations { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<ModuleOperationMapping> ModuleOperationMappings { get; set; }

        public virtual DbSet<NoImagePath> NoImagePaths { get; set; }

        public virtual DbSet<Operation> Operations { get; set; }

        public virtual DbSet<OrganizationHolidayCalendar> OrganizationHolidayCalendars { get; set; }

        public virtual DbSet<PageTypeEnum> PageTypeEnums { get; set; }

        public virtual DbSet<PlanModuleMapping> PlanModuleMappings { get; set; }

        public virtual DbSet<PolicyLeaveTypeMapping> PolicyLeaveTypeMappings { get; set; }
        public virtual DbSet<EmployeeIdentity> EmployeeIdentities { get; set; }

        public virtual DbSet<PolicyType> PolicyTypes { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<ReportingType> ReportingTypes { get; set; }

        public virtual DbSet<RequestType> RequestTypes { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<RoleModuleAndPermission> RoleModuleAndPermissions { get; set; }

        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }

        public virtual DbSet<State> States { get; set; }

        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public virtual DbSet<CountryIdentityRule> CountryIdentityRules { get; set; }
        public virtual DbSet<IdentityCategory> IdentityCategories { get; set; }
        public virtual DbSet<IdentityCategoryDocument> IdentityCategoryDocuments { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }

        public virtual DbSet<TenantEmailConfig> TenantEmailConfigs { get; set; }

        public virtual DbSet<TenantEnabledModule> TenantEnabledModules { get; set; }

        public virtual DbSet<TenantEnabledOperation> TenantEnabledOperations { get; set; }

        public virtual DbSet<TenantIndustry> TenantIndustries { get; set; }

        public virtual DbSet<TenantProfile> TenantProfiles { get; set; }

        public virtual DbSet<TenantSubscription> TenantSubscriptions { get; set; }

        public virtual DbSet<Tender> Tenders { get; set; }

        public virtual DbSet<TenderProject> TenderProjects { get; set; }


        public virtual DbSet<TenderService> TenderServices { get; set; }

        public virtual DbSet<TenderServiceHistory> TenderServiceHistories { get; set; }

        public virtual DbSet<TenderServiceProvider> TenderServiceProviders { get; set; }

        public virtual DbSet<TenderServiceSpecification> TenderServiceSpecifications { get; set; }

        public virtual DbSet<TenderServiceType> TenderServiceTypes { get; set; }

        public virtual DbSet<TenderStatus> TenderStatuses { get; set; }

        public virtual DbSet<TicketClassification> TicketClassifications { get; set; }
        public virtual DbSet<TenantEncryptionKey> TenantEncryptionKeys { get; set; }

        public virtual DbSet<TicketHeader> TicketHeaders { get; set; }

        public virtual DbSet<TicketType> TicketTypes { get; set; }

        public virtual DbSet<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignations { get; set; }

        public virtual DbSet<TravelMode> TravelModes { get; set; }

        public virtual DbSet<UserAttendanceSetting> UserAttendanceSettings { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<WorkflowStage> WorkflowStages { get; set; }

        public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

        public virtual DbSet<WorkstationType> WorkstationTypes { get; set; }
        public virtual DbSet<SubscribedModuleResponseDTO> SubscribedModuleResponseDTOs { get; set; }
        public virtual DbSet<FlatModuleOperationDto> TenantModulesConfigurations { get; set; }
        public virtual DbSet<GetEmployeeIdentitySp> GetEmployeeIdentitySps { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //    => optionsBuilder.UseSqlServer("Server=acer;Database=workforcedb-dev-pre;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;Command Timeout=300");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccoumndationAllowancePolicyByDesignation>(entity =>
            {
            
                entity.HasKey(e => e.Id).HasName("PK__Accoumnd__3214EC071BDF4022");

                entity.ToTable("AccoumndationAllowancePolicyByDesignation", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.FixedStayAllowance)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMetro).HasDefaultValue(false);
                entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
                entity.Property(e => e.MetroBonus)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MinDaysRequired).HasDefaultValue(0);
                entity.Property(e => e.RequiredDocuments).HasColumnType("text");
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Designation).WithMany(p => p.AccoumndationAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.DesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Accoumnda__Desig__11158940");

                entity.HasOne(d => d.EmployeeType).WithMany(p => p.AccoumndationAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.EmployeeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Accoumnda__Emplo__1209AD79");

                entity.HasOne(d => d.PolicyType).WithMany(p => p.AccoumndationAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.PolicyTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Accoumnda__Polic__12FDD1B2");
            });

            modelBuilder.Entity<ApprovalWorkflow>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Approval__3214EC071F76BFC6");

                entity.ToTable("ApprovalWorkflow", "AxionPro");

                entity.Property(e => e.ActionName).HasMaxLength(150);
                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.WorkflowName).HasMaxLength(150);
            });


            modelBuilder.Entity<AssetTicketTypeDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetTic__3214EC0706DC5F37");

                entity.ToTable("AssetTicketTypeDetail", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.AssetType).WithMany(p => p.AssetTicketTypeDetails)
                    .HasForeignKey(d => d.AssetTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetType_AssetTckTypeDetail_ID");

                entity.HasOne(d => d.ResponsibleRole).WithMany(p => p.AssetTicketTypeDetails)
                    .HasForeignKey(d => d.ResponsibleRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResponsibleRole_AssetTckTypeDetail_ID");

                entity.HasOne(d => d.TicketType).WithMany(p => p.AssetTicketTypeDetails)
                    .HasForeignKey(d => d.TicketTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TckType_AstTypeDetail_ID");
            });


            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Asset__3214EC076178ABAE");

                entity.ToTable("Asset", "AxionPro");

                entity.Property(e => e.AssetName).HasMaxLength(100);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Company).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsAssigned).HasDefaultValue(false);
                entity.Property(e => e.IsRepairable).HasDefaultValue(true);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
                entity.Property(e => e.Qrcode)
                    .HasMaxLength(100)
                    .HasColumnName("QRCode");
                // Common Entities
                entity.ConfigureBaseEntity();
                entity.Property(e => e.SerialNumber).HasMaxLength(100);
                entity.Property(e => e.WarrantyExpiryDate).HasColumnType("datetime");
                entity.HasOne(d => d.AssetStatus).WithMany(p => p.Assets)
                    .HasForeignKey(d => d.AssetStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Asset_AssetStatus");
                entity.HasOne(d => d.AssetType).WithMany(p => p.Assets)
                    .HasForeignKey(d => d.AssetTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Asset_AssetType");

            });

            modelBuilder.Entity<AssetImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetIma__3214EC0752335BEC");

                entity.ToTable("AssetImage", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.AssetImagePath).HasMaxLength(500);
                entity.Property(e => e.QRCodeImagePath).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Asset).WithMany(p => p.AssetImages)
                    .HasForeignKey(d => d.AssetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetImage_Asset");
            });
            modelBuilder.Entity<AssetAssignment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetAss__3214EC07CFA2D85C");

                entity.ToTable("AssetAssignment", "AxionPro");

                entity.Property(e => e.ActualReturnDate).HasColumnType("datetime");

                entity.Property(e => e.AssetConditionAtAssign).HasMaxLength(255);
                entity.Property(e => e.AssetConditionAtReturn).HasMaxLength(255);
                entity.Property(e => e.AssignedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExpectedReturnDate).HasColumnType("datetime");
                entity.Property(e => e.IdentificationMethod).HasMaxLength(50);
                entity.Property(e => e.IdentificationValue).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Common Entities
                entity.ConfigureBaseEntity();

                entity.HasOne(d => d.Asset).WithMany(p => p.AssetAssignments)
                    .HasForeignKey(d => d.AssetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetAssignment_Asset");

                entity.HasOne(d => d.AssignmentStatus).WithMany(p => p.AssetAssignments)
                    .HasForeignKey(d => d.AssignmentStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetAssignment_AssignmentStatus");

                entity.HasOne(d => d.Employee).WithMany(p => p.AssetAssignments)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetAssignment_Employee");
            });

            modelBuilder.Entity<AssetHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetHis__3214EC07599816A6");

                entity.ToTable("AssetHistory", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.AssetConditionAtAssign).HasMaxLength(100);
                entity.Property(e => e.AssetConditionAtReturn).HasMaxLength(100);
                entity.Property(e => e.AssignedDate).HasColumnType("datetime");
                entity.Property(e => e.IdentificationMethod).HasMaxLength(50);
                entity.Property(e => e.IdentificationValue).HasMaxLength(255);
                entity.Property(e => e.IsScrapped).HasDefaultValue(false);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.ReturnedDate).HasColumnType("datetime");
                entity.Property(e => e.ScrapDate).HasColumnType("datetime");
                entity.Property(e => e.ScrapReason).HasMaxLength(255);
               
                entity.HasOne(d => d.AssignmentStatus).WithMany(p => p.AssetHistories)
                    .HasForeignKey(d => d.AssignmentStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AssetHist__Assig__7AF13DF7");

                entity.HasOne(d => d.Employee).WithMany(p => p.AssetHistoryEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__AssetHist__Emplo__79FD19BE");

                entity.HasOne(d => d.ScrapApprovedByNavigation).WithMany(p => p.AssetHistoryScrapApprovedByNavigations)
                    .HasForeignKey(d => d.ScrapApprovedBy)
                    .HasConstraintName("FK__AssetHist__Scrap__7BE56230");
            });

            modelBuilder.Entity<AssetStatus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetSta__3214EC0722A59445");

                entity.ToTable("AssetStatus", "AxionPro");

                // Common Entities
                entity.ConfigureBaseEntity();
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.StatusName).HasMaxLength(50);

            });
            modelBuilder.Entity<AssetCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetCat__3214EC07E9FE2792");

                entity.ToTable("AssetCategory", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CategoryName).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.AssetCategories)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetCategory_Tenant");

            });
            modelBuilder.Entity<AssetType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__AssetTyp__3214EC077375A9AA");

                entity.ToTable("AssetType", "AxionPro");

                entity.HasIndex(e => e.TypeName, "UQ__AssetTyp__D4E7DFA8692FD5DF").IsUnique();

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.TypeName).HasMaxLength(100);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.HasOne(d => d.AssetCategory).WithMany(p => p.AssetTypes)
                    .HasForeignKey(d => d.AssetCategoryId)
                    .HasConstraintName("FK_AssetType_AssetCategory");
            });



          


            modelBuilder.Entity<AssignmentStatus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Assignme__3214EC07BCFD76FA");

                entity.ToTable("AssignmentStatus", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.StatusName).HasMaxLength(50);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC079EEE2ABB");

                entity.ToTable("Attendance", "AxionPro");

                entity.Property(e => e.AttendanceImagePath).HasMaxLength(200);
                entity.Property(e => e.AttendanceImageUrl)
                    .HasMaxLength(200)
                    .HasColumnName("AttendanceImageURL");
            });

            modelBuilder.Entity<AttendanceDeviceType>(entity =>
            {
                entity.ToTable("AttendanceDeviceType", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeviceType).HasMaxLength(50);
                entity.Property(e => e.IsDeviceRegister).HasDefaultValue(false);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AttendanceHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC07AC1B1F0C");

                entity.ToTable("AttendanceHistory", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.InTime).HasColumnType("datetime");
                entity.Property(e => e.OutTime).HasColumnType("datetime");
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.TotalBreakHours).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.TotalWorkHours).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.UpdatedDateTime)
                    .HasDefaultValueSql("(NULL)")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.AttendanceHistories)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceHistory_Employee");
            });

            modelBuilder.Entity<AttendanceRequest>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC07DA4D2CA6");

                entity.ToTable("AttendanceRequest", "AxionPro");

                entity.Property(e => e.AttendanceDate).HasColumnType("datetime");
                entity.Property(e => e.AttendanceImagePath).HasMaxLength(200);
                entity.Property(e => e.AttendanceImageUrl)
                    .HasMaxLength(200)
                    .HasColumnName("AttendanceImageURL");
                entity.Property(e => e.Remark).HasMaxLength(255);
            });

            modelBuilder.Entity<BasicMenu>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_BasicMenu_Id");

                entity.ToTable("BasicMenu", "AxionPro");

                entity.Property(e => e.ImageIcon).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.MenuName).HasMaxLength(100);
                entity.Property(e => e.MenuUrl)
                    .HasMaxLength(255)
                    .HasColumnName("MenuURL");
                entity.Property(e => e.Remark).HasMaxLength(200);

                entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                    .HasForeignKey(d => d.ParentMenuId)
                    .HasConstraintName("FK_BasicMenu_ParentMenu");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC078AAEC326");

                entity.ToTable("Candidate", "AxionPro");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Candidat__85FB4E384EF74606").IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Candidat__A9D10534B67C36A0").IsUnique();

                entity.HasIndex(e => e.Aadhaar, "UQ__Candidat__C4B3336970636589").IsUnique();

                entity.HasIndex(e => e.Pan, "UQ__Candidat__C577943D11665D42").IsUnique();

                entity.HasIndex(e => e.CandidateReferenceCode, "UQ__Candidat__CF22B81A936408F4").IsUnique();

                entity.Property(e => e.Aadhaar).HasMaxLength(12);
                entity.Property(e => e.ActionStatus).HasMaxLength(20);
                entity.Property(e => e.AppliedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CandidateReferenceCode).HasMaxLength(20);
                entity.Property(e => e.CurrentCompany).HasMaxLength(200);
                entity.Property(e => e.CurrentLocation).HasMaxLength(200);
                entity.Property(e => e.Education).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.ExpectedSalary).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ExperienceYears).HasColumnType("decimal(4, 1)");
                entity.Property(e => e.FewWords).HasMaxLength(1000);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.LastUpdatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Pan)
                    .HasMaxLength(10)
                    .HasColumnName("PAN");
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.ResumePath).HasMaxLength(200);
                entity.Property(e => e.ResumeUrl)
                    .HasMaxLength(200)
                    .HasColumnName("ResumeURL");
            });

            modelBuilder.Entity<CandidateCategorySkill>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC07853F4240");

                entity.ToTable("CandidateCategorySkill", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateCategorySkills)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Candidate__Candi__15DA3E5D");

                entity.HasOne(d => d.Category).WithMany(p => p.CandidateCategorySkills)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Candidate__Categ__16CE6296");
            });

            modelBuilder.Entity<CandidateHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC07F93DEB9C");

                entity.ToTable("CandidateHistory", "AxionPro");

                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ReapplyAllowedAfter).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateHistories)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Candidate__Candi__17C286CF");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Category__3214EC070D4D0C80");

                entity.ToTable("Category", "AxionPro");

                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.ParentId).HasColumnName("ParentID");
                entity.Property(e => e.Remark).HasMaxLength(50);
                entity.Property(e => e.Tags).HasMaxLength(255);

                entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Category_Parent");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__District__3214EC077686EC47");

                entity.ToTable("District", "AxionPro");

                entity.HasIndex(e => e.DistrictName, "IX_District_DistrictName");

                entity.HasIndex(e => e.StateId, "IX_District_StateId");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DistrictCode).HasMaxLength(50);
                entity.Property(e => e.DistrictName).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PinCode).HasMaxLength(50);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.State).WithMany(p => p.Districts)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_District_State");
            });


            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TenderClient");

                entity.ToTable("Client", "AxionPro");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ClientName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClientType).WithMany(p => p.Clients)
                    .HasForeignKey(d => d.ClientTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_ClientType");
            });

            modelBuilder.Entity<ClientType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ClientTy__3214EC078D984A16");

                entity.ToTable("ClientType", "AxionPro");

                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Country__3214EC070584DCC0");

                entity.ToTable("Country", "AxionPro");

                entity.Property(e => e.CountryCode).HasMaxLength(10);
                entity.Property(e => e.CountryName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
            modelBuilder.Entity<DataViewStructure>(entity =>
            {
                entity.ToTable("DataViewStructure", "AxionPro");

                entity.Property(e => e.Discription).HasMaxLength(150);
                entity.Property(e => e.DisplayOn).HasMaxLength(50);
                entity.Property(e => e.Remark).HasMaxLength(150);
            });

            modelBuilder.Entity<DayCombination>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__DayCombi__3214EC070D2CF976");

                entity.ToTable("DayCombination", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CombinationName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.AddedById).HasColumnType("long");
                entity.Property(e => e.SoftDeletedById).HasColumnType("long");
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.DayCombinations)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_DayCombination_Tenant");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC071E7B0B0B");

                entity.ToTable("Department", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DepartmentName).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.Departments)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_Department_Tenant");
            });

            modelBuilder.Entity<Designation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Designat__3214EC07F9FF4C75");

                entity.ToTable("Designation", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.DesignationName).HasMaxLength(255);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Department).WithMany(p => p.Designations)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Designation_Department");

                entity.HasOne(d => d.Tenant).WithMany(p => p.Designations)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Designation_Tenant");
            });

            modelBuilder.Entity<EmailQueue>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__EmailQue__3214EC0705218438");

                entity.ToTable("EmailQueue", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsSent).HasDefaultValue(false);
                entity.Property(e => e.RetryCount).HasDefaultValue(0);
                entity.Property(e => e.SendDateTime).HasColumnType("datetime");
                entity.Property(e => e.Subject).HasMaxLength(500);
                entity.Property(e => e.ToEmail).HasMaxLength(250);

                entity.HasOne(d => d.Template).WithMany(p => p.EmailQueues)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmailQueu__Templ__44B528D7");
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__EmailTem__3214EC072FE49A64");

                entity.ToTable("EmailTemplate", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.AddedFromIp)
                    .HasMaxLength(50)
                    .HasColumnName("AddedFromIP");
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.FromEmail).HasMaxLength(150);
                entity.Property(e => e.FromName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.LanguageCode).HasMaxLength(10);
                entity.Property(e => e.Subject).HasMaxLength(250);
                entity.Property(e => e.TemplateCode).HasMaxLength(100);
                entity.Property(e => e.TemplateName).HasMaxLength(150);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedFromIp)
                    .HasMaxLength(50)
                    .HasColumnName("UpdatedFromIP");
            });

            modelBuilder.Entity<EmailsLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__EmailsLo__3214EC0779530258");

                entity.ToTable("EmailsLog", "AxionPro");

                entity.Property(e => e.AddedFromIp).HasMaxLength(50);
                entity.Property(e => e.BccEmail).HasMaxLength(1000);
                entity.Property(e => e.CcEmail).HasMaxLength(1000);
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.SentDateTime).HasColumnType("datetime");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Queued");
                entity.Property(e => e.Subject).HasMaxLength(500);
                entity.Property(e => e.ToEmail).HasMaxLength(500);
                entity.Property(e => e.TriggeredBy).HasMaxLength(100);
            });
            modelBuilder.Entity<CountryIdentityRule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CountryI__3214EC0748E4A649");

                entity.ToTable("CountryIdentityRule", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMandatory).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Country).WithMany(p => p.CountryIdentityRules)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Country");

                entity.HasOne(d => d.IdentityCategoryDocument).WithMany(p => p.CountryIdentityRules)
                    .HasForeignKey(d => d.IdentityCategoryDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Country_Document");
            });


            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Employee");

                entity.ToTable("Employee", "AxionPro");

                entity.HasIndex(e => e.OfficialEmail, "UX_Employee_OfficialEmail_Once")
                      .IsUnique()
                      .HasFilter("([TenantId] IS NULL AND [IsSoftDeleted]=(0))");

                entity.Property(e => e.AddedDateTime)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);

                entity.HasOne(d => d.Designation)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(d => d.DesignationId)
                      .OnDelete(DeleteBehavior.SetNull);

                //entity.Property(e => e.CountryId)
                //.IsRequired();

        //        entity.HasOne(e => e.Country)
        //.WithMany()
        //.HasForeignKey(e => e.CountryId)
        //.OnDelete(DeleteBehavior.Cascade)
        //.HasConstraintName("FK_Employee_Country");



                entity.HasOne(d => d.EmployeeType)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(d => d.EmployeeTypeId);

                entity.HasOne(d => d.Gender)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(d => d.GenderId);

                entity.HasOne(d => d.Tenant)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(d => d.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmployeeContact>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0701BE71BF");

                entity.ToTable("EmployeeContact", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.AlternateNumber).HasMaxLength(20);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.HouseNo).HasMaxLength(250);
                entity.Property(e => e.InfoVerifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsEditAllowed).HasDefaultValue(false);
                entity.Property(e => e.IsInfoVerified).HasDefaultValue(false);
                entity.Property(e => e.IsPrimary).HasDefaultValue(false);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(true);
                entity.Property(e => e.LandMark).HasMaxLength(250);
                entity.Property(e => e.Address).HasMaxLength(250);               
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.Street).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeContacts)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeContact_Employee");
            });


            modelBuilder.Entity<EmployeeBankDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC072E9F930F");

                entity.ToTable("EmployeeBankDetail", "AxionPro");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);
                entity.Property(e => e.AccountType).HasMaxLength(50);
                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.BankName).HasMaxLength(100);
                entity.Property(e => e.BranchName).HasMaxLength(100);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IFSCCode)
                    .HasMaxLength(20)
                    .HasColumnName("IFSCCode");
                entity.Property(e => e.InfoVerifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsPrimaryAccount).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UPIId)
                    .HasMaxLength(100)
                    .HasColumnName("UPIId");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeBankDetails)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeBank_Employee");
            });

            modelBuilder.Entity<EmployeeCategorySkill>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07BB3C49C9");

                entity.ToTable("EmployeeCategorySkill", "AxionPro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Category).WithMany(p => p.EmployeeCategorySkills)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_EmployeeCategorySkill_Category");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeCategorySkills)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeCategorySkill_Employee");
            });

            modelBuilder.Entity<EmployeeDailyAttendance>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC078C36F2DB");

                entity.ToTable("EmployeeDailyAttendance", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.AttendanceDate).HasColumnType("datetime");
                entity.Property(e => e.IsLate).HasDefaultValue(false);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AttendanceDeviceType).WithMany(p => p.EmployeeDailyAttendances)
                    .HasForeignKey(d => d.AttendanceDeviceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceDeviceType");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDailyAttendances)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee");

                entity.HasOne(d => d.WorkstationType).WithMany(p => p.EmployeeDailyAttendances)
                    .HasForeignKey(d => d.WorkstationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkstationType");
            });

            modelBuilder.Entity<EmployeeDependent>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074B6A13E2");

                entity.ToTable("EmployeeDependent", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DependentName).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.InfoVerifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsCoveredInPolicy).HasDefaultValue(false);
                entity.Property(e => e.Relation).HasMaxLength(50);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.UpdatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDependents)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeDependents_Employee");
            });

            modelBuilder.Entity<EmployeeEducation>(entity =>
            {
                entity.ToTable("EmployeeEducation", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Degree).HasMaxLength(50);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.EducationGap).HasDefaultValue(false);
                entity.Property(e => e.FileName).HasMaxLength(100);
                entity.Property(e => e.FilePath).HasMaxLength(500);
            
                entity.Property(e => e.ScoreValue).HasMaxLength(50);
                entity.Property(e => e.GradeDivision).HasMaxLength(10);
                entity.Property(e => e.InfoVerifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.InstituteName).HasMaxLength(100);
                entity.Property(e => e.ReasonOfEducationGap).HasMaxLength(255);
                entity.Property(e => e.Remark).HasMaxLength(100);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });
            modelBuilder.Entity<EmployeeExperience>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC071B15DEB3");

                entity.ToTable("EmployeeExperience", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.Ctc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CTC");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.HasEPFAccount).HasColumnName("HasEPFAccount");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<EmployeeExperienceDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07A52190B4");

                entity.ToTable("EmployeeExperienceDetail", "AxionPro");

                entity.Property(e => e.ColleagueContactNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.ColleagueDesignation).HasMaxLength(100);
                entity.Property(e => e.ColleagueName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.CompanyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Designation).HasMaxLength(50);
                entity.Property(e => e.WorkingCountryId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.WorkingDistrictId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.WorkingStateId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.EmployeeIdOfCompany)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ExperienceLetterDocName)
                    .HasMaxLength(100)                    
                    .IsUnicode(false);
                entity.Property(e => e.BankStatementDocName)
                    .HasMaxLength(100)                    
                    .IsUnicode(false);
                entity.Property(e => e.BankStatementDocPath).HasMaxLength(500);

                entity.Property(e => e.TaxationDocFilePath).HasMaxLength(500);
                entity.Property(e => e.ExperienceLetterDocPath).HasMaxLength(500);
                entity.Property(e => e.TaxationDocFilePath).HasMaxLength(500);
                entity.Property(e => e.TaxationDocFileName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.GapCertificateDocName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.GapCertificateDocPath).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsWFH).HasColumnName("IsWFH");
                entity.Property(e => e.JoiningLetterDocName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.JoiningLetterDocPath).HasMaxLength(500);
                entity.Property(e => e.ReasonForLeaving).HasMaxLength(200);
                entity.Property(e => e.ReasonOfGap).HasMaxLength(500);
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.ReportingManagerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ReportingManagerNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.VerificationEmail).HasMaxLength(50);
                entity.Property(e => e.TaxationDocFileName)
               .HasMaxLength(100)
               .IsUnicode(false);
                entity.Property(e => e.TaxationDocFilePath).HasMaxLength(500);
                entity.Property(e => e.VerificationEmail).HasMaxLength(50);
                entity.Property(e => e.VisaDocName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.VisaDocPath)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.VisaType)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.WorkPermitDocName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.WorkPermitDocPath)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.WorkPermitNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.EmployeeExperience).WithMany(p => p.EmployeeExperienceDetails)
               .HasForeignKey(d => d.EmployeeExperienceId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_ExperienceDetail_Experience");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeExperienceDetails)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeExperienceUploadDoc_Employee");
            });

            modelBuilder.Entity<EmployeeExperiencePayslipUpload>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07BDB4A1FB");

                entity.ToTable("EmployeeExperiencePayslipUpload", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PayslipDocName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.PayslipDocPath).HasMaxLength(500);

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeExperiencePayslipUploads)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeExperiencePayslipUpload_Employee");

                entity.HasOne(d => d.ExperienceDetail).WithMany(p => p.EmployeeExperiencePayslipUploads)
                    .HasForeignKey(d => d.ExperienceDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeExperiencePayslipUpload_ExperienceDetail");
            });
            modelBuilder.Entity<IdentityCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Identity__3214EC07393AF557");

                entity.ToTable("IdentityCategory", "AxionPro");

                entity.HasIndex(e => e.Code, "UQ__Identity__A25C5AA7EC142E9A").IsUnique();

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });


            modelBuilder.Entity<IdentityCategoryDocument>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Identity__3214EC07AA61914F");

                entity.ToTable("IdentityCategoryDocument", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);
                entity.Property(e => e.DocumentName)
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsUnique).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdentityCategory).WithMany(p => p.IdentityCategoryDocuments)
                    .HasForeignKey(d => d.IdentityCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IdentityDocument_Category");
            });

            modelBuilder.Entity<EmployeeIdentity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074FF9BCC1");

                entity.ToTable("EmployeeIdentity", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.DocumentFileName).HasMaxLength(255);
                entity.Property(e => e.DocumentFilePath).HasMaxLength(500);
                entity.Property(e => e.EffectiveFrom).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IdentityValue).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsEditAllowed).HasDefaultValue(true);

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeIdentities)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeIdentity_Employee");

                entity.HasOne(d => d.IdentityCategoryDocument).WithMany(p => p.EmployeeIdentities)
                    .HasForeignKey(d => d.IdentityCategoryDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeIdentity_Document");
            });

            modelBuilder.Entity<EmployeeImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07C583D80F");

                entity.ToTable("EmployeeImage", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.FileName).HasMaxLength(50);
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeImages)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EmployeeImages_Employee");
            });

            modelBuilder.Entity<EmployeeInsurancePolicy>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0705F48FB6");

                entity.ToTable("EmployeeInsurancePolicy", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07B6CC8062");

                entity.ToTable("EmployeeLeaveBalance", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Availed).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.CarryForwarded).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.CurrentBalance)
                    .HasComputedColumnSql("([OpeningBalance]-[Availed])", true)
                    .HasColumnType("decimal(6, 2)");
                entity.Property(e => e.Encashed).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.LeavesOnHold)
                    .HasDefaultValue(0m)
                    .HasColumnType("decimal(5, 2)");
                entity.Property(e => e.OpeningBalance).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.EmployeeLeavePolicyMapping).WithMany(p => p.EmployeeLeaveBalances)
                    .HasForeignKey(d => d.EmployeeLeavePolicyMappingId)
                    .HasConstraintName("FK_EmployeeLeaveBalance_EmployeeLeavePolicyMapping");

                entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeLeaveBalances)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EmployeeLeaveBalance_Tenant");
            });

            modelBuilder.Entity<EmployeeLeavePolicyMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07305828A7");

                entity.ToTable("EmployeeLeavePolicyMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EffectiveFrom).HasColumnType("datetime");
                entity.Property(e => e.EffectiveTo).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeavePolicyMappings)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeL__Emplo__73D00A73");

                entity.HasOne(d => d.PolicyLeaveTypeMapping).WithMany(p => p.EmployeeLeavePolicyMappings)
                    .HasForeignKey(d => d.PolicyLeaveTypeMappingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeL__Leave__74C42EAC");

                entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeLeavePolicyMappings)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeL__Tenan__72DBE63A");
            });

            modelBuilder.Entity<EmployeeManagerMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC075BB6D3FC");

                entity.ToTable("EmployeeManagerMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.EffectiveFrom).HasColumnType("datetime");
                entity.Property(e => e.EffectiveTo).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Department).WithMany(p => p.EmployeeManagerMappings)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_EmployeeManagerMapping_Department");

                entity.HasOne(d => d.Designation).WithMany(p => p.EmployeeManagerMappings)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_EmployeeManagerMapping_Designation");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeManagerMappingEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeManagerMapping_Employee");

                entity.HasOne(d => d.Manager).WithMany(p => p.EmployeeManagerMappingManagers)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeManagerMapping_Manager");

                entity.HasOne(d => d.ReportingType).WithMany(p => p.EmployeeManagerMappings)
                    .HasForeignKey(d => d.ReportingTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeManagerMapping_ReportingType");

                entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeManagerMappings)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeManagerMapping_Tenant");
            });

            modelBuilder.Entity<EmployeePersonalDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074796302D");

                entity.ToTable("EmployeePersonalDetail", "AxionPro");
                entity.Property(e => e.AadhaarNumber).HasMaxLength(20);
                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.BloodGroup).HasMaxLength(10);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.AadhaarDocPath).HasMaxLength(200);
                entity.Property(e => e.PanDocPath).HasMaxLength(200);
                entity.Property(e => e.PassportDocPath).HasMaxLength(200);
                entity.Property(e => e.DrivingLicenseNumber).HasMaxLength(20);
                entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactRelation).HasMaxLength(49);
                entity.Property(e => e.UANNumber).HasMaxLength(15);
                entity.Property(e => e.EmergencyContactNumber).HasMaxLength(15);
                entity.Property(e => e.InfoVerifiedDateTime).HasColumnType("datetime");
                entity.Property(e => e.MaritalStatus).HasMaxLength(20);
                entity.Property(e => e.Nationality).HasMaxLength(50);
                entity.Property(e => e.PanNumber).HasMaxLength(20);
                entity.Property(e => e.PassportNumber).HasMaxLength(20);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.VoterId).HasMaxLength(20);

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePersonalDetails)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePersonalDetail_Employee");
            });

            modelBuilder.Entity<EmployeeType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0760C5ED38");

                entity.ToTable("EmployeeType", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.TypeName).HasMaxLength(255);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<EmployeeTypeBasicMenu>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07A773CB3F");

                entity.ToTable("EmployeeTypeBasicMenu", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMenuDisplayInUi).HasColumnName("IsMenuDisplayInUI");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.BasicMenu).WithMany(p => p.EmployeeTypeBasicMenus)
                    .HasForeignKey(d => d.BasicMenuId)
                    .HasConstraintName("FK_EmployeeTypeBasicMenu_BasicMenu");

                entity.HasOne(d => d.EmployeeType).WithMany(p => p.EmployeeTypeBasicMenus)
                    .HasForeignKey(d => d.EmployeeTypeId)
                    .HasConstraintName("FK_EmployeeTypeBasicMenu_EmployeeType");
            });

            modelBuilder.Entity<EmployeesChangedTypeHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0779153EAD");

                entity.ToTable("EmployeesChangedTypeHistory", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ChangeDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeesChangedTypeHistories)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeStatusHistory_Employee");

                entity.HasOne(d => d.NewEmployeeType).WithMany(p => p.EmployeesChangedTypeHistoryNewEmployeeTypes)
                    .HasForeignKey(d => d.NewEmployeeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeStatusHistory_NewEmployeeType");

                entity.HasOne(d => d.OldEmployeeType).WithMany(p => p.EmployeesChangedTypeHistoryOldEmployeeTypes)
                    .HasForeignKey(d => d.OldEmployeeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeStatusHistory_OldEmployeeType");
            });


            modelBuilder.Entity<ForgotPasswordOTPDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ForgotPa__3214EC071E853072");

                entity.ToTable("ForgotPasswordOTPDetail", "AxionPro");

                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Otp)
                    .HasMaxLength(10)
                    .HasColumnName("OTP");
                entity.Property(e => e.OtpexpireDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("OTPExpireDateTime");
                entity.Property(e => e.UsedDateTime).HasColumnType("datetime");

                //entity.HasOne(d => d.Tenant).WithMany(p => p.ForgotPasswordRequests)
                //    .HasForeignKey(d => d.TenantId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ForgotPasswordRequest_Tenant");

                entity.HasOne(d => d.LoginCredential).WithMany(p => p.ForgotPasswordRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForgotPasswordOTPDetail_LoginCredential");
            });


            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Gender__3214EC07EF3CD03D");

                entity.ToTable("Gender", "AxionPro");

                entity.HasIndex(e => e.GenderName, "UQ__Gender__F7C177153AF55502").IsUnique();

                entity.Property(e => e.GenderName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<HolidayMaster>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__HolidayM__3214EC0743457A80");

                entity.ToTable("HolidayMaster", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.HolidayName).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Region).HasMaxLength(100);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<InsurancePolicy>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PolicyMa__3214EC07A7865FF0");

                entity.ToTable("InsurancePolicy", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.AgentContactNumber).HasMaxLength(20);
                entity.Property(e => e.AgentName).HasMaxLength(150);
                entity.Property(e => e.AgentOfficeNumber).HasMaxLength(20);
                entity.Property(e => e.CoverageType).HasMaxLength(100);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.InsurancePolicyName).HasMaxLength(200);
                entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ProviderName).HasMaxLength(100);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<InterviewFeedback>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07C20637CB");

                entity.ToTable("InterviewFeedback", "AxionPro");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");
                entity.Property(e => e.ReapplyAfter).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Candidate).WithMany(p => p.InterviewFeedbacks)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Interview__Candi__2AD55B43");

                entity.HasOne(d => d.InterviewSchedule).WithMany(p => p.InterviewFeedbacks)
                    .HasForeignKey(d => d.InterviewScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Interview__Inter__2BC97F7C");
            });

            modelBuilder.Entity<InterviewPanel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07076FDC5F");

                entity.ToTable("InterviewPanel", "AxionPro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PanelName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InterviewPanelMember>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07205CE980");

                entity.ToTable("InterviewPanelMember", "AxionPro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Panel).WithMany(p => p.InterviewPanelMembers)
                    .HasForeignKey(d => d.PanelId)
                    .HasConstraintName("FK_InterviewPanelMember_Panel");

                entity.HasOne(d => d.UserRole).WithMany(p => p.InterviewPanelMembers)
                    .HasForeignKey(d => d.UserRoleId)
                    .HasConstraintName("FK_InterviewPanelMember_UserRole");
            });

            modelBuilder.Entity<InterviewSchedule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC073C91635C");

                entity.ToTable("InterviewSchedule", "AxionPro");

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ScheduledDate).HasColumnType("datetime");

                entity.HasOne(d => d.Candidate).WithMany(p => p.InterviewSchedules)
                    .HasForeignKey(d => d.CandidateId)
                    .HasConstraintName("FK_InterviewSchedule_Candidate");

                entity.HasOne(d => d.Panel).WithMany(p => p.InterviewSchedules)
                    .HasForeignKey(d => d.PanelId)
                    .HasConstraintName("FK_InterviewSchedule_Panel");
            });

            modelBuilder.Entity<InterviewSdule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC0702C95E26");

                entity.ToTable("InterviewSdule", "AxionPro");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.InterviewMode).HasMaxLength(50);
                entity.Property(e => e.ScheduledDateTime).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveReq__3214EC07D95D9BAD");

                entity.ToTable("LeaveRequest", "AxionPro");

                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
                entity.Property(e => e.CancellationDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsHalfDay).HasDefaultValue(false);
                entity.Property(e => e.IsSandwich).HasDefaultValue(false);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Remark).HasMaxLength(100);
                entity.Property(e => e.Status).HasDefaultValueSql("('Pending')");
                entity.Property(e => e.TotalLeaveDays).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequests)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeaveRequ__Emplo__05EEBAAE");

                entity.HasOne(d => d.LeavePolicy).WithMany(p => p.LeaveRequests)
                    .HasForeignKey(d => d.LeavePolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeaveRequ__Leave__07D70320");

                entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveRequests)
                    .HasForeignKey(d => d.LeaveTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeaveRequ__Leave__06E2DEE7");

                entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveRequests)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeaveRequ__Tenan__04FA9675");
            });

            modelBuilder.Entity<LeaveRule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveRul__3214EC07C443E6FB");

                entity.ToTable("LeaveRule", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsHalfDayAllowed).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.PolicyLeaveType).WithMany(p => p.LeaveRules)
                    .HasForeignKey(d => d.PolicyLeaveTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveRule_PolicyLeaveType");

                entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveRules)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveRule_Tenant");
            });

            modelBuilder.Entity<LeaveSandwichRule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveSan__3214EC0757F4038B");

                entity.ToTable("LeaveSandwichRule", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.RuleName).HasMaxLength(100);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<LeaveSandwichRuleMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveSan__3214EC07EB5FE9CE");

                entity.ToTable("LeaveSandwichRuleMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.DayCombination).WithMany(p => p.LeaveSandwichRuleMappings)
                    .HasForeignKey(d => d.DayCombinationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DayCombination");

                entity.HasOne(d => d.LeaveRule).WithMany(p => p.LeaveSandwichRuleMappings)
                    .HasForeignKey(d => d.LeaveRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveRule");

                entity.HasOne(d => d.LeaveSandwichRule).WithMany(p => p.LeaveSandwichRuleMappings)
                    .HasForeignKey(d => d.LeaveSandwichRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SandwichRule");
            });

            modelBuilder.Entity<LeaveTransactionLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveTra__3214EC0737FE86DD");

                entity.ToTable("LeaveTransactionLog", "AxionPro");

                entity.Property(e => e.LeaveDays).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.TransactionDate).HasColumnType("datetime");
                entity.Property(e => e.TransactionType)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeaveTyp__3214EC0788A5EF9C");

                entity.ToTable("LeaveType", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.LeaveName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveTypes)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_LeaveType_TenantId");
            });

            //modelBuilder.Entity<License>(entity =>
            //{
            //    entity.HasKey(e => e.Id).HasName("PK__License__3214EC07FF575687");

            //    entity.ToTable("License", "AxionPro");

            //    entity.Property(e => e.IsActive).HasDefaultValue(true);
            //    entity.Property(e => e.LicenseEndDate).HasColumnType("datetime");
            //    entity.Property(e => e.LicenseStartDate).HasColumnType("datetime");
            //});

            modelBuilder.Entity<LoginCredential>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LoginCre__3214EC0750429DFA");

                entity.ToTable("LoginCredential", "AxionPro");

                entity.HasIndex(e => e.LoginId, "UQ_LoginId").IsUnique();

                entity.Property(e => e.AddedById).HasDefaultValue(0L);
                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.SoftDeletedById).HasDefaultValue(0L);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.HasFirstLogin).HasDefaultValue(true);
                entity.Property(e => e.IpAddressLocal).HasMaxLength(50);
                entity.Property(e => e.IpAddressPublic).HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.Latitude).HasDefaultValue(0.0);
                entity.Property(e => e.LoginDevice).HasDefaultValue(0);
                entity.Property(e => e.LoginId).HasMaxLength(255);
                entity.Property(e => e.Longitude).HasDefaultValue(0.0);
                entity.Property(e => e.MacAddress).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(555);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.UpdatedById).HasDefaultValue(0L);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.HasOne(d => d.Employee).WithMany(p => p.LoginCredentials)
                   .HasForeignKey(d => d.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_LoginCredential_Employee");
            });

            modelBuilder.Entity<MealAllowancePolicyByDesignation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__MealAllo__3214EC07BB4E4E52");

                entity.ToTable("MealAllowancePolicyByDesignation", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.BreakfastAllowance)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.DinnerAllowance)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.FixedFoodAllowance)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMetro).HasDefaultValue(false);
                entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
                entity.Property(e => e.LunchAllowance)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MetroBonus)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MinDaysRequired).HasDefaultValue(0);
                entity.Property(e => e.RequiredDocuments).HasColumnType("text");
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Designation).WithMany(p => p.MealAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.DesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MealAllow__Desig__39237A9A");

                entity.HasOne(d => d.EmployeeType).WithMany(p => p.MealAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.EmployeeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MealAllow__Emplo__3A179ED3");

                entity.HasOne(d => d.PolicyType).WithMany(p => p.MealAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.PolicyTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MealAllow__Polic__3B0BC30C");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Module__3214EC078891AB2D");

                entity.ToTable("Module", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.ImageIconMobile)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ImageIconWeb)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsModuleDisplayInUi)
                    .HasDefaultValue(true)
                    .HasColumnName("IsModuleDisplayInUI");
                entity.Property(e => e.ItemPriority).HasDefaultValue(0);
                entity.Property(e => e.ModuleCode).HasMaxLength(50);
                entity.Property(e => e.ModuleName).HasMaxLength(100);                
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.URLPath).HasMaxLength(500).HasColumnName("URLPath");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ParentModule).WithMany(p => p.InverseParentModule)
                    .HasForeignKey(d => d.ParentModuleId)
                    .HasConstraintName("FK_Module_ParentModule");
            });

            modelBuilder.Entity<ModuleOperationMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModuleOp__3214EC07BF86A196");

                entity.ToTable("ModuleOperationMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IconUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IconURL");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsCommonItem).HasDefaultValue(false);
                entity.Property(e => e.IsOperational).HasDefaultValue(true);
                entity.Property(e => e.PageUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PageURL");
                entity.Property(e => e.Priority).HasDefaultValue(0);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.DataViewStructure).WithMany(p => p.ModuleOperationMappings)
                    .HasForeignKey(d => d.DataViewStructureId)
                    .HasConstraintName("FK_ModuleOperationMapping_DataViewStructure");

                entity.HasOne(d => d.Module).WithMany(p => p.ModuleOperationMappings)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("FK_ModuleOperationMapping_Module");

                entity.HasOne(d => d.Operation).WithMany(p => p.ModuleOperationMappings)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleOperation_Operation");

                entity.HasOne(d => d.PageType).WithMany(p => p.ModuleOperationMappings)
                    .HasForeignKey(d => d.PageTypeId)
                    .HasConstraintName("FK_ModuleOperationMapping_PageTypeEnum");
            });

            modelBuilder.Entity<NoImagePath>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__NoImageP__3214EC07B56BC19F");

                entity.ToTable("NoImagePath", "AxionPro");

                entity.Property(e => e.DefaultImagePath).HasMaxLength(500);
                entity.Property(e => e.ImageName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ImageType).HasDefaultValue(1);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Operatio__3214EC079C437610");

                entity.ToTable("Operation", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IconImage).HasMaxLength(250);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.OperationName).HasMaxLength(200);
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrganizationHolidayCalendar>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC077FBA239C");

                entity.ToTable("OrganizationHolidayCalendar", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CountryCode).HasMaxLength(5);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.HolidayDate).HasColumnType("datetime");
                entity.Property(e => e.HolidayName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.StateCode).HasMaxLength(10);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.OrganizationHolidayCalendars)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_OrganizationHolidayCalendar_Tenant");
            });

            modelBuilder.Entity<PageTypeEnum>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PageType__3214EC07D71C5D81");

                entity.ToTable("PageTypeEnum", "AxionPro");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.PageTypeName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PlanModuleMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PlanModu__3214EC0729948732");

                entity.ToTable("PlanModuleMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Module).WithMany(p => p.PlanModuleMappings)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanModuleMapping_Module");

                entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.PlanModuleMappings)
                    .HasForeignKey(d => d.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PMM_SubscriptionPlan");
            });

            modelBuilder.Entity<PolicyLeaveTypeMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LeavePol__3214EC07A76146FC");

                entity.ToTable("PolicyLeaveTypeMapping", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EffectiveFrom).HasColumnType("datetime");
                entity.Property(e => e.EffectiveTo).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ProofDocumentType).HasMaxLength(100);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ApplicableGender).WithMany(p => p.PolicyLeaveTypeMappings)
                    .HasForeignKey(d => d.ApplicableGenderId)
                    .HasConstraintName("FK_PolicyLeaveTypeMapping_Gender");
            });

            modelBuilder.Entity<PolicyType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PolicyTy__3214EC07AAE08A64");

                entity.ToTable("PolicyType", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
                entity.Property(e => e.PolicyName).HasMaxLength(255);
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.PolicyTypes)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_PolicyType_Tenant");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0731D02168");

                entity.ToTable("RefreshToken", "AxionPro");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedByIp).HasMaxLength(50);
                entity.Property(e => e.ExpiryDate)
                    .HasDefaultValueSql("(dateadd(day,(5),getdate()))")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsRevoked).HasDefaultValue(false);
                entity.Property(e => e.LoginId).HasMaxLength(255);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
                entity.Property(e => e.RevokedAt).HasColumnType("datetime");
                entity.Property(e => e.RevokedByIp).HasMaxLength(50);
                entity.Property(e => e.Token).HasMaxLength(500);

                entity.HasOne(d => d.Login).WithMany(p => p.RefreshTokens)
                    .HasPrincipalKey(p => p.LoginId)
                    .HasForeignKey(d => d.LoginId)
                    .HasConstraintName("FK__RefreshTo__Login__7132C993");
            });

            modelBuilder.Entity<ReportingType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Reportin__3214EC0773736C22");

                entity.ToTable("ReportingType", "AxionPro");

                entity.HasIndex(e => e.TypeName, "UQ__Reportin__D4E7DFA8AC2E2E45").IsUnique();

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(250);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__RequestT__3214EC07E53AD996");

                entity.ToTable("RequestType", "AxionPro");

                entity.HasIndex(e => new { e.TenantId, e.RequestTypeName }, "UQ_RequestType_Tenant").IsUnique();

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.RequestTypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.RequestTypes)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_RequestType_Tenant");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0723371271");

                entity.ToTable("Role", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime)
                    .HasDefaultValueSql("(NULL)")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsSystemDefault).HasDefaultValue(false);
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasDefaultValueSql("(NULL)");
                entity.Property(e => e.RoleName).HasMaxLength(100);
                entity.Property(e => e.RoleType).HasDefaultValue(null);
                entity.Property(e => e.SoftDeletedById).HasDefaultValueSql("(NULL)");
                entity.Property(e => e.UpdatedById).HasDefaultValueSql("(NULL)");
                entity.Property(e => e.UpdatedDateTime)
                    .HasDefaultValueSql("(NULL)")
                    .HasColumnType("datetime");

                //entity.HasOne(d => d.Designation).WithMany(p => p.Roles)
                //    .HasForeignKey(d => d.DesignationId)
                //    .HasConstraintName("FK_Role_Designation");

                //entity.HasOne(d => d.Tenant).WithMany(p => p.Roles)
                //    .HasForeignKey(d => d.TenantId)
                //    .HasConstraintName("FK_Role_Tenant");
            });

            modelBuilder.Entity<RoleModuleAndPermission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_RoleModuleAndPermission_Id");

                entity.ToTable("RoleModuleAndPermission", "AxionPro");

                entity.HasIndex(e => e.OperationId, "IDX_RolesPermission_OperationId");

                entity.HasIndex(e => e.RoleId, "IDX_RolesPermission_RoleId");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ImageIcon).HasMaxLength(200);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.SoftDeletedById).HasDefaultValue(0L);
                entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Module).WithMany(p => p.RoleModuleAndPermissions)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("FK_RoleModulePermission_Module");

                entity.HasOne(d => d.Operation).WithMany(p => p.RoleModuleAndPermissions)
                    .HasForeignKey(d => d.OperationId)
                    .HasConstraintName("FK_RoleModulePermission_Operation");

                entity.HasOne(d => d.Role).WithMany(p => p.RoleModuleAndPermissions)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_RoleModulePermission_Role");
            });

            modelBuilder.Entity<ServiceProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC07B0695346");

                entity.ToTable("ServiceProvider", "AxionPro");

                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Ceoname)
                    .HasMaxLength(255)
                    .HasColumnName("CEOName");
                entity.Property(e => e.CompanyEmail).HasMaxLength(255);
                entity.Property(e => e.CompanyName).HasMaxLength(255);
                entity.Property(e => e.CompanyType).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Fax).HasMaxLength(50);
                entity.Property(e => e.Gstnumber)
                    .HasMaxLength(50)
                    .HasColumnName("GSTNumber");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.PinCode).HasMaxLength(10);
                entity.Property(e => e.Profile).HasMaxLength(500);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.WebsiteUrl)
                    .HasMaxLength(255)
                    .HasColumnName("WebsiteURL");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__State__3214EC0705AF0D74");

                entity.ToTable("State", "AxionPro");

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.StateName).HasMaxLength(100);

                entity.HasOne(d => d.Country).WithMany(p => p.States)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__State__CountryId__6B0FDBE9");

            
            });

            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07BED0F0D0");

                entity.ToTable("SubscriptionPlan", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.CurrencyKey).HasMaxLength(20);
                entity.Property(e => e.MonthlyPrice).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PerDayPrice).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PlanName).HasMaxLength(100);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.YearlyPrice).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Tenant__3214EC0728DD7C6E");

                entity.ToTable("Tenant", "AxionPro");

                entity.HasIndex(e => e.TenantCode);

                entity.HasIndex(e => e.TenantEmail, "UQ__Tenant__F7C944DD7E3D53D9").IsUnique();

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.CompanyEmailDomain).HasMaxLength(255);
                entity.Property(e => e.CompanyName).HasMaxLength(200);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.ContactPersonName).HasMaxLength(100);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.TenantCode).HasMaxLength(100);
                entity.Property(e => e.TenantEmail).HasMaxLength(200);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.TenantIndustry).WithMany(p => p.Tenants)
                    .HasForeignKey(d => d.TenantIndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tenant_TenantIndustry");
                 entity.HasOne(d => d.Gender).WithMany(p => p.Tenants)
                 .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Gender_Tenant");



            });

            modelBuilder.Entity<TenantEmailConfig>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantEm__3214EC077A90B3A3");

                entity.ToTable("TenantEmailConfig", "AxionPro");

                entity.Property(e => e.FromEmail).HasMaxLength(200);
                entity.Property(e => e.FromName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.SmtpHost).HasMaxLength(200);
                entity.Property(e => e.SmtpPasswordEncrypted).HasMaxLength(500);
                entity.Property(e => e.SmtpUsername).HasMaxLength(200);

                entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEmailConfigs)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_TenantEmailConfig_Tenant");
            });

            modelBuilder.Entity<TenantEnabledModule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC07AC610092");

                entity.ToTable("TenantEnabledModule", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsEnabled).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Module).WithMany(p => p.TenantEnabledModules)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantEnabledModules_Module");
                entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEnabledModules)
                   .HasForeignKey(d => d.TenantId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_TenantEnabledModules_Tenant");


            });

            modelBuilder.Entity<TenantEnabledOperation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC07047C5B46");

                entity.ToTable("TenantEnabledOperation", "AxionPro");

               

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsEnabled).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Module).WithMany(p => p.TenantEnabledOperations)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantEnabledOperations_Module");

                entity.HasOne(d => d.Operation).WithMany(p => p.TenantEnabledOperations)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantEnabledOperations_Operation");

                entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEnabledOperations)
          .HasForeignKey(d => d.TenantId)
          .OnDelete(DeleteBehavior.ClientSetNull)
          .HasConstraintName("FK_TenantEnabledOperations_Tenant");

            });

            modelBuilder.Entity<TenantIndustry>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantIn__3214EC0765258C5E");

                entity.ToTable("TenantIndustry", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IndustryName).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.SoftDeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TenantProfile>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantPr__3214EC0796A1B93D");

                entity.ToTable("TenantProfile", "AxionPro");

                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.BusinessType).HasMaxLength(100);
                entity.Property(e => e.Industry).HasMaxLength(100);
                entity.Property(e => e.LogoUrl).HasMaxLength(255);
                entity.Property(e => e.ThemeColor).HasMaxLength(50);
                entity.Property(e => e.WebsiteUrl).HasMaxLength(200);

                entity.HasOne(d => d.Tenant).WithMany(p => p.TenantProfiles)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_TenantProfile_Tenant");
            });

            modelBuilder.Entity<TenantSubscription>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantSu__3214EC07CF143048");

                entity.ToTable("TenantSubscription", "AxionPro");

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PaymentMode).HasMaxLength(50);
                entity.Property(e => e.PaymentTxnId).HasMaxLength(100);
                entity.Property(e => e.SubscriptionStartDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.TenantSubscriptions)
                    .HasForeignKey(d => d.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantSubscription_Plan");

                entity.HasOne(d => d.Tenant).WithMany(p => p.TenantSubscriptions)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantSubscription_Tenant");
            });

            modelBuilder.Entity<Tender>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Tender__3214EC076E847502");

                entity.ToTable("Tender", "AxionPro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.TenderName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.TenderValue).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Client).WithMany(p => p.Tenders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tender_ClientId");

                entity.HasOne(d => d.TenderStatus).WithMany(p => p.Tenders)
                    .HasForeignKey(d => d.TenderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Tender__TenderSt__3EDC53F0");
            });

            modelBuilder.Entity<TenantEncryptionKey>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC074AAC9A36");

                entity.ToTable("TenantEncryptionKeys", "AxionPro");

                entity.HasIndex(e => e.TenantId, "UQ_TenantKey_TenantId").IsUnique();

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EncryptionKey).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });
            modelBuilder.Entity<TenderProject>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderPr__3214EC073A6AC46E");

                entity.ToTable("TenderProject", "AxionPro");

                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.EstimatedBudget).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ProjectName).HasMaxLength(255);
                entity.Property(e => e.Remark).HasMaxLength(1000);
                entity.Property(e => e.StartDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Status).WithMany(p => p.TenderProjects)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderPro__Statu__40C49C62");

                entity.HasOne(d => d.TenderServiceProvider).WithMany(p => p.TenderProjects)
                    .HasForeignKey(d => d.TenderServiceProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenderProject_TenderServiceProvider");

                entity.HasOne(d => d.UserRole).WithMany(p => p.TenderProjects)
                    .HasForeignKey(d => d.UserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderPro__UserR__42ACE4D4");
            });

            modelBuilder.Entity<TenderService>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07620086F4");

                entity.ToTable("TenderService", "AxionPro");

                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(500);

                entity.HasOne(d => d.Tender).WithMany(p => p.TenderServices)
                    .HasForeignKey(d => d.TenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderSer__Tende__43A1090D");

                entity.HasOne(d => d.TenderServiceType).WithMany(p => p.TenderServices)
                    .HasForeignKey(d => d.TenderServiceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderSer__Tende__44952D46");
            });

            modelBuilder.Entity<TenderServiceHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07F4765A9B");

                entity.ToTable("TenderServiceHistory", "AxionPro");

                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TenderServiceProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC0765F55078");

                entity.ToTable("TenderServiceProvider", "AxionPro");

                entity.Property(e => e.ContractAmount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsInHouse).HasDefaultValue(false);
                entity.Property(e => e.IsPrimaryProvider).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(500);

                entity.HasOne(d => d.Status).WithMany(p => p.TenderServiceProviders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderSer__Statu__4589517F");

                entity.HasOne(d => d.TenderServiceSpecification).WithMany(p => p.TenderServiceProviders)
                    .HasForeignKey(d => d.TenderServiceSpecificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenderServiceProvider_TenderServiceSpecification");

                entity.HasOne(d => d.TenderServiceSpecificationNavigation).WithMany(p => p.TenderServiceProviders)
                    .HasForeignKey(d => d.TenderServiceSpecificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TenderSer__Tende__467D75B8");
            });

            modelBuilder.Entity<TenderServiceSpecification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07FBF10311");

                entity.ToTable("TenderServiceSpecification", "AxionPro");

                entity.Property(e => e.EstimatedBudget).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ProductPlatform).HasMaxLength(255);
                entity.Property(e => e.ProductSpecification).HasMaxLength(1000);
                entity.Property(e => e.SpecificationName).HasMaxLength(255);
                entity.Property(e => e.SpecificationType).HasMaxLength(50);

                entity.HasOne(d => d.TenderService).WithMany(p => p.TenderServiceSpecifications)
                    .HasForeignKey(d => d.TenderServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenderServiceSpecification_TenderService");
            });

            modelBuilder.Entity<TenderServiceType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC072E505507");

                entity.ToTable("TenderServiceType", "AxionPro");

                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ServiceName).HasMaxLength(255);
            });

            modelBuilder.Entity<TenderStatus>(entity =>
            {
                entity.ToTable("TenderStatus", "AxionPro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.StatusName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TicketClassification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TicketCl__3214EC07BA509F8F");

                entity.ToTable("TicketClassification", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ClassificationName).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.SoftDeletedTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.TicketClassifications)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_TicketClassification_Tenant");
            });

            modelBuilder.Entity<TicketHeader>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TicketHe__3214EC07EADE3F1B");

                entity.ToTable("TicketHeader", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.HeaderName).HasMaxLength(150);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.SoftDeletedTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tenant).WithMany(p => p.TicketHeaders)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_TicketHeader_TenantId");

                entity.HasOne(d => d.TicketClassification).WithMany(p => p.TicketHeaders)
                    .HasForeignKey(d => d.TicketClassificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketHeaderType_TicketClassification");
            });

            modelBuilder.Entity<TicketType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TicketTy__3214EC07554DC568");

                entity.ToTable("TicketType", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(true);
                entity.Property(e => e.SoftDeletedTime).HasColumnType("datetime");
                entity.Property(e => e.TicketTypeName).HasMaxLength(100);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ResponsibleRole).WithMany(p => p.TicketTypes)
                    .HasForeignKey(d => d.ResponsibleRoleId)
                    .HasConstraintName("FK_TicketType_ResponsibleRole");

                entity.HasOne(d => d.Tenant).WithMany(p => p.TicketTypes)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_TicketType_Tenant");

                entity.HasOne(d => d.TicketHeader).WithMany(p => p.TicketTypes)
                    .HasForeignKey(d => d.TicketHeaderId)
                    .HasConstraintName("FK_TicketType_Header");
            });

            modelBuilder.Entity<TravelAllowancePolicyByDesignation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TravelAl__3214EC072DD6C86A");

                entity.ToTable("TravelAllowancePolicyByDesignation", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.AdvanceAllowed).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMetro).HasDefaultValue(false);
                entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
                entity.Property(e => e.MaxAdvanceAmount)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MetroBonus)
                    .HasDefaultValue(0.00m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ReimbursementPerKm)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("ReimbursementPerKM");
                entity.Property(e => e.RequiredDocuments).HasColumnType("text");
                entity.Property(e => e.SoftDeleteDateTime).HasColumnType("datetime");
                entity.Property(e => e.TravelClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Designation).WithMany(p => p.TravelAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.DesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TravelAll__Desig__4959E263");

                entity.HasOne(d => d.EmployeeType).WithMany(p => p.TravelAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.EmployeeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TravelAll__Emplo__4A4E069C");

                entity.HasOne(d => d.PolicyType).WithMany(p => p.TravelAllowancePolicyByDesignations)
                    .HasForeignKey(d => d.PolicyTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TravelAll__Polic__4B422AD5");
            });

            modelBuilder.Entity<TravelMode>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TravelMo__3214EC075A97AC69");

                entity.ToTable("TravelMode", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.TravelModeName).HasMaxLength(255);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserAttendanceSetting>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__UserAtte__3214EC0750CE7646");

                entity.ToTable("UserAttendanceSetting", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.GeofenceLatitude).HasColumnType("decimal(10, 8)");
                entity.Property(e => e.GeofenceLongitude).HasColumnType("decimal(10, 8)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsAllowed).HasDefaultValue(true);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.ReportingTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AttendanceDeviceType).WithMany(p => p.UserAttendanceSettings)
                    .HasForeignKey(d => d.AttendanceDeviceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAttendanceSetting_AttendanceDeviceType");

                entity.HasOne(d => d.Employee).WithMany(p => p.UserAttendanceSettings)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_UserAttendanceSetting_Employee");

                entity.HasOne(d => d.WorkstationType).WithMany(p => p.UserAttendanceSettings)
                    .HasForeignKey(d => d.WorkstationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAttendanceSetting_WorkstationType");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07C9E25EB9");

                entity.ToTable("UserRole", "AxionPro");

                entity.Property(e => e.AddedDateTime).HasColumnType("datetime");
                entity.Property(e => e.ApprovalStatus)
                    .HasMaxLength(20)
                    .IsFixedLength();
                entity.Property(e => e.AssignedDateTime).HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.Remark).HasMaxLength(255);
                entity.Property(e => e.RemovedDateTime).HasColumnType("datetime");
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_Role_Employee");

                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserRole_Role");
            });

            modelBuilder.Entity<WorkflowStage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC07428E99CC");

                entity.ToTable("WorkflowStage", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ColorKey).HasMaxLength(50);
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.StageName).HasMaxLength(100);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<WorkflowStep>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC07E62FF378");

                entity.ToTable("WorkflowStep", "AxionPro");

                entity.Property(e => e.AddedDateTime)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsMandatory).HasDefaultValue(true);
                entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });
            modelBuilder.Entity<RoleModuleOperationResponseDTO>().HasNoKey();

        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }



}
