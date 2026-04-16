using axionpro.application.DTOs.Module.NewFolder;

using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Interfaces.IContext;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace axionpro.persistance.Data.Context
{
    public class WorkforceDbContext : DbContext, IWorkforcedbContext
    {


        public WorkforceDbContext(DbContextOptions<WorkforceDbContext> options) : base(options)
        {

        }


        public DbSet<RoleModuleOperationResponseDTO> RoleModuleOperationResponse { get; set; }


        public virtual DbSet<EmployeeContact> EmployeeContacts { get; set; }
        public virtual DbSet<District> Districts { get; set; }
      //  public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public virtual DbSet<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignations { get; set; }

        public virtual DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }
        public virtual DbSet<EmployeePolicyDependentMapping> EmployeePolicyDependentMapping { get; set; }

        public virtual DbSet<EmployeePolicyEnrollment> EmployeePolicyEnrollment { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<PolicyTypeInsuranceMapping> PolicyTypeInsuranceMappings { get; set; }
        public virtual DbSet<AssetAssignment> AssetAssignments { get; set; }

        public virtual DbSet<AssetCategory> AssetCategories { get; set; }

        public virtual DbSet<AssetHistory> AssetHistories { get; set; }
        public virtual DbSet<UnStructuredPolicyTypeMappingWithEmployeeType> UnStructuredPolicyTypeMappingWithEmployeeTypes { get; set; }

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
        public virtual DbSet<DeviceCommandMaster> DeviceCommandMasters { get; set; }

        public virtual DbSet<DeviceCommandQueue> DeviceCommandQueues { get; set; }

        public virtual DbSet<DeviceLogRaw> DeviceLogRaws { get; set; }

        public virtual DbSet<DeviceMaster> DeviceMasters { get; set; }


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
        
        public virtual DbSet<EmployeeExperience> EmployeeExperienceDetails { get; set; }

        public virtual DbSet<EmployeeExperienceDocument> EmployeeExperienceDocuments { get; set; }

        public virtual DbSet<EmployeeImage> EmployeeImages { get; set; }

     //   public virtual DbSet<EmployeeInsuranceMapping> EmployeeInsuranceMappings { get; set; }
        public virtual DbSet<PolicyTypeDocument> PolicyTypeDocuments { get; set; }
        public virtual DbSet<InsurancePolicyDocument> InsurancePolicyDocuments { get; set; }


        public virtual DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }

        public virtual DbSet<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMappings { get; set; }

        public virtual DbSet<EmployeeManagerMapping> EmployeeManagerMappings { get; set; }

        public virtual DbSet<EmployeePersonalDetail> EmployeePersonalDetails { get; set; }

        public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }

        public virtual DbSet<EmployeeTypeBasicMenu> EmployeeTypeBasicMenus { get; set; }

        public virtual DbSet<EmployeesChangedTypeHistory> EmployeesChangedTypeHistories { get; set; }

        public virtual DbSet<ForgotPasswordOtpdetail> ForgotPasswordOtpdetails   { get; set; }

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
        public virtual DbSet<TenantEncryptionKeys> TenantEncryptionKeys { get; set; }
      

        public virtual DbSet<ThreadMessage> ThreadMessage { get; set; }

        public virtual DbSet<Ticket> Ticket { get; set; }

        public virtual DbSet<TicketAttachment> TicketAttachment { get; set; }
        public virtual DbSet<TicketHeader> TicketHeaders { get; set; }

        public virtual DbSet<TicketType> TicketTypes { get; set; }

        public virtual DbSet<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignations { get; set; }

        public virtual DbSet<TravelMode> TravelModes { get; set; }

        public virtual DbSet<UserAttendanceSetting> UserAttendanceSettings { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<EmployeeCodePattern> EmployeeCodePatterns { get; set; }

        public virtual DbSet<WorkflowStage> WorkflowStages { get; set; }

        public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

        public virtual DbSet<WorkstationType> WorkstationTypes { get; set; }
        public virtual DbSet<SubscribedModuleResponseDTO> SubscribedModuleResponseDTOs { get; set; }
        public virtual DbSet<FlatModuleOperationDto> TenantModulesConfigurations { get; set; }
        public virtual DbSet<GetEmployeeIdentitySp> GetEmployeeIdentitySps { get; set; }
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
      

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //    => optionsBuilder.UseSqlServer("Server=acer;Database=workforcedb-dev-pre;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;Command Timeout=300");

      protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccoumndationAllowancePolicyByDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accoumnd__3214EC071BDF4022");

            entity.ToTable("AccoumndationAllowancePolicyByDesignation", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FixedStayAllowance)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMetro).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
            entity.Property(e => e.MetroBonus)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.MinDaysRequired).HasDefaultValue(0);

            entity.HasOne(d => d.Designation).WithMany(p => p.AccoumndationAllowancePolicyByDesignation)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accoumnda__Desig__11158940");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.AccoumndationAllowancePolicyByDesignation)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accoumnda__Emplo__1209AD79");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.AccoumndationAllowancePolicyByDesignation)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accoumnda__Polic__12FDD1B2");
        });

        modelBuilder.Entity<ApprovalWorkflow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Approval__3214EC071F76BFC6");

            entity.ToTable("ApprovalWorkflow", "axionpro");

            entity.Property(e => e.ActionName).HasMaxLength(150);
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(250);
            entity.Property(e => e.WorkflowName).HasMaxLength(150);
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Asset__3214EC076178ABAE");

            entity.ToTable("Asset", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AssetName).HasMaxLength(100);
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Company).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAssigned).HasDefaultValue(false);
            entity.Property(e => e.IsRepairable).HasDefaultValue(true);
            entity.Property(e => e.ModelNo).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Qrcode)
                .HasMaxLength(500)
                .HasColumnName("qrcode");
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(50);
            entity.Property(e => e.SoftDeletedById).HasDefaultValue(0L);
            entity.Property(e => e.Weight).HasMaxLength(50);

            entity.HasOne(d => d.AssetStatus).WithMany(p => p.Asset)
                .HasForeignKey(d => d.AssetStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_AssetStatus");

            entity.HasOne(d => d.AssetType).WithMany(p => p.Asset)
                .HasForeignKey(d => d.AssetTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_AssetType");
        });

        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AssetAssignment_pkey");

            entity.ToTable("AssetAssignment", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"AssetAssignment_Id_seq\"'::regclass)");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.AssetConditionAtAssign).HasMaxLength(255);
            entity.Property(e => e.AssetConditionAtReturn).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Asset).WithMany(p => p.AssetAssignment)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("FK_AssetAssignment_Asset");

            entity.HasOne(d => d.Request).WithMany(p => p.AssetAssignment)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetAssignment_Request");
        });

        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetCat__3214EC07E9FE2792");

            entity.ToTable("AssetCategory", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CategoryName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.Tenant).WithMany(p => p.AssetCategory)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetCategory_Tenant");
        });

        modelBuilder.Entity<AssetHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetHis__3214EC07599816A6");

            entity.ToTable("AssetHistory", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AssetConditionAtAssign).HasMaxLength(100);
            entity.Property(e => e.AssetConditionAtReturn).HasMaxLength(100);
            entity.Property(e => e.IdentificationMethod).HasMaxLength(50);
            entity.Property(e => e.IdentificationValue).HasMaxLength(255);
            entity.Property(e => e.IsScrapped).HasDefaultValue(false);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.ScrapReason).HasMaxLength(255);

            entity.HasOne(d => d.AssignmentStatus).WithMany(p => p.AssetHistory)
                .HasForeignKey(d => d.AssignmentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AssetHist__Assig__7AF13DF7");

            entity.HasOne(d => d.Employee).WithMany(p => p.AssetHistoryEmployee)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__AssetHist__Emplo__79FD19BE");

            entity.HasOne(d => d.ScrapApprovedByNavigation).WithMany(p => p.AssetHistoryScrapApprovedByNavigation)
                .HasForeignKey(d => d.ScrapApprovedBy)
                .HasConstraintName("FK__AssetHist__Scrap__7BE56230");
        });

        modelBuilder.Entity<AssetImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetIma__3214EC0752335BEC");

            entity.ToTable("AssetImage", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AssetImagePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.Asset).WithMany(p => p.AssetImage)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetImage_Asset");
        });

        modelBuilder.Entity<AssetRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AssetRequest_pkey");

            entity.ToTable("AssetRequest", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"AssetRequest_Id_seq\"'::regclass)");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.RequestDate).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<AssetStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetSta__3214EC0722A59445");

            entity.ToTable("AssetStatus", "axionpro");

            entity.HasIndex(e => e.StatusName, "UQ__AssetSta__05E7698A7AA7A28E").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.ColorKey).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.SoftDeletedById).HasDefaultValue(0L);
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedById).HasDefaultValue(0L);
        });

        modelBuilder.Entity<AssetTicketTypeDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetTic__3214EC0706DC5F37");

            entity.ToTable("AssetTicketTypeDetail", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.AssetType).WithMany(p => p.AssetTicketTypeDetail)
                .HasForeignKey(d => d.AssetTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetType_AssetTckTypeDetail_ID");

            entity.HasOne(d => d.ResponsibleRole).WithMany(p => p.AssetTicketTypeDetail)
                .HasForeignKey(d => d.ResponsibleRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponsibleRole_AssetTckTypeDetail_ID");

            entity.HasOne(d => d.TicketType).WithMany(p => p.AssetTicketTypeDetail)
                .HasForeignKey(d => d.TicketTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TckType_AstTypeDetail_ID");
        });

        modelBuilder.Entity<AssetType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetTyp__3214EC077375A9AA");

            entity.ToTable("AssetType", "axionpro");

            entity.HasIndex(e => e.TypeName, "UQ__AssetTyp__D4E7DFA8692FD5DF").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TypeName).HasMaxLength(100);

            entity.HasOne(d => d.AssetCategory).WithMany(p => p.AssetType)
                .HasForeignKey(d => d.AssetCategoryId)
                .HasConstraintName("FK_AssetType_AssetCategory");
        });

        modelBuilder.Entity<AssignmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assignme__3214EC07BCFD76FA");

            entity.ToTable("AssignmentStatus", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC079EEE2ABB");

            entity.ToTable("Attendance", "axionpro");

            entity.Property(e => e.AttendanceImagePath).HasMaxLength(200);
            entity.Property(e => e.AttendanceImageUrl)
                .HasMaxLength(200)
                .HasColumnName("AttendanceImageURL");
        });

        modelBuilder.Entity<AttendanceDeviceType>(entity =>
        {
            entity.ToTable("AttendanceDeviceType", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.IsDeviceRegister).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(255);
        });

        modelBuilder.Entity<AttendanceHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC07AC1B1F0C");

            entity.ToTable("AttendanceHistory", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalBreakHours).HasPrecision(5, 2);
            entity.Property(e => e.TotalWorkHours).HasPrecision(5, 2);

            entity.HasOne(d => d.Employee).WithMany(p => p.AttendanceHistory)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceHistory_Employee");
        });

        modelBuilder.Entity<AttendanceLogs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AttendanceLogs_pkey");

            entity.ToTable("AttendanceLogs", "axionpro");

            entity.HasIndex(e => new { e.EmployeeCode, e.PunchTime }, "IX_AttendanceLogs_Employee_Time");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"AttendanceLogs_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DeviceSn).HasMaxLength(50);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
        });

        modelBuilder.Entity<AttendanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC07DA4D2CA6");

            entity.ToTable("AttendanceRequest", "axionpro");

            entity.Property(e => e.AttendanceImagePath).HasMaxLength(200);
            entity.Property(e => e.AttendanceImageUrl)
                .HasMaxLength(200)
                .HasColumnName("AttendanceImageURL");
            entity.Property(e => e.Remark).HasMaxLength(255);
        });

        modelBuilder.Entity<BasicMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BasicMenu_Id");

            entity.ToTable("BasicMenu", "axionpro");

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

            entity.ToTable("Candidate", "axionpro");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Candidat__85FB4E384EF74606").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Candidat__A9D10534B67C36A0").IsUnique();

            entity.HasIndex(e => e.Aadhaar, "UQ__Candidat__C4B3336970636589").IsUnique();

            entity.HasIndex(e => e.Pan, "UQ__Candidat__C577943D11665D42").IsUnique();

            entity.HasIndex(e => e.CandidateReferenceCode, "UQ__Candidat__CF22B81A936408F4").IsUnique();

            entity.Property(e => e.Aadhaar).HasMaxLength(12);
            entity.Property(e => e.ActionStatus).HasMaxLength(20);
            entity.Property(e => e.AppliedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CandidateReferenceCode).HasMaxLength(20);
            entity.Property(e => e.CurrentCompany).HasMaxLength(200);
            entity.Property(e => e.CurrentLocation).HasMaxLength(200);
            entity.Property(e => e.Education).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.ExpectedSalary).HasPrecision(18, 2);
            entity.Property(e => e.ExperienceYears).HasPrecision(4, 1);
            entity.Property(e => e.FewWords).HasMaxLength(1000);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("now()");
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

            entity.ToTable("CandidateCategorySkill", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateCategorySkill)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Candidate__Candi__15DA3E5D");

            entity.HasOne(d => d.Category).WithMany(p => p.CandidateCategorySkill)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Candidate__Categ__16CE6296");
        });

        modelBuilder.Entity<CandidateHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC07F93DEB9C");

            entity.ToTable("CandidateHistory", "axionpro");

            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateHistory)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Candidate__Candi__17C286CF");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC070D4D0C80");

            entity.ToTable("Category", "axionpro");

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

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__City__3214EC07DC3B7144");

            entity.ToTable("City", "axionpro");

            entity.Property(e => e.CityName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.State).WithMany(p => p.City)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__City__StateId__6EE06CCD");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TenderClient");

            entity.ToTable("Client", "axionpro");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ClientName).HasMaxLength(255);
            entity.Property(e => e.ContactPerson).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.ClientType).WithMany(p => p.Client)
                .HasForeignKey(d => d.ClientTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_ClientType");
        });

        modelBuilder.Entity<ClientType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientTy__3214EC078D984A16");

            entity.ToTable("ClientType", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Country__3214EC070584DCC0");

            entity.ToTable("Country", "axionpro");

            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Stdcode)
                .HasMaxLength(10)
                .HasColumnName("STDCode");
        });

        modelBuilder.Entity<CountryIdentityRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountryI__3214EC0748E4A649");

            entity.ToTable("CountryIdentityRule", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);

            entity.HasOne(d => d.Country).WithMany(p => p.CountryIdentityRule)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Country");

            entity.HasOne(d => d.IdentityCategoryDocument).WithMany(p => p.CountryIdentityRule)
                .HasForeignKey(d => d.IdentityCategoryDocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Country_Document");
        });

        modelBuilder.Entity<CountryStatutoryRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountryS__3214EC07068D8E4F");

            entity.ToTable("CountryStatutoryRule", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SalaryThreshold).HasPrecision(18, 2);

            entity.HasOne(d => d.Country).WithMany(p => p.CountryStatutoryRule)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CountryStatutoryRule_Country");

            entity.HasOne(d => d.StatutoryType).WithMany(p => p.CountryStatutoryRule)
                .HasForeignKey(d => d.StatutoryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CountryStatutoryRule_Statutory");
        });

        modelBuilder.Entity<DataViewStructure>(entity =>
        {
            entity.ToTable("DataViewStructure", "axionpro");

            entity.Property(e => e.Discription).HasMaxLength(150);
            entity.Property(e => e.DisplayOn).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(150);
        });

        modelBuilder.Entity<DayCombination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DayCombi__3214EC070D2CF976");

            entity.ToTable("DayCombination", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CombinationName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(250);

            entity.HasOne(d => d.Tenant).WithMany(p => p.DayCombination)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_DayCombination_Tenant");
        });

        modelBuilder.Entity<DemoRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DemoRequ__3214EC0796C55902");

            entity.ToTable("DemoRequest", "axionpro");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("(gen_random_uuid())::text");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.CurrentHrms)
                .HasMaxLength(200)
                .HasColumnName("CurrentHRMS");
            entity.Property(e => e.DeploymentPreference).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Hrchallenges).HasColumnName("HRChallenges");
            entity.Property(e => e.IndustryType).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying");
            entity.Property(e => e.WorkEmail).HasMaxLength(150);
        });

        modelBuilder.Entity<DemoRequestBiometricDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DemoRequ__3214EC0746BD550C");

            entity.ToTable("DemoRequestBiometricDetail", "axionpro");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("(gen_random_uuid())::text");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.BiometricCompanyName).HasMaxLength(200);
            entity.Property(e => e.DemoRequestId).HasMaxLength(50);
            entity.Property(e => e.MachineLocation).HasMaxLength(250);
            entity.Property(e => e.ModelNumber).HasMaxLength(150);

            entity.HasOne(d => d.DemoRequest).WithMany(p => p.DemoRequestBiometricDetail)
                .HasForeignKey(d => d.DemoRequestId)
                .HasConstraintName("FK_DemoRequestBiometric");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC071E7B0B0B");

            entity.ToTable("Department", "axionpro");

            entity.Property(e => e.DepartmentName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Remark).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Department)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_Department_Tenant");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Designat__3214EC07F9FF4C75");

            entity.ToTable("Designation", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DesignationName).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Designation)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Designation_Department");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Designation)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designation_Tenant");
        });

        modelBuilder.Entity<DeviceCommandMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DeviceCommandMaster_pkey");

            entity.ToTable("DeviceCommandMaster", "axionpro");

            entity.HasIndex(e => e.CommandName, "UQ_DeviceCommandMaster_CommandName").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"DeviceCommandMaster_Id_seq\"'::regclass)");
            entity.Property(e => e.CommandName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DeviceCommandQueue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DeviceCommandQueue_pkey");

            entity.ToTable("DeviceCommandQueue", "axionpro");

            entity.HasIndex(e => e.Status, "IX_DeviceCommandQueue_Status");

            entity.HasIndex(e => new { e.TenantId, e.DeviceId }, "IX_DeviceCommandQueue_Tenant_Device");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"DeviceCommandQueue_Id_seq\"'::regclass)");
            entity.Property(e => e.CommandName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DeviceSn).HasMaxLength(50);
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(0);

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceCommandQueue)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_DeviceCommandQueue_DeviceMaster");
        });

        modelBuilder.Entity<DeviceLogRaw>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DeviceLogRaw_pkey");

            entity.ToTable("DeviceLogRaw", "axionpro");

            entity.HasIndex(e => e.IsProcessed, "IX_DeviceLogRaw_Processed");

            entity.HasIndex(e => new { e.TenantId, e.DeviceSn }, "IX_DeviceLogRaw_Tenant_Device");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"DeviceLogRaw_Id_seq\"'::regclass)");
            entity.Property(e => e.CommandName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DeviceSn).HasMaxLength(50);
            entity.Property(e => e.IsProcessed).HasDefaultValue(false);
        });

        modelBuilder.Entity<DeviceMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DeviceMaster_pkey");

            entity.ToTable("DeviceMaster", "axionpro");

            entity.HasIndex(e => new { e.TenantId, e.DeviceSn }, "UQ_DeviceMaster_Tenant_DeviceSn").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"DeviceMaster_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.DeviceSn).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC07CB6A47D0");

            entity.ToTable("District", "axionpro");

            entity.HasIndex(e => e.DistrictName, "IX_District_DistrictName");

            entity.HasIndex(e => e.StateId, "IX_District_StateId");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DistrictCode).HasMaxLength(50);
            entity.Property(e => e.DistrictName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PinCode).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.State).WithMany(p => p.District)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_District_State");
        });

        modelBuilder.Entity<DistrictMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC07D509FB7C");

            entity.ToTable("DistrictMaster", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.DistrictCode).HasMaxLength(50);
            entity.Property(e => e.DistrictName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PinCode).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(500);
        });

        modelBuilder.Entity<EmailQueue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailQue__3214EC0705218438");

            entity.ToTable("EmailQueue", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsSent).HasDefaultValue(false);
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.ToEmail).HasMaxLength(250);

            entity.HasOne(d => d.Template).WithMany(p => p.EmailQueue)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmailQueu__Templ__44B528D7");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTem__3214EC072FE49A64");

            entity.ToTable("EmailTemplate", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
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
            entity.Property(e => e.UpdatedFromIp)
                .HasMaxLength(50)
                .HasColumnName("UpdatedFromIP");
        });

        modelBuilder.Entity<EmailsLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailsLo__3214EC0779530258");

            entity.ToTable("EmailsLog", "axionpro");

            entity.Property(e => e.AddedFromIp).HasMaxLength(50);
            entity.Property(e => e.BccEmail).HasMaxLength(1000);
            entity.Property(e => e.CcEmail).HasMaxLength(1000);
            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Queued'::character varying");
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.ToEmail).HasMaxLength(500);
            entity.Property(e => e.TriggeredBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07E3264254");

            entity.ToTable("Employee", "axionpro");

            entity.HasIndex(e => e.OfficialEmail, "UX_Employee_SystemUser_OnlyOnce").IsUnique();

            entity.HasIndex(e => e.Id, "UX_Employee_TenantIdNullOnce").IsUnique();

            entity.HasIndex(e => e.Id, "UX_Employee_TenantId_Null_OnlyOnce").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.BloodGroup).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(50);
            entity.Property(e => e.EmergencyContactPerson).HasMaxLength(100);
            entity.Property(e => e.EmployementCode).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(50);
            entity.Property(e => e.OfficialEmail).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(200);

            entity.HasOne(d => d.Country).WithMany(p => p.Employee)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Country");

            entity.HasOne(d => d.Designation).WithMany(p => p.Employee)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Employee_Designation");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.Employee)
                .HasForeignKey(d => d.EmployeeTypeId)
                .HasConstraintName("FK_Employee_EmployeeType");

            entity.HasOne(d => d.Gender).WithMany(p => p.Employee)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Employee_Gender");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Employee)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Employee_Tenant");
        });

        modelBuilder.Entity<EmployeeBankDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC072E9F930F");

            entity.ToTable("EmployeeBankDetail", "axionpro");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.BranchName).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(20)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.IsPrimaryAccount).HasDefaultValue(true);
            entity.Property(e => e.Upiid)
                .HasMaxLength(100)
                .HasColumnName("UPIId");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeBankDetail)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeBank_Employee");
        });

        modelBuilder.Entity<EmployeeCategorySkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07BB3C49C9");

            entity.ToTable("EmployeeCategorySkill", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.EmployeeCategorySkill)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_EmployeeCategorySkill_Category");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeCategorySkill)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_EmployeeCategorySkill_Employee");
        });

        modelBuilder.Entity<EmployeeCodePattern>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC070ECDA69B");

            entity.ToTable("EmployeeCodePattern", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Prefix).HasMaxLength(20);
            entity.Property(e => e.RunningNumberLength).HasDefaultValue(4);
            entity.Property(e => e.Separator)
                .HasMaxLength(5)
                .HasDefaultValueSql("'/'::character varying");

            entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeCodePattern)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeCodePattern_Tenant");
        });

        modelBuilder.Entity<EmployeeContact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07371CFD87");

            entity.ToTable("EmployeeContact", "axionpro");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.AlternateNumber).HasMaxLength(20);
            entity.Property(e => e.ContactName).HasMaxLength(20);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HouseNo).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEditAllowed).HasDefaultValue(true);
            entity.Property(e => e.IsInfoVerified).HasDefaultValue(false);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(true);
            entity.Property(e => e.LandMark).HasMaxLength(250);
            entity.Property(e => e.Remark).HasMaxLength(250);
            entity.Property(e => e.Street).HasMaxLength(250);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeContact)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_EmployeeContact_Employee");
        });

        modelBuilder.Entity<EmployeeDailyAttendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC078C36F2DB");

            entity.ToTable("EmployeeDailyAttendance", "axionpro");

            entity.Property(e => e.IsLate).HasDefaultValue(false);

            entity.HasOne(d => d.AttendanceDeviceType).WithMany(p => p.EmployeeDailyAttendance)
                .HasForeignKey(d => d.AttendanceDeviceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceDeviceType");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDailyAttendance)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee");

            entity.HasOne(d => d.WorkstationType).WithMany(p => p.EmployeeDailyAttendance)
                .HasForeignKey(d => d.WorkstationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkstationType");
        });

        modelBuilder.Entity<EmployeeDependent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074B6A13E2");

            entity.ToTable("EmployeeDependent", "axionpro");

            entity.Property(e => e.DependentName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.IsCoveredInPolicy).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.UpdatedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDependent)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_EmployeeDependents_Employee");
        });

        modelBuilder.Entity<EmployeeEducation>(entity =>
        {
            entity.ToTable("EmployeeEducation", "axionpro");

            entity.Property(e => e.Degree).HasMaxLength(50);
            entity.Property(e => e.EducationGap).HasDefaultValue(false);
            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.GradeDivision).HasMaxLength(10);
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.ReasonOfEducationGap).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(100);
            entity.Property(e => e.ScoreValue).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeExperience>(entity =>
        {
            entity.ToTable("EmployeeExperience", "axionpro");

            entity.Property(e => e.ColleagueContactNumber).HasMaxLength(20);
            entity.Property(e => e.ColleagueDesignation).HasMaxLength(100);
            entity.Property(e => e.ColleagueName).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.Ctc)
                .HasPrecision(18, 2)
                .HasColumnName("CTC");
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.EmployeeIdOfCompany).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAnyGap).HasDefaultValue(false);
            entity.Property(e => e.IsForeignExperience).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsWfh)
                .HasDefaultValue(false)
                .HasColumnName("IsWFH");
            entity.Property(e => e.ReasonForLeaving).HasMaxLength(200);
            entity.Property(e => e.ReasonOfGap).HasMaxLength(500);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.ReportingManagerName).HasMaxLength(100);
            entity.Property(e => e.ReportingManagerNumber).HasMaxLength(20);
            entity.Property(e => e.VerificationEmail).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeExperience)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_EmployeeExperience_Employee");
        });

        modelBuilder.Entity<EmployeeExperienceDocument>(entity =>
        {
            entity.ToTable("EmployeeExperienceDocument", "axionpro");

            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.HasExperienceDocUploaded).HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(200);

            entity.HasOne(d => d.EmployeeExperience).WithMany(p => p.EmployeeExperienceDocument)
                .HasForeignKey(d => d.EmployeeExperienceId)
                .HasConstraintName("FK_EmployeeExperienceDocument_Experience");
        });

        modelBuilder.Entity<EmployeeIdentity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074FF9BCC1");

            entity.ToTable("EmployeeIdentity", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.DocumentFileName).HasMaxLength(255);
            entity.Property(e => e.DocumentFilePath).HasMaxLength(500);
            entity.Property(e => e.EffectiveFrom).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IdentityValue).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEditAllowed).HasDefaultValue(true);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeIdentity)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeIdentity_Employee");

            entity.HasOne(d => d.IdentityCategoryDocument).WithMany(p => p.EmployeeIdentity)
                .HasForeignKey(d => d.IdentityCategoryDocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeIdentity_Document");
        });

        modelBuilder.Entity<EmployeeImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07C583D80F");

            entity.ToTable("EmployeeImage", "axionpro");

            entity.Property(e => e.FileName).HasMaxLength(50);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeImage)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EmployeeImages_Employee");
        });

        modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07B6CC8062");

            entity.ToTable("EmployeeLeaveBalance", "axionpro");

            entity.Property(e => e.Availed).HasPrecision(5, 2);
            entity.Property(e => e.CarryForwarded).HasPrecision(5, 2);
            entity.Property(e => e.CurrentBalance)
                .HasPrecision(5, 2)
                .HasComputedColumnSql("(\"OpeningBalance\" - \"Availed\")", true);
            entity.Property(e => e.Encashed).HasPrecision(5, 2);
            entity.Property(e => e.LeavesOnHold)
                .HasPrecision(5, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.OpeningBalance).HasPrecision(5, 2);

            entity.HasOne(d => d.EmployeeLeavePolicyMapping).WithMany(p => p.EmployeeLeaveBalance)
                .HasForeignKey(d => d.EmployeeLeavePolicyMappingId)
                .HasConstraintName("FK_EmployeeLeaveBalance_EmployeeLeavePolicyMapping");

            entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeLeaveBalance)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EmployeeLeaveBalance_Tenant");
        });

        modelBuilder.Entity<EmployeeLeavePolicyMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07305828A7");

            entity.ToTable("EmployeeLeavePolicyMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(250);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeavePolicyMapping)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Emplo__73D00A73");

            entity.HasOne(d => d.PolicyLeaveTypeMapping).WithMany(p => p.EmployeeLeavePolicyMapping)
                .HasForeignKey(d => d.PolicyLeaveTypeMappingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Leave__74C42EAC");

            entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeLeavePolicyMapping)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Tenan__72DBE63A");
        });

        modelBuilder.Entity<EmployeeManagerMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC075BB6D3FC");

            entity.ToTable("EmployeeManagerMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(250);

            entity.HasOne(d => d.Department).WithMany(p => p.EmployeeManagerMapping)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_EmployeeManagerMapping_Department");

            entity.HasOne(d => d.Designation).WithMany(p => p.EmployeeManagerMapping)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK_EmployeeManagerMapping_Designation");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeManagerMappingEmployee)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeManagerMapping_Employee");

            entity.HasOne(d => d.Manager).WithMany(p => p.EmployeeManagerMappingManager)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeManagerMapping_Manager");

            entity.HasOne(d => d.ReportingType).WithMany(p => p.EmployeeManagerMapping)
                .HasForeignKey(d => d.ReportingTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeManagerMapping_ReportingType");

            entity.HasOne(d => d.Tenant).WithMany(p => p.EmployeeManagerMapping)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeManagerMapping_Tenant");
        });

        modelBuilder.Entity<EmployeePersonalDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC074796302D");

            entity.ToTable("EmployeePersonalDetail", "axionpro");

            entity.Property(e => e.AadhaarDocName).HasMaxLength(100);
            entity.Property(e => e.AadhaarDocPath).HasMaxLength(500);
            entity.Property(e => e.AadhaarNumber).HasMaxLength(20);
            entity.Property(e => e.BloodGroup).HasMaxLength(10);
            entity.Property(e => e.DrivingLicenseNumber).HasMaxLength(20);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(15);
            entity.Property(e => e.EmergencyContactRelation).HasMaxLength(50);
            entity.Property(e => e.HasEpfaccount).HasColumnName("HasEPFAccount");
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.PanDocName).HasMaxLength(100);
            entity.Property(e => e.PanDocPath).HasMaxLength(500);
            entity.Property(e => e.PanNumber).HasMaxLength(20);
            entity.Property(e => e.PassportDocName).HasMaxLength(100);
            entity.Property(e => e.PassportDocPath).HasMaxLength(500);
            entity.Property(e => e.PassportNumber).HasMaxLength(20);
            entity.Property(e => e.Uannumber)
                .HasMaxLength(20)
                .HasColumnName("UANNumber");
            entity.Property(e => e.VoterId).HasMaxLength(20);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePersonalDetail)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePersonalDetail_Employee");
        });

        modelBuilder.Entity<EmployeePolicyDependentMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EmployeePolicyDependentMapping_pkey");

            entity.ToTable("EmployeePolicyDependentMapping", "axionpro");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsCovered).HasDefaultValue(true);

            entity.HasOne(d => d.Dependent).WithMany(p => p.EmployeePolicyDependentMapping)
                .HasForeignKey(d => d.DependentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EPD_Dependent");

            entity.HasOne(d => d.EmployeePolicyEnrollment).WithMany(p => p.EmployeePolicyDependentMapping)
                .HasForeignKey(d => d.EmployeePolicyEnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EPD_Enrollment");
        });

        modelBuilder.Entity<EmployeePolicyEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EmployeePolicyEnrollment_pkey");

            entity.ToTable("EmployeePolicyEnrollment", "axionpro");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePolicyEnrollment)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePolicyEnrollment_Employee");

            entity.HasOne(d => d.InsurancePolicy).WithMany(p => p.EmployeePolicyEnrollment)
                .HasForeignKey(d => d.InsurancePolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePolicyEnrollment_InsurancePolicy");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.EmployeePolicyEnrollment)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePolicyEnrollment_PolicyType");
        });

        modelBuilder.Entity<EmployeeStatutoryAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07ACDD4E51");

            entity.ToTable("EmployeeStatutoryAccount", "axionpro");

            entity.Property(e => e.AccountNumber).HasMaxLength(100);
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.EmployerCode).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.StatutoryType).WithMany(p => p.EmployeeStatutoryAccount)
                .HasForeignKey(d => d.StatutoryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeStatutoryAccount_Statutory");
        });

        modelBuilder.Entity<EmployeeType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0760C5ED38");

            entity.ToTable("EmployeeType", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(255);
        });

        modelBuilder.Entity<EmployeeTypeBasicMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07A773CB3F");

            entity.ToTable("EmployeeTypeBasicMenu", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMenuDisplayInUi).HasColumnName("IsMenuDisplayInUI");

            entity.HasOne(d => d.BasicMenu).WithMany(p => p.EmployeeTypeBasicMenu)
                .HasForeignKey(d => d.BasicMenuId)
                .HasConstraintName("FK_EmployeeTypeBasicMenu_BasicMenu");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.EmployeeTypeBasicMenu)
                .HasForeignKey(d => d.EmployeeTypeId)
                .HasConstraintName("FK_EmployeeTypeBasicMenu_EmployeeType");
        });

        modelBuilder.Entity<EmployeeWorkDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07BDFE60B6");

            entity.ToTable("EmployeeWorkDocument", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.FileName).HasMaxLength(250);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.EmployeeWorkHistory).WithMany(p => p.EmployeeWorkDocument)
                .HasForeignKey(d => d.EmployeeWorkHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeWorkDocument_WorkHistory");

            entity.HasOne(d => d.WorkDocumentType).WithMany(p => p.EmployeeWorkDocument)
                .HasForeignKey(d => d.WorkDocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeWorkDocument_DocumentType");
        });

        modelBuilder.Entity<EmployeeWorkHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07994D46FA");

            entity.ToTable("EmployeeWorkHistory", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Ctc)
                .HasPrecision(18, 2)
                .HasColumnName("CTC");
            entity.Property(e => e.Designation).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEditAllowed).HasDefaultValue(true);
            entity.Property(e => e.IsWfh).HasColumnName("IsWFH");
            entity.Property(e => e.ReasonForLeaving).HasMaxLength(500);
            entity.Property(e => e.ReportingManagerName).HasMaxLength(150);
            entity.Property(e => e.ReportingManagerNumber).HasMaxLength(20);
            entity.Property(e => e.VerificationEmail).HasMaxLength(150);
            entity.Property(e => e.VerificationMode).HasMaxLength(50);

            entity.HasOne(d => d.EmployeeWorkProfile).WithMany(p => p.EmployeeWorkHistory)
                .HasForeignKey(d => d.EmployeeWorkProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeWorkHistory_Profile");
        });

        modelBuilder.Entity<EmployeeWorkProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07605A7EB3");

            entity.ToTable("EmployeeWorkProfile", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("timezone('utc'::text, now())");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEditAllowed).HasDefaultValue(true);
        });

        modelBuilder.Entity<EmployeesChangedTypeHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0779153EAD");

            entity.ToTable("EmployeesChangedTypeHistory", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ChangeDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeesChangedTypeHistory)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeStatusHistory_Employee");

            entity.HasOne(d => d.NewEmployeeType).WithMany(p => p.EmployeesChangedTypeHistoryNewEmployeeType)
                .HasForeignKey(d => d.NewEmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeStatusHistory_NewEmployeeType");

            entity.HasOne(d => d.OldEmployeeType).WithMany(p => p.EmployeesChangedTypeHistoryOldEmployeeType)
                .HasForeignKey(d => d.OldEmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeStatusHistory_OldEmployeeType");
        });

        modelBuilder.Entity<ForgotPasswordOtpdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ForgotPa__3214EC071E853072");

            entity.ToTable("ForgotPasswordOTPDetail", "axionpro");

            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.Otp)
                .HasMaxLength(10)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpexpireDateTime).HasColumnName("OTPExpireDateTime");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gender__3214EC07EF3CD03D");

            entity.ToTable("Gender", "axionpro");

            entity.HasIndex(e => e.GenderName, "UQ__Gender__F7C177153AF55502").IsUnique();

            entity.Property(e => e.GenderName).HasMaxLength(50);
        });

        modelBuilder.Entity<HolidayMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HolidayM__3214EC0743457A80");

            entity.ToTable("HolidayMaster", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.HolidayName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(250);
        });

        modelBuilder.Entity<IdentityCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Identity__3214EC07393AF557");

            entity.ToTable("IdentityCategory", "axionpro");

            entity.HasIndex(e => e.Code, "UQ__Identity__A25C5AA7EC142E9A").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<IdentityCategoryDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Identity__3214EC07AA61914F");

            entity.ToTable("IdentityCategoryDocument", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.DocumentName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsUnique).HasDefaultValue(true);

            entity.HasOne(d => d.IdentityCategory).WithMany(p => p.IdentityCategoryDocument)
                .HasForeignKey(d => d.IdentityCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IdentityDocument_Category");
        });

        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PolicyMa__3214EC07A7865FF0");

            entity.ToTable("InsurancePolicy", "axionpro");

            entity.Property(e => e.AgentContactNumber).HasMaxLength(20);
            entity.Property(e => e.AgentName).HasMaxLength(150);
            entity.Property(e => e.AgentOfficeNumber).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EmployeeAllowed).HasDefaultValue(true);
            entity.Property(e => e.InsurancePolicyName).HasMaxLength(200);
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProviderName).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.Country).WithMany(p => p.InsurancePolicy)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_InsurancePolicy_Country");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.InsurancePolicy)
                .HasForeignKey(d => d.PolicyTypeId)
                .HasConstraintName("FK_InsurancePolicy_PolicyType");
        });

        modelBuilder.Entity<InsurancePolicyDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Insuranc__3214EC07B3D0A432");

            entity.ToTable("InsurancePolicyDocument", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DocumentType).HasMaxLength(20);
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LanguageCode).HasMaxLength(10);

            entity.HasOne(d => d.InsurancePolicy).WithMany(p => p.InsurancePolicyDocument)
                .HasForeignKey(d => d.InsurancePolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsurancePolicyDocument_InsurancePolicy");
        });

        modelBuilder.Entity<InterviewFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07C20637CB");

            entity.ToTable("InterviewFeedback", "axionpro");

            entity.Property(e => e.Rating).HasPrecision(3, 1);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Candidate).WithMany(p => p.InterviewFeedback)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Interview__Candi__2AD55B43");

            entity.HasOne(d => d.InterviewSchedule).WithMany(p => p.InterviewFeedback)
                .HasForeignKey(d => d.InterviewScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Interview__Inter__2BC97F7C");
        });

        modelBuilder.Entity<InterviewPanel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07076FDC5F");

            entity.ToTable("InterviewPanel", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PanelName).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(255);
        });

        modelBuilder.Entity<InterviewPanelMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07205CE980");

            entity.ToTable("InterviewPanelMember", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remarks).HasMaxLength(255);

            entity.HasOne(d => d.Panel).WithMany(p => p.InterviewPanelMember)
                .HasForeignKey(d => d.PanelId)
                .HasConstraintName("FK_InterviewPanelMember_Panel");

            entity.HasOne(d => d.UserRole).WithMany(p => p.InterviewPanelMember)
                .HasForeignKey(d => d.UserRoleId)
                .HasConstraintName("FK_InterviewPanelMember_UserRole");
        });

        modelBuilder.Entity<InterviewSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC073C91635C");

            entity.ToTable("InterviewSchedule", "axionpro");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remarks).HasMaxLength(255);

            entity.HasOne(d => d.Candidate).WithMany(p => p.InterviewSchedule)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_InterviewSchedule_Candidate");

            entity.HasOne(d => d.Panel).WithMany(p => p.InterviewSchedule)
                .HasForeignKey(d => d.PanelId)
                .HasConstraintName("FK_InterviewSchedule_Panel");
        });

        modelBuilder.Entity<InterviewSdule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC0702C95E26");

            entity.ToTable("InterviewSdule", "axionpro");

            entity.Property(e => e.InterviewMode).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveReq__3214EC07D95D9BAD");

            entity.ToTable("LeaveRequest", "axionpro");

            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsHalfDay).HasDefaultValue(false);
            entity.Property(e => e.IsSandwich).HasDefaultValue(false);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Remark).HasMaxLength(100);
            entity.Property(e => e.TotalLeaveDays).HasPrecision(5, 2);

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequest)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Emplo__05EEBAAE");

            entity.HasOne(d => d.LeavePolicy).WithMany(p => p.LeaveRequest)
                .HasForeignKey(d => d.LeavePolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Leave__07D70320");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveRequest)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Leave__06E2DEE7");

            entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveRequest)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Tenan__04FA9675");
        });

        modelBuilder.Entity<LeaveRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveRul__3214EC07C443E6FB");

            entity.ToTable("LeaveRule", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsHalfDayAllowed).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.PolicyLeaveType).WithMany(p => p.LeaveRule)
                .HasForeignKey(d => d.PolicyLeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRule_PolicyLeaveType");

            entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveRule)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRule_Tenant");
        });

        modelBuilder.Entity<LeaveSandwichRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveSan__3214EC0757F4038B");

            entity.ToTable("LeaveSandwichRule", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(250);
            entity.Property(e => e.RuleName).HasMaxLength(100);
        });

        modelBuilder.Entity<LeaveSandwichRuleMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveSan__3214EC07EB5FE9CE");

            entity.ToTable("LeaveSandwichRuleMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(250);

            entity.HasOne(d => d.DayCombination).WithMany(p => p.LeaveSandwichRuleMapping)
                .HasForeignKey(d => d.DayCombinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DayCombination");

            entity.HasOne(d => d.LeaveRule).WithMany(p => p.LeaveSandwichRuleMapping)
                .HasForeignKey(d => d.LeaveRuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRule");

            entity.HasOne(d => d.LeaveSandwichRule).WithMany(p => p.LeaveSandwichRuleMapping)
                .HasForeignKey(d => d.LeaveSandwichRuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SandwichRule");
        });

        modelBuilder.Entity<LeaveTransactionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveTra__3214EC0737FE86DD");

            entity.ToTable("LeaveTransactionLog", "axionpro");

            entity.Property(e => e.LeaveDays).HasPrecision(5, 2);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.TransactionType).HasMaxLength(20);
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveTyp__3214EC0788A5EF9C");

            entity.ToTable("LeaveType", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveName).HasMaxLength(50);

            entity.HasOne(d => d.Tenant).WithMany(p => p.LeaveType)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_LeaveType_TenantId");
        });

        //modelBuilder.Entity<License>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK__License__3214EC07FF575687");

        //    entity.ToTable("License", "axionpro");

        //    entity.Property(e => e.IsActive).HasDefaultValue(true);
        //});

        modelBuilder.Entity<LoginCredential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoginCre__3214EC0750429DFA");

            entity.ToTable("LoginCredential", "axionpro");

            entity.HasIndex(e => e.LoginId, "UQ_LoginId").IsUnique();

            entity.Property(e => e.AddedById).HasDefaultValue(0L);
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
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
            entity.Property(e => e.Password).HasMaxLength(550);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.SoftDeletedById).HasDefaultValue(0L);
            entity.Property(e => e.UpdatedById).HasDefaultValue(0L);

            entity.HasOne(d => d.Employee).WithMany(p => p.LoginCredential)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoginCredential_Employee");
        });

        modelBuilder.Entity<MealAllowancePolicyByDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MealAllo__3214EC07BB4E4E52");

            entity.ToTable("MealAllowancePolicyByDesignation", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.BreakfastAllowance)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.DinnerAllowance)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.FixedFoodAllowance)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMetro).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
            entity.Property(e => e.LunchAllowance)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.MetroBonus)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.MinDaysRequired).HasDefaultValue(0);

            entity.HasOne(d => d.Designation).WithMany(p => p.MealAllowancePolicyByDesignation)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MealAllow__Desig__39237A9A");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.MealAllowancePolicyByDesignation)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MealAllow__Emplo__3A179ED3");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.MealAllowancePolicyByDesignation)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MealAllow__Polic__3B0BC30C");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Module__3214EC078891AB2D");

            entity.ToTable("Module", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.ImageIconMobile).HasMaxLength(255);
            entity.Property(e => e.ImageIconWeb).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsModuleDisplayInUi)
                .HasDefaultValue(true)
                .HasColumnName("IsModuleDisplayInUI");
            entity.Property(e => e.ItemPriority).HasDefaultValue(0);
            entity.Property(e => e.ModuleCode).HasMaxLength(50);
            entity.Property(e => e.ModuleName).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.Urlpath)
                .HasMaxLength(500)
                .HasColumnName("URLPath");

            entity.HasOne(d => d.ParentModule).WithMany(p => p.InverseParentModule)
                .HasForeignKey(d => d.ParentModuleId)
                .HasConstraintName("FK_Module_ParentModule");
        });

        modelBuilder.Entity<ModuleOperationMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ModuleOp__3214EC07BF86A196");

            entity.ToTable("ModuleOperationMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(255)
                .HasColumnName("IconURL");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsCommonItem).HasDefaultValue(false);
            entity.Property(e => e.IsOperational).HasDefaultValue(true);
            entity.Property(e => e.PageUrl)
                .HasMaxLength(255)
                .HasColumnName("PageURL");
            entity.Property(e => e.Priority).HasDefaultValue(0);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.DataViewStructure).WithMany(p => p.ModuleOperationMapping)
                .HasForeignKey(d => d.DataViewStructureId)
                .HasConstraintName("FK_ModuleOperationMapping_DataViewStructure");

            entity.HasOne(d => d.Module).WithMany(p => p.ModuleOperationMapping)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_ModuleOperationMapping_Module");

            entity.HasOne(d => d.Operation).WithMany(p => p.ModuleOperationMapping)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModuleOperation_Operation");

            entity.HasOne(d => d.PageType).WithMany(p => p.ModuleOperationMapping)
                .HasForeignKey(d => d.PageTypeId)
                .HasConstraintName("FK_ModuleOperationMapping_PageTypeEnum");
        });

        modelBuilder.Entity<NoImagePath>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NoImageP__3214EC07B56BC19F");

            entity.ToTable("NoImagePath", "axionpro");

            entity.Property(e => e.DefaultImagePath).HasMaxLength(500);
            entity.Property(e => e.ImageName).HasMaxLength(50);
            entity.Property(e => e.ImageType).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Operatio__3214EC079C437610");

            entity.ToTable("Operation", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IconImage).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OperationName).HasMaxLength(200);
            entity.Property(e => e.Remark).HasMaxLength(200);
        });

        modelBuilder.Entity<OrganizationHolidayCalendar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC077FBA239C");

            entity.ToTable("OrganizationHolidayCalendar", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.HolidayName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.StateCode).HasMaxLength(10);

            entity.HasOne(d => d.Tenant).WithMany(p => p.OrganizationHolidayCalendar)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_OrganizationHolidayCalendar_Tenant");
        });

        modelBuilder.Entity<PageTypeEnum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PageType__3214EC07D71C5D81");

            entity.ToTable("PageTypeEnum", "axionpro");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PageTypeName).HasMaxLength(20);
        });

        modelBuilder.Entity<PlanModuleMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanModu__3214EC0729948732");

            entity.ToTable("PlanModuleMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.UpdatedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Module).WithMany(p => p.PlanModuleMapping)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanModuleMapping_Module");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.PlanModuleMapping)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PMM_SubscriptionPlan");
        });

        modelBuilder.Entity<PolicyLeaveTypeMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeavePol__3214EC07A76146FC");

            entity.ToTable("PolicyLeaveTypeMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProofDocumentType).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(250);

            entity.HasOne(d => d.ApplicableGender).WithMany(p => p.PolicyLeaveTypeMapping)
                .HasForeignKey(d => d.ApplicableGenderId)
                .HasConstraintName("FK_PolicyLeaveTypeMapping_Gender");
        });

        modelBuilder.Entity<PolicyType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PolicyTy__3214EC07AAE08A64");

            entity.ToTable("PolicyType", "axionpro");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
            entity.Property(e => e.PolicyName).HasMaxLength(255);

            entity.HasOne(d => d.Tenant).WithMany(p => p.PolicyType)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_PolicyType_Tenant");
        });

        modelBuilder.Entity<PolicyTypeDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CompanyP__3214EC07BADB592E");

            entity.ToTable("PolicyTypeDocument", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DocumentTitle).HasMaxLength(200);
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.PolicyType).WithMany(p => p.PolicyTypeDocument)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyPolicyDocument_PolicyType");
        });

        modelBuilder.Entity<PolicyTypeInsuranceMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PolicyTy__3214EC07E433096D");

            entity.ToTable("PolicyTypeInsuranceMapping", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.InsurancePolicy).WithMany(p => p.PolicyTypeInsuranceMapping)
                .HasForeignKey(d => d.InsurancePolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PTIM_InsurancePolicy");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.PolicyTypeInsuranceMapping)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PTIM_PolicyType");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0731D02168");

            entity.ToTable("RefreshToken", "axionpro");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            entity.Property(e => e.ExpiryDate).HasDefaultValueSql("(CURRENT_TIMESTAMP + '5 days'::interval)");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            entity.Property(e => e.LoginId).HasMaxLength(255);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.Login).WithMany(p => p.RefreshToken)
                .HasPrincipalKey(p => p.LoginId)
                .HasForeignKey(d => d.LoginId)
                .HasConstraintName("FK__RefreshTo__Login__7132C993");
        });

        modelBuilder.Entity<ReportingType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reportin__3214EC0773736C22");

            entity.ToTable("ReportingType", "axionpro");

            entity.HasIndex(e => e.TypeName, "UQ__Reportin__D4E7DFA8AC2E2E45").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestT__3214EC07E53AD996");

            entity.ToTable("RequestType", "axionpro");

            entity.HasIndex(e => new { e.TenantId, e.RequestTypeName }, "UQ_RequestType_Tenant").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.RequestTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RequestType)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_RequestType_Tenant");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0723371271");

            entity.ToTable("Role", "axionpro");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsSystemDefault).HasDefaultValue(false);
            entity.Property(e => e.Remark)
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<RoleModuleAndPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RoleModuleAndPermission_Id");

            entity.ToTable("RoleModuleAndPermission", "axionpro");

            entity.HasIndex(e => e.OperationId, "IDX_RolesPermission_OperationId");

            entity.HasIndex(e => e.RoleId, "IDX_RolesPermission_RoleId");

            entity.Property(e => e.ImageIcon).HasMaxLength(200);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.Module).WithMany(p => p.RoleModuleAndPermission)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_RoleModulePermission_Module");

            entity.HasOne(d => d.Operation).WithMany(p => p.RoleModuleAndPermission)
                .HasForeignKey(d => d.OperationId)
                .HasConstraintName("FK_RoleModulePermission_Operation");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleModuleAndPermission)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RoleModulePermission_Role");
        });

        modelBuilder.Entity<ServiceProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC07B0695346");

            entity.ToTable("ServiceProvider", "axionpro");

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

            entity.ToTable("State", "axionpro");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StateName).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.State)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__State__CountryId__6B0FDBE9");
        });

        modelBuilder.Entity<StatutoryType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Statutor__3214EC078EED6601");

            entity.ToTable("StatutoryType", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Country).WithMany(p => p.StatutoryType)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatutoryType_Country");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07BED0F0D0");

            entity.ToTable("SubscriptionPlan", "axionpro");

            entity.Property(e => e.CurrencyKey).HasMaxLength(20);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.PerDayPrice).HasPrecision(10, 2);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.YearlyPrice).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tenant__3214EC0728DD7C6E");

            entity.ToTable("Tenant", "axionpro");

            entity.HasIndex(e => e.TenantEmail, "UQ__Tenant__F7C944DD7E3D53D9").IsUnique();

            entity.Property(e => e.CompanyEmailDomain).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.ContactPersonName).HasMaxLength(100);
            entity.Property(e => e.TenantCode).HasMaxLength(100);
            entity.Property(e => e.TenantEmail).HasMaxLength(200);

            entity.HasOne(d => d.Gender).WithMany(p => p.Tenant)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Gender_Tenant");

            entity.HasOne(d => d.TenantIndustry).WithMany(p => p.Tenant)
                .HasForeignKey(d => d.TenantIndustryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tenant_TenantIndustry");
        });

        modelBuilder.Entity<TenantEmailConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantEm__3214EC077A90B3A3");

            entity.ToTable("TenantEmailConfig", "axionpro");

            entity.Property(e => e.FromEmail).HasMaxLength(200);
            entity.Property(e => e.FromName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SecrateKey)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.SmtpHost).HasMaxLength(200);
            entity.Property(e => e.SmtpPasswordEncrypted).HasMaxLength(500);
            entity.Property(e => e.SmtpUsername).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEmailConfig)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_TenantEmailConfig_Tenant");
        });

        modelBuilder.Entity<TenantEnabledModule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC07AC610092");

            entity.ToTable("TenantEnabledModule", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);

            entity.HasOne(d => d.Module).WithMany(p => p.TenantEnabledModule)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantEnabledModules_Module");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEnabledModule)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantEnabledModules_Tenant");
        });

        modelBuilder.Entity<TenantEnabledOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC07047C5B46");

            entity.ToTable("TenantEnabledOperation", "axionpro");

            entity.HasIndex(e => new { e.TenantId, e.ModuleId, e.OperationId }, "UQ_Tenant_Module_Operation").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);

            entity.HasOne(d => d.Module).WithMany(p => p.TenantEnabledOperation)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantEnabledOperations_Module");

            entity.HasOne(d => d.Operation).WithMany(p => p.TenantEnabledOperation)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantEnabledOperations_Operation");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantEnabledOperation)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantEnabledOperations_Tenant");
        });

        modelBuilder.Entity<TenantEncryptionKeys>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantEn__3214EC070E3540EE");

            entity.ToTable("TenantEncryptionKeys", "axionpro");

            entity.HasIndex(e => e.TenantId, "UQ_TenantKey_TenantId").IsUnique();

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EncryptionKey).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TenantIndustry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantIn__3214EC0765258C5E");

            entity.ToTable("TenantIndustry", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IndustryName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(255);
        });

        modelBuilder.Entity<TenantProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantPr__3214EC0796A1B93D");

            entity.ToTable("TenantProfile", "axionpro");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.BusinessType).HasMaxLength(100);
            entity.Property(e => e.Industry).HasMaxLength(100);
            entity.Property(e => e.LogoUrl).HasMaxLength(255);
            entity.Property(e => e.ThemeColor).HasMaxLength(50);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantProfile)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_TenantProfile_Tenant");
        });

        modelBuilder.Entity<TenantSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantSu__3214EC07CF143048");

            entity.ToTable("TenantSubscription", "axionpro");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentMode).HasMaxLength(50);
            entity.Property(e => e.PaymentTxnId).HasMaxLength(100);
            entity.Property(e => e.SubscriptionStartDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.TenantSubscription)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantSubscription_Plan");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantSubscription)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantSubscription_Tenant");
        });

        modelBuilder.Entity<Tender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tender__3214EC076E847502");

            entity.ToTable("Tender", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.TenderName).HasMaxLength(255);
            entity.Property(e => e.TenderValue).HasPrecision(18, 2);

            entity.HasOne(d => d.Client).WithMany(p => p.Tender)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tender_ClientId");

            entity.HasOne(d => d.TenderStatus).WithMany(p => p.Tender)
                .HasForeignKey(d => d.TenderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tender__TenderSt__3EDC53F0");
        });

        modelBuilder.Entity<TenderProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderPr__3214EC073A6AC46E");

            entity.ToTable("TenderProject", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EstimatedBudget).HasPrecision(18, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(1000);
            entity.Property(e => e.StartDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Status).WithMany(p => p.TenderProject)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderPro__Statu__40C49C62");

            entity.HasOne(d => d.TenderServiceProvider).WithMany(p => p.TenderProject)
                .HasForeignKey(d => d.TenderServiceProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenderProject_TenderServiceProvider");

            entity.HasOne(d => d.UserRole).WithMany(p => p.TenderProject)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderPro__UserR__42ACE4D4");
        });

        modelBuilder.Entity<TenderService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07620086F4");

            entity.ToTable("TenderService", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.Tender).WithMany(p => p.TenderService)
                .HasForeignKey(d => d.TenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderSer__Tende__43A1090D");

            entity.HasOne(d => d.TenderServiceType).WithMany(p => p.TenderService)
                .HasForeignKey(d => d.TenderServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderSer__Tende__44952D46");
        });

        modelBuilder.Entity<TenderServiceHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07F4765A9B");

            entity.ToTable("TenderServiceHistory", "axionpro");

            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(100);
        });

        modelBuilder.Entity<TenderServiceProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC0765F55078");

            entity.ToTable("TenderServiceProvider", "axionpro");

            entity.Property(e => e.ContractAmount).HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsInHouse).HasDefaultValue(false);
            entity.Property(e => e.IsPrimaryProvider).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(500);

            entity.HasOne(d => d.Status).WithMany(p => p.TenderServiceProvider)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderSer__Statu__4589517F");

            entity.HasOne(d => d.TenderServiceSpecification).WithMany(p => p.TenderServiceProvider)
                .HasForeignKey(d => d.TenderServiceSpecificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenderServiceProvider_TenderServiceSpecification");

            entity.HasOne(d => d.TenderServiceSpecificationNavigation).WithMany(p => p.TenderServiceProvider)
                .HasForeignKey(d => d.TenderServiceSpecificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenderSer__Tende__467D75B8");
        });

        modelBuilder.Entity<TenderServiceSpecification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC07FBF10311");

            entity.ToTable("TenderServiceSpecification", "axionpro");

            entity.Property(e => e.EstimatedBudget).HasPrecision(18, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProductPlatform).HasMaxLength(255);
            entity.Property(e => e.ProductSpecification).HasMaxLength(1000);
            entity.Property(e => e.SpecificationName).HasMaxLength(255);
            entity.Property(e => e.SpecificationType).HasMaxLength(50);

            entity.HasOne(d => d.TenderService).WithMany(p => p.TenderServiceSpecification)
                .HasForeignKey(d => d.TenderServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenderServiceSpecification_TenderService");
        });

        modelBuilder.Entity<TenderServiceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenderSe__3214EC072E505507");

            entity.ToTable("TenderServiceType", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<TenderStatus>(entity =>
        {
            entity.ToTable("TenderStatus", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity <axionpro.domain.Entity.Thread>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Thread_pkey");

            entity.ToTable("Thread", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Thread_Id_seq\"'::regclass)");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<ThreadMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ThreadMessage_pkey");

            entity.ToTable("ThreadMessage", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"ThreadMessage_Id_seq\"'::regclass)");
            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.MessageType).HasDefaultValue(1);

            entity.HasOne(d => d.AddedBy).WithMany(p => p.ThreadMessage)
                .HasForeignKey(d => d.AddedById)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ThreadMessage_Employee");

            entity.HasOne(d => d.Thread).WithMany(p => p.ThreadMessage)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("FK_ThreadMessage_Thread");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ticket_pkey");

            entity.ToTable("Ticket", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Ticket_Id_seq\"'::regclass)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.TicketNumber).HasMaxLength(50);

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.TicketAssignedToUser)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Ticket_AssignedUser");

            entity.HasOne(d => d.RecommendedByUser).WithMany(p => p.TicketRecommendedByUser)
                .HasForeignKey(d => d.RecommendedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Ticket_RecommendedBy");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.TicketRequestedByUser)
                .HasForeignKey(d => d.RequestedByUserId)
                .HasConstraintName("FK_Ticket_RequestedBy");

            entity.HasOne(d => d.RequestedForUser).WithMany(p => p.TicketRequestedForUser)
                .HasForeignKey(d => d.RequestedForUserId)
                .HasConstraintName("FK_Ticket_RequestedFor");

            entity.HasOne(d => d.TicketClassification).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.TicketClassificationId)
                .HasConstraintName("FK_Ticket_Classification");

            entity.HasOne(d => d.TicketHeader).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.TicketHeaderId)
                .HasConstraintName("FK_Ticket_Header");

            entity.HasOne(d => d.TicketType).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.TicketTypeId)
                .HasConstraintName("FK_Ticket_Type");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TicketAttachment_pkey");

            entity.ToTable("TicketAttachment", "axionpro");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"TicketAttachment_Id_seq\"'::regclass)");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.UploadedDateTime).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketAttachment)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_TicketAttachment_Ticket");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.TicketAttachment)
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TicketAttachment_User");
        });

        modelBuilder.Entity<TicketClassification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketCl__3214EC07BA509F8F");

            entity.ToTable("TicketClassification", "axionpro");

            entity.Property(e => e.ClassificationName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Tenant).WithMany(p => p.TicketClassification)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_TicketClassification_Tenant");
        });

        modelBuilder.Entity<TicketHeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketHe__3214EC07EADE3F1B");

            entity.ToTable("TicketHeader", "axionpro");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.HeaderName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Tenant).WithMany(p => p.TicketHeader)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_TicketHeader_TenantId");

            entity.HasOne(d => d.TicketClassification).WithMany(p => p.TicketHeader)
                .HasForeignKey(d => d.TicketClassificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketHeaderType_TicketClassification");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketTy__3214EC07554DC568");

            entity.ToTable("TicketType", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(true);
            entity.Property(e => e.TicketTypeName).HasMaxLength(100);

            entity.HasOne(d => d.ResponsibleRole).WithMany(p => p.TicketType)
                .HasForeignKey(d => d.ResponsibleRoleId)
                .HasConstraintName("FK_TicketType_ResponsibleRole");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TicketType)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_TicketType_Tenant");

            entity.HasOne(d => d.TicketHeader).WithMany(p => p.TicketType)
                .HasForeignKey(d => d.TicketHeaderId)
                .HasConstraintName("FK_TicketType_Header");
        });

        modelBuilder.Entity<TravelAllowancePolicyByDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TravelAl__3214EC072DD6C86A");

            entity.ToTable("TravelAllowancePolicyByDesignation", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AdvanceAllowed).HasDefaultValue(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMetro).HasDefaultValue(false);
            entity.Property(e => e.IsSoftDelete).HasDefaultValue(false);
            entity.Property(e => e.MaxAdvanceAmount)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.MetroBonus)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);
            entity.Property(e => e.ReimbursementPerKm)
                .HasPrecision(10, 2)
                .HasColumnName("ReimbursementPerKM");
            entity.Property(e => e.TravelClass).HasMaxLength(50);

            entity.HasOne(d => d.Designation).WithMany(p => p.TravelAllowancePolicyByDesignation)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TravelAll__Desig__4959E263");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.TravelAllowancePolicyByDesignation)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TravelAll__Emplo__4A4E069C");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.TravelAllowancePolicyByDesignation)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TravelAll__Polic__4B422AD5");
        });

        modelBuilder.Entity<TravelMode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TravelMo__3214EC075A97AC69");

            entity.ToTable("TravelMode", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TravelModeName).HasMaxLength(255);
        });

        modelBuilder.Entity<UnStructuredPolicyTypeMappingWithEmployeeType>(entity =>
        {
            entity.ToTable("UnStructuredPolicyTypeMappingWithEmployeeType", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.UnStructuredPolicyTypeMappingWithEmployeeType)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnStructuredPolicyTypeMappingWithEmployeeType_EmployeeType");

            entity.HasOne(d => d.PolicyType).WithMany(p => p.UnStructuredPolicyTypeMappingWithEmployeeType)
                .HasForeignKey(d => d.PolicyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnStructuredPolicyTypeMappingWithEmployeeType_PolicyType");
        });

        modelBuilder.Entity<UserAttendanceSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAtte__3214EC0750CE7646");

            entity.ToTable("UserAttendanceSetting", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.GeofenceLatitude).HasPrecision(10, 8);
            entity.Property(e => e.GeofenceLongitude).HasPrecision(10, 8);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAllowed).HasDefaultValue(true);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.AttendanceDeviceType).WithMany(p => p.UserAttendanceSetting)
                .HasForeignKey(d => d.AttendanceDeviceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAttendanceSetting_AttendanceDeviceType");

            entity.HasOne(d => d.Employee).WithMany(p => p.UserAttendanceSetting)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_UserAttendanceSetting_Employee");

            entity.HasOne(d => d.WorkstationType).WithMany(p => p.UserAttendanceSetting)
                .HasForeignKey(d => d.WorkstationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAttendanceSetting_WorkstationType");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07C9E25EB9");

            entity.ToTable("UserRole", "axionpro");

            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.Employee).WithMany(p => p.UserRole)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Role_Employee");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRole)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRole_Role");
        });

        modelBuilder.Entity<WorkDocumentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkDocu__3214EC0710449AA0");

            entity.ToTable("WorkDocumentType", "axionpro");

            entity.Property(e => e.DocumentTypeName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<WorkflowStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC07428E99CC");

            entity.ToTable("WorkflowStage", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ColorKey).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StageName).HasMaxLength(100);
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC07E62FF378");

            entity.ToTable("WorkflowStep", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
            entity.Property(e => e.IsSoftDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remark).HasMaxLength(250);
        });

        modelBuilder.Entity<WorkstationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorkstationMode");

            entity.ToTable("WorkstationType", "axionpro");

            entity.Property(e => e.AddedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Workstation)
                .HasMaxLength(20)
                .IsFixedLength();
        });

       
    }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }



}
