



using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Data.Context
{
    public partial class WorkforcedbContext : DbContext
    {
        public WorkforcedbContext()
        {
        }

        public WorkforcedbContext(DbContextOptions<WorkforcedbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accoumndationallowancepolicybydesignation> Accoumndationallowancepolicybydesignations { get; set; }

        public virtual DbSet<Approvalworkflow> Approvalworkflows { get; set; }

        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<Assetassignment> Assetassignments { get; set; }

        public virtual DbSet<Assetcategory> Assetcategories { get; set; }

        public virtual DbSet<Assethistory> Assethistories { get; set; }

        public virtual DbSet<Assetimage> Assetimages { get; set; }

        public virtual DbSet<Assetstatus> Assetstatuses { get; set; }

        public virtual DbSet<Assettickettypedetail> Assettickettypedetails { get; set; }

        public virtual DbSet<Assettype> Assettypes { get; set; }

        public virtual DbSet<Assignmentstatus> Assignmentstatuses { get; set; }

        public virtual DbSet<Attendance> Attendances { get; set; }

        public virtual DbSet<Attendancedevicetype> Attendancedevicetypes { get; set; }

        public virtual DbSet<Attendancehistory> Attendancehistories { get; set; }

        public virtual DbSet<Attendancerequest> Attendancerequests { get; set; }

        public virtual DbSet<Basicmenu> Basicmenus { get; set; }

        public virtual DbSet<Candidate> Candidates { get; set; }

        public virtual DbSet<Candidatecategoryskill> Candidatecategoryskills { get; set; }

        public virtual DbSet<Candidatehistory> Candidatehistories { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Clienttype> Clienttypes { get; set; }

        public virtual DbSet<Companypolicydocument> Companypolicydocuments { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Countryidentityrule> Countryidentityrules { get; set; }

        public virtual DbSet<Countrystatutoryrule> Countrystatutoryrules { get; set; }

        public virtual DbSet<Dataviewstructure> Dataviewstructures { get; set; }

        public virtual DbSet<Daycombination> Daycombinations { get; set; }

        public virtual DbSet<Demorequest> Demorequests { get; set; }

        public virtual DbSet<Demorequestbiometricdetail> Demorequestbiometricdetails { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Designation> Designations { get; set; }

        public virtual DbSet<District> Districts { get; set; }

        public virtual DbSet<Districtmaster> Districtmasters { get; set; }

        public virtual DbSet<Emailqueue> Emailqueues { get; set; }

        public virtual DbSet<Emailslog> Emailslogs { get; set; }

        public virtual DbSet<Emailtemplate> Emailtemplates { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<Employeebankdetail> Employeebankdetails { get; set; }

        public virtual DbSet<Employeecategoryskill> Employeecategoryskills { get; set; }

        public virtual DbSet<Employeecodepattern> Employeecodepatterns { get; set; }

        public virtual DbSet<Employeecontact> Employeecontacts { get; set; }

        public virtual DbSet<Employeedailyattendance> Employeedailyattendances { get; set; }

        public virtual DbSet<Employeedependent> Employeedependents { get; set; }

        public virtual DbSet<Employeeeducation> Employeeeducations { get; set; }

        public virtual DbSet<Employeeexperience> Employeeexperiences { get; set; }

        public virtual DbSet<Employeeexperiencedetail> Employeeexperiencedetails { get; set; }

        public virtual DbSet<Employeeexperiencepayslipupload> Employeeexperiencepayslipuploads { get; set; }

        public virtual DbSet<Employeeidentity> Employeeidentities { get; set; }

        public virtual DbSet<Employeeimage> Employeeimages { get; set; }

        public virtual DbSet<Employeeinsurancemapping> Employeeinsurancemappings { get; set; }

        public virtual DbSet<Employeeinsurancepolicy> Employeeinsurancepolicies { get; set; }

        public virtual DbSet<Employeeleavebalance> Employeeleavebalances { get; set; }

        public virtual DbSet<Employeeleavepolicymapping> Employeeleavepolicymappings { get; set; }

        public virtual DbSet<Employeemanagermapping> Employeemanagermappings { get; set; }

        public virtual DbSet<Employeepersonaldetail> Employeepersonaldetails { get; set; }

        public virtual DbSet<Employeeschangedtypehistory> Employeeschangedtypehistories { get; set; }

        public virtual DbSet<Employeestatutoryaccount> Employeestatutoryaccounts { get; set; }

        public virtual DbSet<Employeetype> Employeetypes { get; set; }

        public virtual DbSet<Employeetypebasicmenu> Employeetypebasicmenus { get; set; }

        public virtual DbSet<Employeeworkdocument> Employeeworkdocuments { get; set; }

        public virtual DbSet<Employeeworkhistory> Employeeworkhistories { get; set; }

        public virtual DbSet<Employeeworkprofile> Employeeworkprofiles { get; set; }

        public virtual DbSet<Forgotpasswordotpdetail> Forgotpasswordotpdetails { get; set; }

        public virtual DbSet<Gender> Genders { get; set; }

        public virtual DbSet<Holidaymaster> Holidaymasters { get; set; }

        public virtual DbSet<Identitycategory> Identitycategories { get; set; }

        public virtual DbSet<Identitycategorydocument> Identitycategorydocuments { get; set; }

        public virtual DbSet<Insurancepolicy> Insurancepolicies { get; set; }

        public virtual DbSet<Insurancepolicydocument> Insurancepolicydocuments { get; set; }

        public virtual DbSet<Interviewfeedback> Interviewfeedbacks { get; set; }

        public virtual DbSet<Interviewpanel> Interviewpanels { get; set; }

        public virtual DbSet<Interviewpanelmember> Interviewpanelmembers { get; set; }

        public virtual DbSet<Interviewschedule> Interviewschedules { get; set; }

        public virtual DbSet<Interviewsdule> Interviewsdules { get; set; }

        public virtual DbSet<Leaverequest> Leaverequests { get; set; }

        public virtual DbSet<Leaverule> Leaverules { get; set; }

        public virtual DbSet<Leavesandwichrule> Leavesandwichrules { get; set; }

        public virtual DbSet<Leavesandwichrulemapping> Leavesandwichrulemappings { get; set; }

        public virtual DbSet<Leavetransactionlog> Leavetransactionlogs { get; set; }

        public virtual DbSet<Leavetype> Leavetypes { get; set; }

        public virtual DbSet<License> Licenses { get; set; }

        public virtual DbSet<Logincredential> Logincredentials { get; set; }

        public virtual DbSet<Mealallowancepolicybydesignation> Mealallowancepolicybydesignations { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<Module1> Modules1 { get; set; }

        public virtual DbSet<Moduleoperationmapping> Moduleoperationmappings { get; set; }

        public virtual DbSet<Noimagepath> Noimagepaths { get; set; }

        public virtual DbSet<Operation> Operations { get; set; }

        public virtual DbSet<Organizationholidaycalendar> Organizationholidaycalendars { get; set; }

        public virtual DbSet<Pagetypeenum> Pagetypeenums { get; set; }

        public virtual DbSet<Planmodulemapping> Planmodulemappings { get; set; }

        public virtual DbSet<Policyleavetypemapping> Policyleavetypemappings { get; set; }

        public virtual DbSet<Policytype> Policytypes { get; set; }

        public virtual DbSet<Policytypeinsurancemapping> Policytypeinsurancemappings { get; set; }

        public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

        public virtual DbSet<Reportingtype> Reportingtypes { get; set; }

        public virtual DbSet<Requesttype> Requesttypes { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Role1> Roles1 { get; set; }

        public virtual DbSet<Rolemoduleandpermission> Rolemoduleandpermissions { get; set; }

        public virtual DbSet<Serviceprovider> Serviceproviders { get; set; }

        public virtual DbSet<State> States { get; set; }

        public virtual DbSet<Statutorytype> Statutorytypes { get; set; }

        public virtual DbSet<Subscriptionplan> Subscriptionplans { get; set; }

        public virtual DbSet<Tenant> Tenants { get; set; }

        public virtual DbSet<Tenantemailconfig> Tenantemailconfigs { get; set; }

        public virtual DbSet<Tenantenabledmodule> Tenantenabledmodules { get; set; }

        public virtual DbSet<Tenantenabledoperation> Tenantenabledoperations { get; set; }

        public virtual DbSet<Tenantencryptionkey> Tenantencryptionkeys { get; set; }

        public virtual DbSet<Tenantindustry> Tenantindustries { get; set; }

        public virtual DbSet<Tenantprofile> Tenantprofiles { get; set; }

        public virtual DbSet<Tenantsubscription> Tenantsubscriptions { get; set; }

        public virtual DbSet<Tender> Tenders { get; set; }

        public virtual DbSet<Tenderproject> Tenderprojects { get; set; }

        public virtual DbSet<Tenderservice> Tenderservices { get; set; }

        public virtual DbSet<Tenderservicehistory> Tenderservicehistories { get; set; }

        public virtual DbSet<Tenderserviceprovider> Tenderserviceproviders { get; set; }

        public virtual DbSet<Tenderservicespecification> Tenderservicespecifications { get; set; }

        public virtual DbSet<Tenderservicetype> Tenderservicetypes { get; set; }

        public virtual DbSet<Tenderstatus> Tenderstatuses { get; set; }

        public virtual DbSet<Ticketclassification> Ticketclassifications { get; set; }

        public virtual DbSet<Ticketheader> Ticketheaders { get; set; }

        public virtual DbSet<Tickettype> Tickettypes { get; set; }

        public virtual DbSet<Travelallowancepolicybydesignation> Travelallowancepolicybydesignations { get; set; }

        public virtual DbSet<Travelmode> Travelmodes { get; set; }

        public virtual DbSet<Userattendancesetting> Userattendancesettings { get; set; }

        public virtual DbSet<Userrole> Userroles { get; set; }

        public virtual DbSet<Workdocumenttype> Workdocumenttypes { get; set; }

        public virtual DbSet<Workflowstage> Workflowstages { get; set; }

        public virtual DbSet<Workflowstep> Workflowsteps { get; set; }

        public virtual DbSet<Workstationtype> Workstationtypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=workforcedb;Username=postgres;Password=1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accoumndationallowancepolicybydesignation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("accoumndationallowancepolicybydesignation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Designationid).HasColumnName("designationid");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Fixedstayallowance)
                    .HasPrecision(10, 2)
                    .HasColumnName("fixedstayallowance");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismetro).HasColumnName("ismetro");
                entity.Property(e => e.Issoftdelete).HasColumnName("issoftdelete");
                entity.Property(e => e.Metrobonus)
                    .HasPrecision(10, 2)
                    .HasColumnName("metrobonus");
                entity.Property(e => e.Mindaysrequired).HasColumnName("mindaysrequired");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Requireddocuments).HasColumnName("requireddocuments");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Approvalworkflow>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("approvalworkflow", "axionpro");

                entity.Property(e => e.Actionname)
                    .HasMaxLength(150)
                    .HasColumnName("actionname");
                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Projectchildmoduledetailid).HasColumnName("projectchildmoduledetailid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Workflowname)
                    .HasMaxLength(150)
                    .HasColumnName("workflowname");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("asset", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Assetname)
                    .HasMaxLength(100)
                    .HasColumnName("assetname");
                entity.Property(e => e.Assetstatusid).HasColumnName("assetstatusid");
                entity.Property(e => e.Assettypeid).HasColumnName("assettypeid");
                entity.Property(e => e.Barcode)
                    .HasMaxLength(100)
                    .HasColumnName("barcode");
                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .HasColumnName("color");
                entity.Property(e => e.Company)
                    .HasMaxLength(100)
                    .HasColumnName("company");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isassigned).HasColumnName("isassigned");
                entity.Property(e => e.Isrepairable).HasColumnName("isrepairable");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Modelno)
                    .HasMaxLength(50)
                    .HasColumnName("modelno");
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2)
                    .HasColumnName("price");
                entity.Property(e => e.Purchasedate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("purchasedate");
                entity.Property(e => e.Qrcode)
                    .HasMaxLength(250)
                    .HasColumnName("qrcode");
                entity.Property(e => e.Serialnumber)
                    .HasMaxLength(100)
                    .HasColumnName("serialnumber");
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Warrantyexpirydate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("warrantyexpirydate");
                entity.Property(e => e.Weight)
                    .HasMaxLength(50)
                    .HasColumnName("weight");
            });

            modelBuilder.Entity<Assetassignment>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assetassignment", "axionpro");

                entity.Property(e => e.Actualreturndate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("actualreturndate");
                entity.Property(e => e.Addedbydatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addedbydatetime");
                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Approvalid).HasColumnName("approvalid");
                entity.Property(e => e.Assetconditionatassign)
                    .HasMaxLength(255)
                    .HasColumnName("assetconditionatassign");
                entity.Property(e => e.Assetconditionatreturn)
                    .HasMaxLength(255)
                    .HasColumnName("assetconditionatreturn");
                entity.Property(e => e.Assetid).HasColumnName("assetid");
                entity.Property(e => e.Assigneddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("assigneddate");
                entity.Property(e => e.Assignmentstatusid).HasColumnName("assignmentstatusid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Expectedreturndate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("expectedreturndate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Identificationmethod)
                    .HasMaxLength(50)
                    .HasColumnName("identificationmethod");
                entity.Property(e => e.Identificationvalue)
                    .HasMaxLength(100)
                    .HasColumnName("identificationvalue");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isapproved).HasColumnName("isapproved");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatebyid).HasColumnName("updatebyid");
                entity.Property(e => e.Updatedbydatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updatedbydatetime");
            });

            modelBuilder.Entity<Assetcategory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assetcategory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Categoryname)
                    .HasMaxLength(200)
                    .HasColumnName("categoryname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assethistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assethistory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Assetconditionatassign)
                    .HasMaxLength(100)
                    .HasColumnName("assetconditionatassign");
                entity.Property(e => e.Assetconditionatreturn)
                    .HasMaxLength(100)
                    .HasColumnName("assetconditionatreturn");
                entity.Property(e => e.Assetid).HasColumnName("assetid");
                entity.Property(e => e.Assigneddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("assigneddate");
                entity.Property(e => e.Assignmentstatusid).HasColumnName("assignmentstatusid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Identificationmethod)
                    .HasMaxLength(50)
                    .HasColumnName("identificationmethod");
                entity.Property(e => e.Identificationvalue)
                    .HasMaxLength(255)
                    .HasColumnName("identificationvalue");
                entity.Property(e => e.Isscrapped).HasColumnName("isscrapped");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(500)
                    .HasColumnName("remarks");
                entity.Property(e => e.Returneddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("returneddate");
                entity.Property(e => e.Scrapapprovedby).HasColumnName("scrapapprovedby");
                entity.Property(e => e.Scrapdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("scrapdate");
                entity.Property(e => e.Scrapreason)
                    .HasMaxLength(255)
                    .HasColumnName("scrapreason");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatebyid).HasColumnName("updatebyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assetimage>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assetimage", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Assetid).HasColumnName("assetid");
                entity.Property(e => e.Assetimagepath)
                    .HasMaxLength(500)
                    .HasColumnName("assetimagepath");
                entity.Property(e => e.Assetimagetype).HasColumnName("assetimagetype");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isprimary).HasColumnName("isprimary");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assetstatus>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assetstatus", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Colorkey)
                    .HasMaxLength(50)
                    .HasColumnName("colorkey");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Statusname)
                    .HasMaxLength(50)
                    .HasColumnName("statusname");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assettickettypedetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assettickettypedetail", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Assettypeid).HasColumnName("assettypeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Responsibleroleid).HasColumnName("responsibleroleid");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeletedtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedtime");
                entity.Property(e => e.Tickettypeid).HasColumnName("tickettypeid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assettype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assettype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Assetcategoryid).HasColumnName("assetcategoryid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Typename)
                    .HasMaxLength(100)
                    .HasColumnName("typename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Assignmentstatus>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("assignmentstatus", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Statusname)
                    .HasMaxLength(50)
                    .HasColumnName("statusname");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("attendance", "axionpro");

                entity.Property(e => e.Attendancedate).HasColumnName("attendancedate");
                entity.Property(e => e.Attendanceimagepath)
                    .HasMaxLength(200)
                    .HasColumnName("attendanceimagepath");
                entity.Property(e => e.Attendanceimageurl)
                    .HasMaxLength(200)
                    .HasColumnName("attendanceimageurl");
                entity.Property(e => e.Deviceid).HasColumnName("deviceid");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
            });

            modelBuilder.Entity<Attendancedevicetype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("attendancedevicetype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Devicetype)
                    .HasMaxLength(50)
                    .HasColumnName("devicetype");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isdeviceregister).HasColumnName("isdeviceregister");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Attendancehistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("attendancehistory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Attendancedate).HasColumnName("attendancedate");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Intime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("intime");
                entity.Property(e => e.Outtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("outtime");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .HasColumnName("remarks");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
                entity.Property(e => e.Totalbreakhours)
                    .HasPrecision(5, 2)
                    .HasColumnName("totalbreakhours");
                entity.Property(e => e.Totalworkhours)
                    .HasPrecision(5, 2)
                    .HasColumnName("totalworkhours");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Attendancerequest>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("attendancerequest", "axionpro");

                entity.Property(e => e.Attendancedate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("attendancedate");
                entity.Property(e => e.Attendancedevicetypeid).HasColumnName("attendancedevicetypeid");
                entity.Property(e => e.Attendanceimagepath)
                    .HasMaxLength(200)
                    .HasColumnName("attendanceimagepath");
                entity.Property(e => e.Attendanceimageurl)
                    .HasMaxLength(200)
                    .HasColumnName("attendanceimageurl");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Workstationtypeid).HasColumnName("workstationtypeid");
            });

            modelBuilder.Entity<Basicmenu>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("basicmenu", "axionpro");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Imageicon)
                    .HasMaxLength(100)
                    .HasColumnName("imageicon");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Menuname)
                    .HasMaxLength(100)
                    .HasColumnName("menuname");
                entity.Property(e => e.Menuurl)
                    .HasMaxLength(255)
                    .HasColumnName("menuurl");
                entity.Property(e => e.Parentmenuid).HasColumnName("parentmenuid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("candidate", "axionpro");

                entity.Property(e => e.Aadhaar)
                    .HasMaxLength(12)
                    .HasColumnName("aadhaar");
                entity.Property(e => e.Actionstatus)
                    .HasMaxLength(20)
                    .HasColumnName("actionstatus");
                entity.Property(e => e.Applieddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("applieddate");
                entity.Property(e => e.Candidatereferencecode)
                    .HasMaxLength(20)
                    .HasColumnName("candidatereferencecode");
                entity.Property(e => e.Currentcompany)
                    .HasMaxLength(200)
                    .HasColumnName("currentcompany");
                entity.Property(e => e.Currentlocation)
                    .HasMaxLength(200)
                    .HasColumnName("currentlocation");
                entity.Property(e => e.Dateofbirth).HasColumnName("dateofbirth");
                entity.Property(e => e.Education)
                    .HasMaxLength(50)
                    .HasColumnName("education");
                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnName("email");
                entity.Property(e => e.Expectedsalary)
                    .HasPrecision(18, 2)
                    .HasColumnName("expectedsalary");
                entity.Property(e => e.Experienceyears)
                    .HasPrecision(4, 1)
                    .HasColumnName("experienceyears");
                entity.Property(e => e.Fewwords)
                    .HasMaxLength(1000)
                    .HasColumnName("fewwords");
                entity.Property(e => e.Firstname)
                    .HasMaxLength(100)
                    .HasColumnName("firstname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isblacklisted).HasColumnName("isblacklisted");
                entity.Property(e => e.Isfresher).HasColumnName("isfresher");
                entity.Property(e => e.Lastname)
                    .HasMaxLength(100)
                    .HasColumnName("lastname");
                entity.Property(e => e.Lastupdateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("lastupdateddatetime");
                entity.Property(e => e.Noticeperiod).HasColumnName("noticeperiod");
                entity.Property(e => e.Pan)
                    .HasMaxLength(10)
                    .HasColumnName("pan");
                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(50)
                    .HasColumnName("phonenumber");
                entity.Property(e => e.Resumepath)
                    .HasMaxLength(200)
                    .HasColumnName("resumepath");
                entity.Property(e => e.Resumeurl)
                    .HasMaxLength(200)
                    .HasColumnName("resumeurl");
                entity.Property(e => e.Skillset)
                    .HasColumnType("character varying")
                    .HasColumnName("skillset");
            });

            modelBuilder.Entity<Candidatecategoryskill>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("candidatecategoryskill", "axionpro");

                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Candidateid).HasColumnName("candidateid");
                entity.Property(e => e.Categoryid).HasColumnName("categoryid");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Candidatehistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("candidatehistory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Candidateid).HasColumnName("candidateid");
                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Reapplyallowedafter)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reapplyallowedafter");
                entity.Property(e => e.Reason)
                    .HasColumnType("character varying")
                    .HasColumnName("reason");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("category", "axionpro");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
                entity.Property(e => e.Parentid).HasColumnName("parentid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .HasColumnName("remark");
                entity.Property(e => e.Tags)
                    .HasMaxLength(255)
                    .HasColumnName("tags");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("city", "axionpro");

                entity.Property(e => e.Cityname)
                    .HasMaxLength(100)
                    .HasColumnName("cityname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Stateid).HasColumnName("stateid");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("client", "axionpro");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.Clientname)
                    .HasMaxLength(255)
                    .HasColumnName("clientname");
                entity.Property(e => e.Clienttypeid).HasColumnName("clienttypeid");
                entity.Property(e => e.Contactperson)
                    .HasMaxLength(255)
                    .HasColumnName("contactperson");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(15)
                    .HasColumnName("phonenumber");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
            });

            modelBuilder.Entity<Clienttype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("clienttype", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Typename)
                    .HasMaxLength(50)
                    .HasColumnName("typename");
            });

            modelBuilder.Entity<Companypolicydocument>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("companypolicydocument", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Documenttitle)
                    .HasMaxLength(200)
                    .HasColumnName("documenttitle");
                entity.Property(e => e.Filename)
                    .HasMaxLength(200)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("country", "axionpro");

                entity.Property(e => e.Countrycode)
                    .HasMaxLength(10)
                    .HasColumnName("countrycode");
                entity.Property(e => e.Countryname)
                    .HasMaxLength(100)
                    .HasColumnName("countryname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Countryidentityrule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("countryidentityrule", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Identitycategorydocumentid).HasColumnName("identitycategorydocumentid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismandatory).HasColumnName("ismandatory");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Countrystatutoryrule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("countrystatutoryrule", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismandatory).HasColumnName("ismandatory");
                entity.Property(e => e.Salarythreshold)
                    .HasPrecision(18, 2)
                    .HasColumnName("salarythreshold");
                entity.Property(e => e.Statutorytypeid).HasColumnName("statutorytypeid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Dataviewstructure>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("dataviewstructure", "axionpro");

                entity.Property(e => e.Discription)
                    .HasMaxLength(150)
                    .HasColumnName("discription");
                entity.Property(e => e.Displayon)
                    .HasMaxLength(50)
                    .HasColumnName("displayon");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isdisplayedatpriority).HasColumnName("isdisplayedatpriority");
                entity.Property(e => e.Remark)
                    .HasMaxLength(150)
                    .HasColumnName("remark");
            });

            modelBuilder.Entity<Daycombination>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("daycombination", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Combinationname)
                    .HasMaxLength(100)
                    .HasColumnName("combinationname");
                entity.Property(e => e.Endday).HasColumnName("endday");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Startday).HasColumnName("startday");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Demorequest>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("demorequest", "axionpro");

                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Companyname)
                    .HasMaxLength(200)
                    .HasColumnName("companyname");
                entity.Property(e => e.Contactnumber)
                    .HasMaxLength(20)
                    .HasColumnName("contactnumber");
                entity.Property(e => e.Currenthrms)
                    .HasMaxLength(200)
                    .HasColumnName("currenthrms");
                entity.Property(e => e.Deploymentpreference)
                    .HasMaxLength(50)
                    .HasColumnName("deploymentpreference");
                entity.Property(e => e.Firstname)
                    .HasMaxLength(100)
                    .HasColumnName("firstname");
                entity.Property(e => e.Hasexistingbiometric).HasColumnName("hasexistingbiometric");
                entity.Property(e => e.Hrchallenges)
                    .HasColumnType("character varying")
                    .HasColumnName("hrchallenges");
                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");
                entity.Property(e => e.Industrytype)
                    .HasMaxLength(150)
                    .HasColumnName("industrytype");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isbiometricrequired).HasColumnName("isbiometricrequired");
                entity.Property(e => e.Lastname)
                    .HasMaxLength(100)
                    .HasColumnName("lastname");
                entity.Property(e => e.Numberofemployees).HasColumnName("numberofemployees");
                entity.Property(e => e.Requiredmachinecount).HasColumnName("requiredmachinecount");
                entity.Property(e => e.Requiresintegration).HasColumnName("requiresintegration");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
                entity.Property(e => e.Workemail)
                    .HasMaxLength(150)
                    .HasColumnName("workemail");
            });

            modelBuilder.Entity<Demorequestbiometricdetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("demorequestbiometricdetail", "axionpro");

                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Biometriccompanyname)
                    .HasMaxLength(200)
                    .HasColumnName("biometriccompanyname");
                entity.Property(e => e.Demorequestid)
                    .HasMaxLength(50)
                    .HasColumnName("demorequestid");
                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");
                entity.Property(e => e.Machinecount).HasColumnName("machinecount");
                entity.Property(e => e.Machinelocation)
                    .HasMaxLength(250)
                    .HasColumnName("machinelocation");
                entity.Property(e => e.Modelnumber)
                    .HasMaxLength(150)
                    .HasColumnName("modelnumber");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("department", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Departmentname)
                    .HasMaxLength(255)
                    .HasColumnName("departmentname");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isexecutiveoffice).HasColumnName("isexecutiveoffice");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Designation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("designation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Departmentid).HasColumnName("departmentid");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Designationname)
                    .HasMaxLength(255)
                    .HasColumnName("designationname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("district", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Districtcode)
                    .HasMaxLength(50)
                    .HasColumnName("districtcode");
                entity.Property(e => e.Districtname)
                    .HasMaxLength(200)
                    .HasColumnName("districtname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Pincode)
                    .HasMaxLength(50)
                    .HasColumnName("pincode");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Stateid).HasColumnName("stateid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Districtmaster>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("districtmaster", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Districtcode)
                    .HasMaxLength(50)
                    .HasColumnName("districtcode");
                entity.Property(e => e.Districtname)
                    .HasMaxLength(200)
                    .HasColumnName("districtname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Pincode)
                    .HasMaxLength(50)
                    .HasColumnName("pincode");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Stateid).HasColumnName("stateid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Emailqueue>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("emailqueue", "axionpro");

                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Bccemail)
                    .HasColumnType("character varying")
                    .HasColumnName("bccemail");
                entity.Property(e => e.Body)
                    .HasColumnType("character varying")
                    .HasColumnName("body");
                entity.Property(e => e.Ccemail)
                    .HasColumnType("character varying")
                    .HasColumnName("ccemail");
                entity.Property(e => e.Errormessage)
                    .HasColumnType("character varying")
                    .HasColumnName("errormessage");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Issent).HasColumnName("issent");
                entity.Property(e => e.Retrycount).HasColumnName("retrycount");
                entity.Property(e => e.Senddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("senddatetime");
                entity.Property(e => e.Subject)
                    .HasMaxLength(500)
                    .HasColumnName("subject");
                entity.Property(e => e.Templateid).HasColumnName("templateid");
                entity.Property(e => e.Toemail)
                    .HasMaxLength(250)
                    .HasColumnName("toemail");
            });

            modelBuilder.Entity<Emailslog>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("emailslog", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addedfromip)
                    .HasMaxLength(50)
                    .HasColumnName("addedfromip");
                entity.Property(e => e.Additionalinfojson)
                    .HasColumnType("character varying")
                    .HasColumnName("additionalinfojson");
                entity.Property(e => e.Bccemail)
                    .HasMaxLength(1000)
                    .HasColumnName("bccemail");
                entity.Property(e => e.Body)
                    .HasColumnType("character varying")
                    .HasColumnName("body");
                entity.Property(e => e.Ccemail)
                    .HasMaxLength(1000)
                    .HasColumnName("ccemail");
                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Emailqueueid).HasColumnName("emailqueueid");
                entity.Property(e => e.Errormessage)
                    .HasMaxLength(1000)
                    .HasColumnName("errormessage");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Retrycount).HasColumnName("retrycount");
                entity.Property(e => e.Sentdatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("sentdatetime");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
                entity.Property(e => e.Subject)
                    .HasMaxLength(500)
                    .HasColumnName("subject");
                entity.Property(e => e.Templateid).HasColumnName("templateid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Toemail)
                    .HasMaxLength(500)
                    .HasColumnName("toemail");
                entity.Property(e => e.Triggeredby)
                    .HasMaxLength(100)
                    .HasColumnName("triggeredby");
            });

            modelBuilder.Entity<Emailtemplate>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("emailtemplate", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Addedfromip)
                    .HasMaxLength(50)
                    .HasColumnName("addedfromip");
                entity.Property(e => e.Bccemail)
                    .HasColumnType("character varying")
                    .HasColumnName("bccemail");
                entity.Property(e => e.Body)
                    .HasColumnType("character varying")
                    .HasColumnName("body");
                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .HasColumnName("category");
                entity.Property(e => e.Ccemail)
                    .HasColumnType("character varying")
                    .HasColumnName("ccemail");
                entity.Property(e => e.Fromemail)
                    .HasMaxLength(150)
                    .HasColumnName("fromemail");
                entity.Property(e => e.Fromname)
                    .HasMaxLength(100)
                    .HasColumnName("fromname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Languagecode)
                    .HasMaxLength(10)
                    .HasColumnName("languagecode");
                entity.Property(e => e.Subject)
                    .HasMaxLength(250)
                    .HasColumnName("subject");
                entity.Property(e => e.Templatecode)
                    .HasMaxLength(100)
                    .HasColumnName("templatecode");
                entity.Property(e => e.Templatename)
                    .HasMaxLength(150)
                    .HasColumnName("templatename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Updatedfromip)
                    .HasMaxLength(50)
                    .HasColumnName("updatedfromip");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employee", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Bloodgroup)
                    .HasMaxLength(10)
                    .HasColumnName("bloodgroup");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Dateofbirth)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dateofbirth");
                entity.Property(e => e.Dateofexit)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dateofexit");
                entity.Property(e => e.Dateofonboarding)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dateofonboarding");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Departmentid).HasColumnName("departmentid");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Designationid).HasColumnName("designationid");
                entity.Property(e => e.Emergencycontactnumber)
                    .HasMaxLength(50)
                    .HasColumnName("emergencycontactnumber");
                entity.Property(e => e.Emergencycontactperson)
                    .HasMaxLength(100)
                    .HasColumnName("emergencycontactperson");
                entity.Property(e => e.Employeedocumentid).HasColumnName("employeedocumentid");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Employementcode)
                    .HasMaxLength(50)
                    .HasColumnName("employementcode");
                entity.Property(e => e.Firstname)
                    .HasMaxLength(100)
                    .HasColumnName("firstname");
                entity.Property(e => e.Functionalid).HasColumnName("functionalid");
                entity.Property(e => e.Genderid).HasColumnName("genderid");
                entity.Property(e => e.Haspermanent).HasColumnName("haspermanent");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Ismarried).HasColumnName("ismarried");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Lastname)
                    .HasMaxLength(100)
                    .HasColumnName("lastname");
                entity.Property(e => e.Middlename)
                    .HasMaxLength(100)
                    .HasColumnName("middlename");
                entity.Property(e => e.Mobilenumber)
                    .HasMaxLength(50)
                    .HasColumnName("mobilenumber");
                entity.Property(e => e.Officialemail)
                    .HasMaxLength(255)
                    .HasColumnName("officialemail");
                entity.Property(e => e.Referalid).HasColumnName("referalid");
                entity.Property(e => e.Relation).HasColumnName("relation");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeebankdetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeebankdetail", "axionpro");

                entity.Property(e => e.Accountnumber)
                    .HasMaxLength(50)
                    .HasColumnName("accountnumber");
                entity.Property(e => e.Accounttype)
                    .HasMaxLength(50)
                    .HasColumnName("accounttype");
                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Bankname)
                    .HasMaxLength(100)
                    .HasColumnName("bankname");
                entity.Property(e => e.Branchname)
                    .HasMaxLength(100)
                    .HasColumnName("branchname");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Filename)
                    .HasMaxLength(100)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Filetype).HasColumnName("filetype");
                entity.Property(e => e.Haschequedocuploaded).HasColumnName("haschequedocuploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Ifsccode)
                    .HasMaxLength(20)
                    .HasColumnName("ifsccode");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Isprimaryaccount).HasColumnName("isprimaryaccount");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Upiid)
                    .HasMaxLength(100)
                    .HasColumnName("upiid");
            });

            modelBuilder.Entity<Employeecategoryskill>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeecategoryskill", "axionpro");

                entity.Property(e => e.Categoryid).HasColumnName("categoryid");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Employeecodepattern>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeecodepattern", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Includedepartment).HasColumnName("includedepartment");
                entity.Property(e => e.Includemonth).HasColumnName("includemonth");
                entity.Property(e => e.Includeyear).HasColumnName("includeyear");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Lastusednumber).HasColumnName("lastusednumber");
                entity.Property(e => e.Prefix)
                    .HasMaxLength(20)
                    .HasColumnName("prefix");
                entity.Property(e => e.Runningnumberlength).HasColumnName("runningnumberlength");
                entity.Property(e => e.Separator)
                    .HasMaxLength(5)
                    .HasColumnName("separator");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeecontact>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeecontact", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasColumnName("address");
                entity.Property(e => e.Alternatenumber)
                    .HasMaxLength(20)
                    .HasColumnName("alternatenumber");
                entity.Property(e => e.Contactname)
                    .HasMaxLength(20)
                    .HasColumnName("contactname");
                entity.Property(e => e.Contactnumber)
                    .HasMaxLength(20)
                    .HasColumnName("contactnumber");
                entity.Property(e => e.Contacttype).HasColumnName("contacttype");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Districtid).HasColumnName("districtid");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Houseno)
                    .HasMaxLength(250)
                    .HasColumnName("houseno");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Isprimary).HasColumnName("isprimary");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Landmark)
                    .HasMaxLength(250)
                    .HasColumnName("landmark");
                entity.Property(e => e.Relation).HasColumnName("relation");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Stateid).HasColumnName("stateid");
                entity.Property(e => e.Street)
                    .HasMaxLength(250)
                    .HasColumnName("street");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeedailyattendance>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeedailyattendance", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Attendancedate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("attendancedate");
                entity.Property(e => e.Attendancedevicetypeid).HasColumnName("attendancedevicetypeid");
                entity.Property(e => e.Clickedimage).HasColumnName("clickedimage");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Islate).HasColumnName("islate");
                entity.Property(e => e.Ismarked).HasColumnName("ismarked");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Workstationtypeid).HasColumnName("workstationtypeid");
            });

            modelBuilder.Entity<Employeedependent>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeedependent", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Dateofbirth)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dateofbirth");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Dependentname)
                    .HasMaxLength(200)
                    .HasColumnName("dependentname");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Filename)
                    .HasMaxLength(100)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Filetype).HasColumnName("filetype");
                entity.Property(e => e.Hasproofuploaded).HasColumnName("hasproofuploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iscoveredinpolicy).HasColumnName("iscoveredinpolicy");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Ismarried).HasColumnName("ismarried");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Relation).HasColumnName("relation");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeeducation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeeducation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Degree).HasMaxLength(50);
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Educationgap).HasColumnName("educationgap");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Enddate).HasColumnName("enddate");
                entity.Property(e => e.Filename)
                    .HasMaxLength(100)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Filetype).HasColumnName("filetype");
                entity.Property(e => e.Gapyears).HasColumnName("gapyears");
                entity.Property(e => e.Gradedivision)
                    .HasMaxLength(10)
                    .HasColumnName("gradedivision");
                entity.Property(e => e.Haseducationdocuploded).HasColumnName("haseducationdocuploded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Institutename)
                    .HasMaxLength(100)
                    .HasColumnName("institutename");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Reasonofeducationgap)
                    .HasMaxLength(255)
                    .HasColumnName("reasonofeducationgap");
                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .HasColumnName("remark");
                entity.Property(e => e.Scoretype).HasColumnName("scoretype");
                entity.Property(e => e.Scorevalue)
                    .HasMaxLength(50)
                    .HasColumnName("scorevalue");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeexperience>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeexperience", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.Ctc)
                    .HasPrecision(18, 2)
                    .HasColumnName("ctc");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Hasepfaccount).HasColumnName("hasepfaccount");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isforeignexperience).HasColumnName("isforeignexperience");
                entity.Property(e => e.Isfresher).HasColumnName("isfresher");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeexperiencedetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeexperiencedetail", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Bankstatementdocname)
                    .HasMaxLength(100)
                    .HasColumnName("bankstatementdocname");
                entity.Property(e => e.Bankstatementdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("bankstatementdocpath");
                entity.Property(e => e.Colleaguecontactnumber)
                    .HasMaxLength(15)
                    .HasColumnName("colleaguecontactnumber");
                entity.Property(e => e.Colleaguedesignation)
                    .HasMaxLength(100)
                    .HasColumnName("colleaguedesignation");
                entity.Property(e => e.Colleaguename)
                    .HasMaxLength(100)
                    .HasColumnName("colleaguename");
                entity.Property(e => e.Companyname)
                    .HasMaxLength(100)
                    .HasColumnName("companyname");
                entity.Property(e => e.Designation)
                    .HasMaxLength(50)
                    .HasColumnName("designation");
                entity.Property(e => e.Employeeexperienceid).HasColumnName("employeeexperienceid");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Employeeidofcompany)
                    .HasMaxLength(100)
                    .HasColumnName("employeeidofcompany");
                entity.Property(e => e.Enddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("enddate");
                entity.Property(e => e.Experience).HasColumnName("experience");
                entity.Property(e => e.Experienceletterdocname)
                    .HasMaxLength(100)
                    .HasColumnName("experienceletterdocname");
                entity.Property(e => e.Experienceletterdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("experienceletterdocpath");
                entity.Property(e => e.Foreigncontractdocname)
                    .HasMaxLength(200)
                    .HasColumnName("foreigncontractdocname");
                entity.Property(e => e.Foreigncontractdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("foreigncontractdocpath");
                entity.Property(e => e.Gapcertificatedocname)
                    .HasMaxLength(100)
                    .HasColumnName("gapcertificatedocname");
                entity.Property(e => e.Gapcertificatedocpath)
                    .HasMaxLength(500)
                    .HasColumnName("gapcertificatedocpath");
                entity.Property(e => e.Gapyearfrom)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("gapyearfrom");
                entity.Property(e => e.Gapyearto)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("gapyearto");
                entity.Property(e => e.Hasbankstatementuploaded).HasColumnName("hasbankstatementuploaded");
                entity.Property(e => e.Hasforeigncontractuploaded).HasColumnName("hasforeigncontractuploaded");
                entity.Property(e => e.Hasimmigrationstampuploaded).HasColumnName("hasimmigrationstampuploaded");
                entity.Property(e => e.Hastaxationdoc).HasColumnName("hastaxationdoc");
                entity.Property(e => e.Hasuploadedbankstatement).HasColumnName("hasuploadedbankstatement");
                entity.Property(e => e.Hasuploadedexperienceletter).HasColumnName("hasuploadedexperienceletter");
                entity.Property(e => e.Hasuploadedgapcertificate).HasColumnName("hasuploadedgapcertificate");
                entity.Property(e => e.Hasuploadedjoiningletter).HasColumnName("hasuploadedjoiningletter");
                entity.Property(e => e.Hasuploadedtaxationdoc).HasColumnName("hasuploadedtaxationdoc");
                entity.Property(e => e.Hasvisauploaded).HasColumnName("hasvisauploaded");
                entity.Property(e => e.Hasworkpermituploaded).HasColumnName("hasworkpermituploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Immigrationstampdocname)
                    .HasMaxLength(200)
                    .HasColumnName("immigrationstampdocname");
                entity.Property(e => e.Immigrationstampdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("immigrationstampdocpath");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isanygap).HasColumnName("isanygap");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isexperienceverified).HasColumnName("isexperienceverified");
                entity.Property(e => e.Isexperienceverifiedbycall).HasColumnName("isexperienceverifiedbycall");
                entity.Property(e => e.Isexperienceverifiedbymail).HasColumnName("isexperienceverifiedbymail");
                entity.Property(e => e.Isforeignexperience).HasColumnName("isforeignexperience");
                entity.Property(e => e.Isinfolatestyear).HasColumnName("isinfolatestyear");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Iswfh).HasColumnName("iswfh");
                entity.Property(e => e.Joiningletterdocname)
                    .HasMaxLength(100)
                    .HasColumnName("joiningletterdocname");
                entity.Property(e => e.Joiningletterdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("joiningletterdocpath");
                entity.Property(e => e.Reasonforleaving)
                    .HasMaxLength(100)
                    .HasColumnName("reasonforleaving");
                entity.Property(e => e.Reasonofgap)
                    .HasMaxLength(500)
                    .HasColumnName("reasonofgap");
                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .HasColumnName("remark");
                entity.Property(e => e.Reportingmanagername)
                    .HasMaxLength(100)
                    .HasColumnName("reportingmanagername");
                entity.Property(e => e.Reportingmanagernumber)
                    .HasMaxLength(15)
                    .HasColumnName("reportingmanagernumber");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Startdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("startdate");
                entity.Property(e => e.Taxationdocfilename)
                    .HasMaxLength(100)
                    .HasColumnName("taxationdocfilename");
                entity.Property(e => e.Taxationdocfilepath)
                    .HasMaxLength(500)
                    .HasColumnName("taxationdocfilepath");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Verificationemail)
                    .HasMaxLength(50)
                    .HasColumnName("verificationemail");
                entity.Property(e => e.Visadocname)
                    .HasMaxLength(200)
                    .HasColumnName("visadocname");
                entity.Property(e => e.Visadocpath)
                    .HasMaxLength(500)
                    .HasColumnName("visadocpath");
                entity.Property(e => e.Visatype)
                    .HasMaxLength(50)
                    .HasColumnName("visatype");
                entity.Property(e => e.Workingcountryid).HasColumnName("workingcountryid");
                entity.Property(e => e.Workingdistrictid).HasColumnName("workingdistrictid");
                entity.Property(e => e.Workingstateid).HasColumnName("workingstateid");
                entity.Property(e => e.Workpermitdocname)
                    .HasMaxLength(200)
                    .HasColumnName("workpermitdocname");
                entity.Property(e => e.Workpermitdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("workpermitdocpath");
                entity.Property(e => e.Workpermitnumber)
                    .HasMaxLength(50)
                    .HasColumnName("workpermitnumber");
            });

            modelBuilder.Entity<Employeeexperiencepayslipupload>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeexperiencepayslipupload", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Experiencedetailid).HasColumnName("experiencedetailid");
                entity.Property(e => e.Hasuploadedpayslip).HasColumnName("hasuploadedpayslip");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Payslipdocname)
                    .HasMaxLength(200)
                    .HasColumnName("payslipdocname");
                entity.Property(e => e.Payslipdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("payslipdocpath");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeidentity>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeidentity", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Documentfilename)
                    .HasMaxLength(255)
                    .HasColumnName("documentfilename");
                entity.Property(e => e.Documentfilepath)
                    .HasMaxLength(500)
                    .HasColumnName("documentfilepath");
                entity.Property(e => e.Effectivefrom).HasColumnName("effectivefrom");
                entity.Property(e => e.Effectiveto).HasColumnName("effectiveto");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Hasidentityuploaded).HasColumnName("hasidentityuploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Identitycategorydocumentid).HasColumnName("identitycategorydocumentid");
                entity.Property(e => e.Identityvalue)
                    .HasMaxLength(100)
                    .HasColumnName("identityvalue");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeimage>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeimage", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Filename)
                    .HasMaxLength(50)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Filetype).HasColumnName("filetype");
                entity.Property(e => e.Hasimageuploaded).HasColumnName("hasimageuploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isprimary).HasColumnName("isprimary");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatebyid).HasColumnName("updatebyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeinsurancemapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeinsurancemapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Coverageenddate).HasColumnName("coverageenddate");
                entity.Property(e => e.Coveragestartdate).HasColumnName("coveragestartdate");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Insurancepolicyid).HasColumnName("insurancepolicyid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeinsurancepolicy>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeinsurancepolicy", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Approvedbyid).HasColumnName("approvedbyid");
                entity.Property(e => e.Assigneddate).HasColumnName("assigneddate");
                entity.Property(e => e.Createddate).HasColumnName("createddate");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Insurancepolicyid).HasColumnName("insurancepolicyid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeleavebalance>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeleavebalance", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Availed)
                    .HasPrecision(5, 2)
                    .HasColumnName("availed");
                entity.Property(e => e.Carryforwarded)
                    .HasPrecision(5, 2)
                    .HasColumnName("carryforwarded");
                entity.Property(e => e.Currentbalance)
                    .HasPrecision(6, 2)
                    .HasColumnName("currentbalance");
                entity.Property(e => e.Employeeleavepolicymappingid).HasColumnName("employeeleavepolicymappingid");
                entity.Property(e => e.Encashed)
                    .HasPrecision(5, 2)
                    .HasColumnName("encashed");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isallbalanceonhold).HasColumnName("isallbalanceonhold");
                entity.Property(e => e.Leavesonhold)
                    .HasPrecision(5, 2)
                    .HasColumnName("leavesonhold");
                entity.Property(e => e.Leaveyear).HasColumnName("leaveyear");
                entity.Property(e => e.Openingbalance)
                    .HasPrecision(5, 2)
                    .HasColumnName("openingbalance");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeleavepolicymapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeleavepolicymapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Effectivefrom)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectivefrom");
                entity.Property(e => e.Effectiveto)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectiveto");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isleavebalanceassigned).HasColumnName("isleavebalanceassigned");
                entity.Property(e => e.Policyleavetypemappingid).HasColumnName("policyleavetypemappingid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeemanagermapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeemanagermapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Departmentid).HasColumnName("departmentid");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Designationid).HasColumnName("designationid");
                entity.Property(e => e.Effectivefrom)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectivefrom");
                entity.Property(e => e.Effectiveto)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectiveto");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Managerid).HasColumnName("managerid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Reportingtypeid).HasColumnName("reportingtypeid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeepersonaldetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeepersonaldetail", "axionpro");

                entity.Property(e => e.Aadhaardocname)
                    .HasMaxLength(100)
                    .HasColumnName("aadhaardocname");
                entity.Property(e => e.Aadhaardocpath)
                    .HasMaxLength(500)
                    .HasColumnName("aadhaardocpath");
                entity.Property(e => e.Aadhaarnumber)
                    .HasMaxLength(20)
                    .HasColumnName("aadhaarnumber");
                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Bloodgroup)
                    .HasMaxLength(10)
                    .HasColumnName("bloodgroup");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Drivinglicensenumber)
                    .HasMaxLength(20)
                    .HasColumnName("drivinglicensenumber");
                entity.Property(e => e.Emergencycontactname)
                    .HasMaxLength(100)
                    .HasColumnName("emergencycontactname");
                entity.Property(e => e.Emergencycontactnumber)
                    .HasMaxLength(15)
                    .HasColumnName("emergencycontactnumber");
                entity.Property(e => e.Emergencycontactrelation)
                    .HasMaxLength(50)
                    .HasColumnName("emergencycontactrelation");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Hasaadhaariduploaded).HasColumnName("hasaadhaariduploaded");
                entity.Property(e => e.Hasepfaccount).HasColumnName("hasepfaccount");
                entity.Property(e => e.Haspaniduploaded).HasColumnName("haspaniduploaded");
                entity.Property(e => e.Haspassportiduploaded).HasColumnName("haspassportiduploaded");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Infoverifiedbyid).HasColumnName("infoverifiedbyid");
                entity.Property(e => e.Infoverifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("infoverifieddatetime");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isinfoverified).HasColumnName("isinfoverified");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Maritalstatus).HasColumnName("maritalstatus");
                entity.Property(e => e.Nationality)
                    .HasMaxLength(50)
                    .HasColumnName("nationality");
                entity.Property(e => e.Pandocname)
                    .HasMaxLength(100)
                    .HasColumnName("pandocname");
                entity.Property(e => e.Pandocpath)
                    .HasMaxLength(500)
                    .HasColumnName("pandocpath");
                entity.Property(e => e.Pannumber)
                    .HasMaxLength(20)
                    .HasColumnName("pannumber");
                entity.Property(e => e.Passportdocname)
                    .HasMaxLength(100)
                    .HasColumnName("passportdocname");
                entity.Property(e => e.Passportdocpath)
                    .HasMaxLength(500)
                    .HasColumnName("passportdocpath");
                entity.Property(e => e.Passportnumber)
                    .HasMaxLength(20)
                    .HasColumnName("passportnumber");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Uannumber)
                    .HasMaxLength(20)
                    .HasColumnName("uannumber");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Voterid)
                    .HasMaxLength(20)
                    .HasColumnName("voterid");
            });

            modelBuilder.Entity<Employeeschangedtypehistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeschangedtypehistory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Changedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("changedatetime");
                entity.Property(e => e.Changedbyid).HasColumnName("changedbyid");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Newemployeetypeid).HasColumnName("newemployeetypeid");
                entity.Property(e => e.Oldemployeetypeid).HasColumnName("oldemployeetypeid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeestatutoryaccount>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeestatutoryaccount", "axionpro");

                entity.Property(e => e.Accountnumber)
                    .HasMaxLength(100)
                    .HasColumnName("accountnumber");
                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Contributionenddate).HasColumnName("contributionenddate");
                entity.Property(e => e.Contributionstartdate).HasColumnName("contributionstartdate");
                entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Employercode)
                    .HasMaxLength(100)
                    .HasColumnName("employercode");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Statutorytypeid).HasColumnName("statutorytypeid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeetype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeetype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Typename)
                    .HasMaxLength(255)
                    .HasColumnName("typename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeetypebasicmenu>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeetypebasicmenu", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Basicmenuid).HasColumnName("basicmenuid");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Forplatform).HasColumnName("forplatform");
                entity.Property(e => e.Hasaccess).HasColumnName("hasaccess");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isdisplayable).HasColumnName("isdisplayable");
                entity.Property(e => e.Ismenudisplayinui).HasColumnName("ismenudisplayinui");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Employeeworkdocument>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeworkdocument", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Employeeworkhistoryid).HasColumnName("employeeworkhistoryid");
                entity.Property(e => e.Filename)
                    .HasMaxLength(250)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isverified).HasColumnName("isverified");
                entity.Property(e => e.Verifiedbyid).HasColumnName("verifiedbyid");
                entity.Property(e => e.Verifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("verifieddatetime");
                entity.Property(e => e.Workdocumenttypeid).HasColumnName("workdocumenttypeid");
            });

            modelBuilder.Entity<Employeeworkhistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeworkhistory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Companyname)
                    .HasMaxLength(200)
                    .HasColumnName("companyname");
                entity.Property(e => e.Ctc)
                    .HasPrecision(18, 2)
                    .HasColumnName("ctc");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Designation)
                    .HasMaxLength(150)
                    .HasColumnName("designation");
                entity.Property(e => e.Employeeworkprofileid).HasColumnName("employeeworkprofileid");
                entity.Property(e => e.Enddate).HasColumnName("enddate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isforeignexperience).HasColumnName("isforeignexperience");
                entity.Property(e => e.Isverified).HasColumnName("isverified");
                entity.Property(e => e.Iswfh).HasColumnName("iswfh");
                entity.Property(e => e.Reasonforleaving)
                    .HasMaxLength(500)
                    .HasColumnName("reasonforleaving");
                entity.Property(e => e.Reportingmanagername)
                    .HasMaxLength(150)
                    .HasColumnName("reportingmanagername");
                entity.Property(e => e.Reportingmanagernumber)
                    .HasMaxLength(20)
                    .HasColumnName("reportingmanagernumber");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Verificationemail)
                    .HasMaxLength(150)
                    .HasColumnName("verificationemail");
                entity.Property(e => e.Verificationmode)
                    .HasMaxLength(50)
                    .HasColumnName("verificationmode");
                entity.Property(e => e.Verifiedbyid).HasColumnName("verifiedbyid");
                entity.Property(e => e.Verifieddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("verifieddatetime");
                entity.Property(e => e.Workingcountryid).HasColumnName("workingcountryid");
                entity.Property(e => e.Workingdistrictid).HasColumnName("workingdistrictid");
                entity.Property(e => e.Workingstateid).HasColumnName("workingstateid");
            });

            modelBuilder.Entity<Employeeworkprofile>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("employeeworkprofile", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iseditallowed).HasColumnName("iseditallowed");
                entity.Property(e => e.Isfresher).HasColumnName("isfresher");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Forgotpasswordotpdetail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("forgotpasswordotpdetail", "axionpro");

                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isused).HasColumnName("isused");
                entity.Property(e => e.Isvalidate).HasColumnName("isvalidate");
                entity.Property(e => e.Otp)
                    .HasMaxLength(10)
                    .HasColumnName("otp");
                entity.Property(e => e.Otpexpiredatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("otpexpiredatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Useddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("useddatetime");
                entity.Property(e => e.Userid).HasColumnName("userid");
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("gender", "axionpro");

                entity.Property(e => e.Gendername)
                    .HasMaxLength(50)
                    .HasColumnName("gendername");
                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<Holidaymaster>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("holidaymaster", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Holidaydate).HasColumnName("holidaydate");
                entity.Property(e => e.Holidayname)
                    .HasMaxLength(200)
                    .HasColumnName("holidayname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isregionalholiday).HasColumnName("isregionalholiday");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Isweekend).HasColumnName("isweekend");
                entity.Property(e => e.Region)
                    .HasMaxLength(100)
                    .HasColumnName("region");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Identitycategory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("identitycategory", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Identitycategorydocument>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("identitycategorydocument", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");
                entity.Property(e => e.Documentname)
                    .HasMaxLength(150)
                    .HasColumnName("documentname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Identitycategoryid).HasColumnName("identitycategoryid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isunique).HasColumnName("isunique");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Insurancepolicy>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("insurancepolicy", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Agentcontactnumber)
                    .HasMaxLength(20)
                    .HasColumnName("agentcontactnumber");
                entity.Property(e => e.Agentname)
                    .HasMaxLength(150)
                    .HasColumnName("agentname");
                entity.Property(e => e.Agentofficenumber)
                    .HasMaxLength(20)
                    .HasColumnName("agentofficenumber");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Employeeallowed).HasColumnName("employeeallowed");
                entity.Property(e => e.Enddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("enddate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Inlawsallowed).HasColumnName("inlawsallowed");
                entity.Property(e => e.Insurancepolicyname)
                    .HasMaxLength(200)
                    .HasColumnName("insurancepolicyname");
                entity.Property(e => e.Insurancepolicynumber)
                    .HasMaxLength(100)
                    .HasColumnName("insurancepolicynumber");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Maxchildallowed).HasColumnName("maxchildallowed");
                entity.Property(e => e.Maxspouseallowed).HasColumnName("maxspouseallowed");
                entity.Property(e => e.Parentsallowed).HasColumnName("parentsallowed");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Providername)
                    .HasMaxLength(100)
                    .HasColumnName("providername");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Startdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("startdate");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Insurancepolicydocument>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("insurancepolicydocument", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Documenttype)
                    .HasMaxLength(20)
                    .HasColumnName("documenttype");
                entity.Property(e => e.Filename)
                    .HasMaxLength(200)
                    .HasColumnName("filename");
                entity.Property(e => e.Filepath)
                    .HasMaxLength(500)
                    .HasColumnName("filepath");
                entity.Property(e => e.Filetype).HasColumnName("filetype");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Insurancepolicyid).HasColumnName("insurancepolicyid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Languagecode)
                    .HasMaxLength(10)
                    .HasColumnName("languagecode");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Interviewfeedback>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("interviewfeedback", "axionpro");

                entity.Property(e => e.Candidateid).HasColumnName("candidateid");
                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Feedback)
                    .HasColumnType("character varying")
                    .HasColumnName("feedback");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Interviewscheduleid).HasColumnName("interviewscheduleid");
                entity.Property(e => e.Rating)
                    .HasPrecision(3, 1)
                    .HasColumnName("rating");
                entity.Property(e => e.Reapplyafter)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reapplyafter");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Interviewpanel>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("interviewpanel", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isapproved).HasColumnName("isapproved");
                entity.Property(e => e.Panelname)
                    .HasMaxLength(100)
                    .HasColumnName("panelname");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .HasColumnName("remarks");
            });

            modelBuilder.Entity<Interviewpanelmember>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("interviewpanelmember", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isapproved).HasColumnName("isapproved");
                entity.Property(e => e.Panelid).HasColumnName("panelid");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .HasColumnName("remarks");
                entity.Property(e => e.Userroleid).HasColumnName("userroleid");
            });

            modelBuilder.Entity<Interviewschedule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("interviewschedule", "axionpro");

                entity.Property(e => e.Candidateid).HasColumnName("candidateid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Panelid).HasColumnName("panelid");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .HasColumnName("remarks");
                entity.Property(e => e.Scheduleddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("scheduleddate");
            });

            modelBuilder.Entity<Interviewsdule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("interviewsdule", "axionpro");

                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Description)
                    .HasColumnType("character varying")
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Interviewerid).HasColumnName("interviewerid");
                entity.Property(e => e.Interviewmode)
                    .HasMaxLength(50)
                    .HasColumnName("interviewmode");
                entity.Property(e => e.Remarks)
                    .HasColumnType("character varying")
                    .HasColumnName("remarks");
                entity.Property(e => e.Scheduleddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("scheduleddatetime");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Leaverequest>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leaverequest", "axionpro");

                entity.Property(e => e.Approvedbyid).HasColumnName("approvedbyid");
                entity.Property(e => e.Approveddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("approveddate");
                entity.Property(e => e.Cancellationdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("cancellationdate");
                entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
                entity.Property(e => e.Createddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Fromdate).HasColumnName("fromdate");
                entity.Property(e => e.Halfdaydate).HasColumnName("halfdaydate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isdocumentattached).HasColumnName("isdocumentattached");
                entity.Property(e => e.Isfirsthalf).HasColumnName("isfirsthalf");
                entity.Property(e => e.Ishalfday).HasColumnName("ishalfday");
                entity.Property(e => e.Issandwich).HasColumnName("issandwich");
                entity.Property(e => e.Leavepolicyid).HasColumnName("leavepolicyid");
                entity.Property(e => e.Leavetypeid).HasColumnName("leavetypeid");
                entity.Property(e => e.Reason)
                    .HasMaxLength(500)
                    .HasColumnName("reason");
                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .HasColumnName("remark");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Todate).HasColumnName("todate");
                entity.Property(e => e.Totalleavedays)
                    .HasPrecision(5, 2)
                    .HasColumnName("totalleavedays");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Leaverule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leaverule", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Applysandwichrule).HasColumnName("applysandwichrule");
                entity.Property(e => e.Halfdaynoticehours).HasColumnName("halfdaynoticehours");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ishalfdayallowed).HasColumnName("ishalfdayallowed");
                entity.Property(e => e.Islinkedsandwichrule).HasColumnName("islinkedsandwichrule");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Maxcontinuousleaves).HasColumnName("maxcontinuousleaves");
                entity.Property(e => e.Mingapbetweenleaves).HasColumnName("mingapbetweenleaves");
                entity.Property(e => e.Noticeperioddays).HasColumnName("noticeperioddays");
                entity.Property(e => e.Policyleavetypeid).HasColumnName("policyleavetypeid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletebyid).HasColumnName("softdeletebyid");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Leavesandwichrule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leavesandwichrule", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isincludeholiday).HasColumnName("isincludeholiday");
                entity.Property(e => e.Isincludeweekend).HasColumnName("isincludeweekend");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Rulename)
                    .HasMaxLength(100)
                    .HasColumnName("rulename");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Leavesandwichrulemapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leavesandwichrulemapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Daycombinationid).HasColumnName("daycombinationid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Leaveruleid).HasColumnName("leaveruleid");
                entity.Property(e => e.Leavesandwichruleid).HasColumnName("leavesandwichruleid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Leavetransactionlog>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leavetransactionlog", "axionpro");

                entity.Property(e => e.Employeeleavebalanceid).HasColumnName("employeeleavebalanceid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Leavedays)
                    .HasPrecision(5, 2)
                    .HasColumnName("leavedays");
                entity.Property(e => e.Performedby).HasColumnName("performedby");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(500)
                    .HasColumnName("remarks");
                entity.Property(e => e.Transactiondate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("transactiondate");
                entity.Property(e => e.Transactiontype)
                    .HasMaxLength(20)
                    .HasColumnName("transactiontype");
            });

            modelBuilder.Entity<Leavetype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("leavetype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Leavename)
                    .HasMaxLength(50)
                    .HasColumnName("leavename");
                entity.Property(e => e.Softdeletedby).HasColumnName("softdeletedby");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatebyid).HasColumnName("updatebyid");
                entity.Property(e => e.Updatedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updatedatetime");
            });

            modelBuilder.Entity<License>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("license", "axionpro");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Licenseenddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("licenseenddate");
                entity.Property(e => e.Licensestartdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("licensestartdate");
            });

            modelBuilder.Entity<Logincredential>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("logincredential", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Hasfirstlogin).HasColumnName("hasfirstlogin");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Ipaddresslocal)
                    .HasMaxLength(50)
                    .HasColumnName("ipaddresslocal");
                entity.Property(e => e.Ipaddresspublic)
                    .HasMaxLength(50)
                    .HasColumnName("ipaddresspublic");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ispasswordchangerequired).HasColumnName("ispasswordchangerequired");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Logindevice).HasColumnName("logindevice");
                entity.Property(e => e.Loginid)
                    .HasMaxLength(255)
                    .HasColumnName("loginid");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
                entity.Property(e => e.Macaddress)
                    .HasMaxLength(255)
                    .HasColumnName("macaddress");
                entity.Property(e => e.Password).HasMaxLength(550);
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Mealallowancepolicybydesignation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("mealallowancepolicybydesignation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Breakfastallowance)
                    .HasPrecision(10, 2)
                    .HasColumnName("breakfastallowance");
                entity.Property(e => e.Designationid).HasColumnName("designationid");
                entity.Property(e => e.Dinnerallowance)
                    .HasPrecision(10, 2)
                    .HasColumnName("dinnerallowance");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Fixedfoodallowance)
                    .HasPrecision(10, 2)
                    .HasColumnName("fixedfoodallowance");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismetro).HasColumnName("ismetro");
                entity.Property(e => e.Issoftdelete).HasColumnName("issoftdelete");
                entity.Property(e => e.Lunchallowance)
                    .HasPrecision(10, 2)
                    .HasColumnName("lunchallowance");
                entity.Property(e => e.Metrobonus)
                    .HasPrecision(10, 2)
                    .HasColumnName("metrobonus");
                entity.Property(e => e.Mindaysrequired).HasColumnName("mindaysrequired");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Requireddocuments).HasColumnName("requireddocuments");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("Module", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Displayname)
                    .HasMaxLength(100)
                    .HasColumnName("displayname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Imageiconmobile)
                    .HasMaxLength(255)
                    .HasColumnName("imageiconmobile");
                entity.Property(e => e.Imageiconweb)
                    .HasMaxLength(255)
                    .HasColumnName("imageiconweb");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iscommonmenu).HasColumnName("iscommonmenu");
                entity.Property(e => e.Isleafnode).HasColumnName("isleafnode");
                entity.Property(e => e.Ismoduledisplayinui).HasColumnName("ismoduledisplayinui");
                entity.Property(e => e.Itempriority).HasColumnName("itempriority");
                entity.Property(e => e.Modulecode)
                    .HasMaxLength(50)
                    .HasColumnName("modulecode");
                entity.Property(e => e.Modulename)
                    .HasMaxLength(100)
                    .HasColumnName("modulename");
                entity.Property(e => e.Parentmoduleid).HasColumnName("parentmoduleid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Urlpath)
                    .HasMaxLength(500)
                    .HasColumnName("urlpath");
            });

            modelBuilder.Entity<Module1>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("module", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Displayname)
                    .HasMaxLength(100)
                    .HasColumnName("displayname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Imageiconmobile)
                    .HasMaxLength(255)
                    .HasColumnName("imageiconmobile");
                entity.Property(e => e.Imageiconweb)
                    .HasMaxLength(255)
                    .HasColumnName("imageiconweb");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iscommonmenu).HasColumnName("iscommonmenu");
                entity.Property(e => e.Isleafnode).HasColumnName("isleafnode");
                entity.Property(e => e.Ismoduledisplayinui).HasColumnName("ismoduledisplayinui");
                entity.Property(e => e.Itempriority).HasColumnName("itempriority");
                entity.Property(e => e.Modulecode)
                    .HasMaxLength(50)
                    .HasColumnName("modulecode");
                entity.Property(e => e.Modulename)
                    .HasMaxLength(100)
                    .HasColumnName("modulename");
                entity.Property(e => e.Parentmoduleid).HasColumnName("parentmoduleid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Urlpath)
                    .HasMaxLength(500)
                    .HasColumnName("urlpath");
            });

            modelBuilder.Entity<Moduleoperationmapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("moduleoperationmapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Dataviewstructureid).HasColumnName("dataviewstructureid");
                entity.Property(e => e.Iconurl)
                    .HasMaxLength(255)
                    .HasColumnName("iconurl");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iscommonitem).HasColumnName("iscommonitem");
                entity.Property(e => e.Isoperational).HasColumnName("isoperational");
                entity.Property(e => e.Moduleid).HasColumnName("moduleid");
                entity.Property(e => e.Operationid).HasColumnName("operationid");
                entity.Property(e => e.Pagetypeid).HasColumnName("pagetypeid");
                entity.Property(e => e.Pageurl)
                    .HasMaxLength(255)
                    .HasColumnName("pageurl");
                entity.Property(e => e.Priority).HasColumnName("priority");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Noimagepath>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("noimagepath", "axionpro");

                entity.Property(e => e.Defaultimagepath)
                    .HasMaxLength(500)
                    .HasColumnName("defaultimagepath");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Imagename)
                    .HasMaxLength(50)
                    .HasColumnName("imagename");
                entity.Property(e => e.Imagetype).HasColumnName("imagetype");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("operation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Iconimage)
                    .HasMaxLength(250)
                    .HasColumnName("iconimage");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Operationname)
                    .HasMaxLength(200)
                    .HasColumnName("operationname");
                entity.Property(e => e.Operationtype).HasColumnName("operationtype");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Updatedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updatedatetime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
            });

            modelBuilder.Entity<Organizationholidaycalendar>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("organizationholidaycalendar", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Countrycode)
                    .HasMaxLength(5)
                    .HasColumnName("countrycode");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Holidaydate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("holidaydate");
                entity.Property(e => e.Holidayname)
                    .HasMaxLength(100)
                    .HasColumnName("holidayname");
                entity.Property(e => e.Holidayyear).HasColumnName("holidayyear");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isoptional).HasColumnName("isoptional");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Statecode)
                    .HasMaxLength(10)
                    .HasColumnName("statecode");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Pagetypeenum>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("pagetypeenum", "axionpro");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Pagetypename)
                    .HasMaxLength(20)
                    .HasColumnName("pagetypename");
            });

            modelBuilder.Entity<Planmodulemapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("planmodulemapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Moduleid).HasColumnName("moduleid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Subscriptionplanid).HasColumnName("subscriptionplanid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Policyleavetypemapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("policyleavetypemapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Applicablegenderid).HasColumnName("applicablegenderid");
                entity.Property(e => e.Carryforward).HasColumnName("carryforward");
                entity.Property(e => e.Carryforwardexpirymonths).HasColumnName("carryforwardexpirymonths");
                entity.Property(e => e.Effectivefrom)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectivefrom");
                entity.Property(e => e.Effectiveto)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("effectiveto");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Encashable).HasColumnName("encashable");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isemployeemapped).HasColumnName("isemployeemapped");
                entity.Property(e => e.Ismarriedapplicable).HasColumnName("ismarriedapplicable");
                entity.Property(e => e.Isproofrequired).HasColumnName("isproofrequired");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Leavetypeid).HasColumnName("leavetypeid");
                entity.Property(e => e.Maxcarryforward).HasColumnName("maxcarryforward");
                entity.Property(e => e.Maxencashable).HasColumnName("maxencashable");
                entity.Property(e => e.Monthlyaccrual).HasColumnName("monthlyaccrual");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Proofdocumenttype)
                    .HasMaxLength(100)
                    .HasColumnName("proofdocumenttype");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletebyid).HasColumnName("softdeletebyid");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Totalleavesperyear).HasColumnName("totalleavesperyear");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Policytype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("policytype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdelete).HasColumnName("issoftdelete");
                entity.Property(e => e.Isstructured).HasColumnName("isstructured");
                entity.Property(e => e.Policyname)
                    .HasMaxLength(255)
                    .HasColumnName("policyname");
                entity.Property(e => e.Softdeletebyid).HasColumnName("softdeletebyid");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatebyid).HasColumnName("updatebyid");
                entity.Property(e => e.Updatedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updatedatetime");
            });

            modelBuilder.Entity<Policytypeinsurancemapping>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("policytypeinsurancemapping", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Insurancepolicyid).HasColumnName("insurancepolicyid");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Softdeletebyid).HasColumnName("softdeletebyid");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Refreshtoken>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("refreshtoken", "axionpro");

                entity.Property(e => e.Createdat)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createdat");
                entity.Property(e => e.Createdbyip)
                    .HasMaxLength(50)
                    .HasColumnName("createdbyip");
                entity.Property(e => e.Expirydate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("expirydate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isrevoked).HasColumnName("isrevoked");
                entity.Property(e => e.Loginid)
                    .HasMaxLength(255)
                    .HasColumnName("loginid");
                entity.Property(e => e.Replacedbytoken)
                    .HasMaxLength(500)
                    .HasColumnName("replacedbytoken");
                entity.Property(e => e.Revokedat)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("revokedat");
                entity.Property(e => e.Revokedbyip)
                    .HasMaxLength(50)
                    .HasColumnName("revokedbyip");
                entity.Property(e => e.Token).HasMaxLength(500);
            });

            modelBuilder.Entity<Reportingtype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("reportingtype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Typename)
                    .HasMaxLength(50)
                    .HasColumnName("typename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Requesttype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("requesttype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Requesttypename)
                    .HasMaxLength(100)
                    .HasColumnName("requesttypename");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("role", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Issystemdefault).HasColumnName("issystemdefault");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Rolename)
                    .HasMaxLength(100)
                    .HasColumnName("rolename");
                entity.Property(e => e.Roletype).HasColumnName("roletype");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Role1>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("Role", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Issystemdefault).HasColumnName("issystemdefault");
                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasColumnName("remark");
                entity.Property(e => e.Rolename)
                    .HasMaxLength(100)
                    .HasColumnName("rolename");
                entity.Property(e => e.Roletype).HasColumnName("roletype");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Rolemoduleandpermission>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("rolemoduleandpermission", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Hasaccess).HasColumnName("hasaccess");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Imageicon)
                    .HasMaxLength(200)
                    .HasColumnName("imageicon");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isoperational).HasColumnName("isoperational");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Moduleid).HasColumnName("moduleid");
                entity.Property(e => e.Operationid).HasColumnName("operationid");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Roleid).HasColumnName("roleid");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updatedatetime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Serviceprovider>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("serviceprovider", "axionpro");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnName("address");
                entity.Property(e => e.Ceoname)
                    .HasMaxLength(255)
                    .HasColumnName("ceoname");
                entity.Property(e => e.Companyemail)
                    .HasMaxLength(255)
                    .HasColumnName("companyemail");
                entity.Property(e => e.Companyname)
                    .HasMaxLength(255)
                    .HasColumnName("companyname");
                entity.Property(e => e.Companytype)
                    .HasMaxLength(50)
                    .HasColumnName("companytype");
                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");
                entity.Property(e => e.Establishedyear).HasColumnName("establishedyear");
                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .HasColumnName("fax");
                entity.Property(e => e.Gstnumber)
                    .HasMaxLength(50)
                    .HasColumnName("gstnumber");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");
                entity.Property(e => e.Pincode)
                    .HasMaxLength(10)
                    .HasColumnName("pincode");
                entity.Property(e => e.Profile)
                    .HasMaxLength(500)
                    .HasColumnName("profile");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Websiteurl)
                    .HasMaxLength(255)
                    .HasColumnName("websiteurl");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("state", "axionpro");

                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Statename)
                    .HasMaxLength(100)
                    .HasColumnName("statename");
            });

            modelBuilder.Entity<Statutorytype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("statutorytype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isemployeecontributionrequired).HasColumnName("isemployeecontributionrequired");
                entity.Property(e => e.Isemployercontributionrequired).HasColumnName("isemployercontributionrequired");
                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("name");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Subscriptionplan>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("subscriptionplan", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Currencykey)
                    .HasMaxLength(20)
                    .HasColumnName("currencykey");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Iscustom).HasColumnName("iscustom");
                entity.Property(e => e.Isfree).HasColumnName("isfree");
                entity.Property(e => e.Ismostpopular).HasColumnName("ismostpopular");
                entity.Property(e => e.Maxusers).HasColumnName("maxusers");
                entity.Property(e => e.Monthlyprice)
                    .HasPrecision(10, 2)
                    .HasColumnName("monthlyprice");
                entity.Property(e => e.Perdayprice)
                    .HasPrecision(10, 2)
                    .HasColumnName("perdayprice");
                entity.Property(e => e.Planname)
                    .HasMaxLength(100)
                    .HasColumnName("planname");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Yearlyprice)
                    .HasPrecision(10, 2)
                    .HasColumnName("yearlyprice");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenant", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Companyemaildomain)
                    .HasMaxLength(255)
                    .HasColumnName("companyemaildomain");
                entity.Property(e => e.Companyname)
                    .HasMaxLength(200)
                    .HasColumnName("companyname");
                entity.Property(e => e.Contactnumber)
                    .HasMaxLength(20)
                    .HasColumnName("contactnumber");
                entity.Property(e => e.Contactpersonname)
                    .HasMaxLength(100)
                    .HasColumnName("contactpersonname");
                entity.Property(e => e.Countryid).HasColumnName("countryid");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Genderid).HasColumnName("genderid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Isverified).HasColumnName("isverified");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Tenantcode)
                    .HasMaxLength(100)
                    .HasColumnName("tenantcode");
                entity.Property(e => e.Tenantemail)
                    .HasMaxLength(200)
                    .HasColumnName("tenantemail");
                entity.Property(e => e.Tenantindustryid).HasColumnName("tenantindustryid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tenantemailconfig>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantemailconfig", "axionpro");

                entity.Property(e => e.Fromemail)
                    .HasMaxLength(200)
                    .HasColumnName("fromemail");
                entity.Property(e => e.Fromname)
                    .HasMaxLength(100)
                    .HasColumnName("fromname");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Smtphost)
                    .HasMaxLength(200)
                    .HasColumnName("smtphost");
                entity.Property(e => e.Smtppasswordencrypted)
                    .HasMaxLength(500)
                    .HasColumnName("smtppasswordencrypted");
                entity.Property(e => e.Smtpport).HasColumnName("smtpport");
                entity.Property(e => e.Smtpusername)
                    .HasMaxLength(200)
                    .HasColumnName("smtpusername");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
            });

            modelBuilder.Entity<Tenantenabledmodule>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantenabledmodule", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isenabled).HasColumnName("isenabled");
                entity.Property(e => e.Isleafnode).HasColumnName("isleafnode");
                entity.Property(e => e.Moduleid).HasColumnName("moduleid");
                entity.Property(e => e.Parentmoduleid).HasColumnName("parentmoduleid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tenantenabledoperation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantenabledoperation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isenabled).HasColumnName("isenabled");
                entity.Property(e => e.Isoperationused).HasColumnName("isoperationused");
                entity.Property(e => e.Moduleid).HasColumnName("moduleid");
                entity.Property(e => e.Operationid).HasColumnName("operationid");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tenantencryptionkey>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantencryptionkeys", "axionpro");

                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Encryptionkey)
                    .HasMaxLength(1000)
                    .HasColumnName("encryptionkey");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tenantindustry>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantindustry", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Industryname)
                    .HasMaxLength(100)
                    .HasColumnName("industryname");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeted).HasColumnName("issoftdeted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeleteddatetime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tenantprofile>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantprofile", "axionpro");

                entity.Property(e => e.Address)
                    .HasMaxLength(300)
                    .HasColumnName("address");
                entity.Property(e => e.Businesstype)
                    .HasMaxLength(100)
                    .HasColumnName("businesstype");
                entity.Property(e => e.Foundedyear).HasColumnName("foundedyear");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Industry)
                    .HasMaxLength(100)
                    .HasColumnName("industry");
                entity.Property(e => e.Logourl)
                    .HasMaxLength(255)
                    .HasColumnName("logourl");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Themecolor)
                    .HasMaxLength(50)
                    .HasColumnName("themecolor");
                entity.Property(e => e.Totalbranches).HasColumnName("totalbranches");
                entity.Property(e => e.Totalemployees).HasColumnName("totalemployees");
                entity.Property(e => e.Websiteurl)
                    .HasMaxLength(200)
                    .HasColumnName("websiteurl");
            });

            modelBuilder.Entity<Tenantsubscription>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenantsubscription", "axionpro");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Istrial).HasColumnName("istrial");
                entity.Property(e => e.Paymentmode)
                    .HasMaxLength(50)
                    .HasColumnName("paymentmode");
                entity.Property(e => e.Paymenttxnid)
                    .HasMaxLength(100)
                    .HasColumnName("paymenttxnid");
                entity.Property(e => e.Subscriptionenddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("subscriptionenddate");
                entity.Property(e => e.Subscriptionplanid).HasColumnName("subscriptionplanid");
                entity.Property(e => e.Subscriptionstartdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("subscriptionstartdate");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
            });

            modelBuilder.Entity<Tender>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tender", "axionpro");

                entity.Property(e => e.Clientid).HasColumnName("clientid");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Enddate).HasColumnName("enddate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Tendername)
                    .HasMaxLength(255)
                    .HasColumnName("tendername");
                entity.Property(e => e.Tenderstatusid).HasColumnName("tenderstatusid");
                entity.Property(e => e.Tendervalue)
                    .HasPrecision(18, 2)
                    .HasColumnName("tendervalue");
            });

            modelBuilder.Entity<Tenderproject>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderproject", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");
                entity.Property(e => e.Enddate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("enddate");
                entity.Property(e => e.Estimatedbudget)
                    .HasPrecision(18, 2)
                    .HasColumnName("estimatedbudget");
                entity.Property(e => e.Expectedteamsize).HasColumnName("expectedteamsize");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Projectname)
                    .HasMaxLength(255)
                    .HasColumnName("projectname");
                entity.Property(e => e.Remark)
                    .HasMaxLength(1000)
                    .HasColumnName("remark");
                entity.Property(e => e.Startdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("startdate");
                entity.Property(e => e.Statusid).HasColumnName("statusid");
                entity.Property(e => e.Tenderserviceproviderid).HasColumnName("tenderserviceproviderid");
                entity.Property(e => e.Userroleid).HasColumnName("userroleid");
            });

            modelBuilder.Entity<Tenderservice>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderservice", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Tenderid).HasColumnName("tenderid");
                entity.Property(e => e.Tenderservicetypeid).HasColumnName("tenderservicetypeid");
            });

            modelBuilder.Entity<Tenderservicehistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderservicehistory", "axionpro");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Status)
                    .HasMaxLength(100)
                    .HasColumnName("status");
                entity.Property(e => e.Tenderserviceid).HasColumnName("tenderserviceid");
            });

            modelBuilder.Entity<Tenderserviceprovider>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderserviceprovider", "axionpro");

                entity.Property(e => e.Contractamount)
                    .HasPrecision(18, 2)
                    .HasColumnName("contractamount");
                entity.Property(e => e.Contractenddate).HasColumnName("contractenddate");
                entity.Property(e => e.Contractstartdate).HasColumnName("contractstartdate");
                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isinhouse).HasColumnName("isinhouse");
                entity.Property(e => e.Isprimaryprovider).HasColumnName("isprimaryprovider");
                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .HasColumnName("remark");
                entity.Property(e => e.Serviceproviderid).HasColumnName("serviceproviderid");
                entity.Property(e => e.Statusid).HasColumnName("statusid");
                entity.Property(e => e.Tenderservicespecificationid).HasColumnName("tenderservicespecificationid");
            });

            modelBuilder.Entity<Tenderservicespecification>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderservicespecification", "axionpro");

                entity.Property(e => e.Estimatedbudget)
                    .HasPrecision(18, 2)
                    .HasColumnName("estimatedbudget");
                entity.Property(e => e.Experiencerequired).HasColumnName("experiencerequired");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Noticeperiodconsidered).HasColumnName("noticeperiodconsidered");
                entity.Property(e => e.Productplatform)
                    .HasMaxLength(255)
                    .HasColumnName("productplatform");
                entity.Property(e => e.Productspecification)
                    .HasMaxLength(1000)
                    .HasColumnName("productspecification");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Specificationname)
                    .HasMaxLength(255)
                    .HasColumnName("specificationname");
                entity.Property(e => e.Specificationtype)
                    .HasMaxLength(50)
                    .HasColumnName("specificationtype");
                entity.Property(e => e.Tenderserviceid).HasColumnName("tenderserviceid");
            });

            modelBuilder.Entity<Tenderservicetype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderservicetype", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Parentserviceid).HasColumnName("parentserviceid");
                entity.Property(e => e.Servicename)
                    .HasMaxLength(255)
                    .HasColumnName("servicename");
            });

            modelBuilder.Entity<Tenderstatus>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tenderstatus", "axionpro");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Statusname)
                    .HasMaxLength(100)
                    .HasColumnName("statusname");
            });

            modelBuilder.Entity<Ticketclassification>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("ticketclassification", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Classificationname)
                    .HasMaxLength(100)
                    .HasColumnName("classificationname");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeletedtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedtime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Ticketheader>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("ticketheader", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.Headername)
                    .HasMaxLength(150)
                    .HasColumnName("headername");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isassetrelated).HasColumnName("isassetrelated");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeletedtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedtime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Ticketclassificationid).HasColumnName("ticketclassificationid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Tickettype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("tickettype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Responsibleroleid).HasColumnName("responsibleroleid");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Softdeletedtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedtime");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Ticketheaderid).HasColumnName("ticketheaderid");
                entity.Property(e => e.Tickettypename)
                    .HasMaxLength(100)
                    .HasColumnName("tickettypename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Travelallowancepolicybydesignation>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("travelallowancepolicybydesignation", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Advanceallowed).HasColumnName("advanceallowed");
                entity.Property(e => e.Designationid).HasColumnName("designationid");
                entity.Property(e => e.Employeetypeid).HasColumnName("employeetypeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismetro).HasColumnName("ismetro");
                entity.Property(e => e.Issoftdelete).HasColumnName("issoftdelete");
                entity.Property(e => e.Maxadvanceamount)
                    .HasPrecision(10, 2)
                    .HasColumnName("maxadvanceamount");
                entity.Property(e => e.Maxtraveldistance).HasColumnName("maxtraveldistance");
                entity.Property(e => e.Metrobonus)
                    .HasPrecision(10, 2)
                    .HasColumnName("metrobonus");
                entity.Property(e => e.Mintraveldistance).HasColumnName("mintraveldistance");
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Reimbursementperkm)
                    .HasPrecision(10, 2)
                    .HasColumnName("reimbursementperkm");
                entity.Property(e => e.Requireddocuments).HasColumnName("requireddocuments");
                entity.Property(e => e.Softdeletedatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("softdeletedatetime");
                entity.Property(e => e.Travelclass)
                    .HasMaxLength(50)
                    .HasColumnName("travelclass");
                entity.Property(e => e.Travelmodeid).HasColumnName("travelmodeid");
            });

            modelBuilder.Entity<Travelmode>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("travelmode", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Travelmodename)
                    .HasMaxLength(255)
                    .HasColumnName("travelmodename");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Userattendancesetting>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("userattendancesetting", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Attendancedevicetypeid).HasColumnName("attendancedevicetypeid");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Geofencelatitude)
                    .HasPrecision(10, 8)
                    .HasColumnName("geofencelatitude");
                entity.Property(e => e.Geofencelongitude)
                    .HasPrecision(10, 8)
                    .HasColumnName("geofencelongitude");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isallowed).HasColumnName("isallowed");
                entity.Property(e => e.Isgeofenceenabled).HasColumnName("isgeofenceenabled");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Reportingtime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reportingtime");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Workstationtypeid).HasColumnName("workstationtypeid");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("userrole", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Approvalrequired).HasColumnName("approvalrequired");
                entity.Property(e => e.Approvalstatus)
                    .HasMaxLength(20)
                    .HasColumnName("approvalstatus");
                entity.Property(e => e.Assignedbyid).HasColumnName("assignedbyid");
                entity.Property(e => e.Assigneddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("assigneddatetime");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Employeeid).HasColumnName("employeeid");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Isprimaryrole).HasColumnName("isprimaryrole");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark");
                entity.Property(e => e.Removeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("removeddatetime");
                entity.Property(e => e.Roleid).HasColumnName("roleid");
                entity.Property(e => e.Rolestartdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("rolestartdate");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Workdocumenttype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("workdocumenttype", "axionpro");

                entity.Property(e => e.Documenttypename)
                    .HasMaxLength(100)
                    .HasColumnName("documenttypename");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Workflowstage>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("workflowstage", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Colorkey)
                    .HasMaxLength(50)
                    .HasColumnName("colorkey");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Stagename)
                    .HasMaxLength(100)
                    .HasColumnName("stagename");
                entity.Property(e => e.Stageorder).HasColumnName("stageorder");
                entity.Property(e => e.Tenantid).HasColumnName("tenantid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Workflowstep>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("workflowstep", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Approvallevel).HasColumnName("approvallevel");
                entity.Property(e => e.Approvalworkflowid).HasColumnName("approvalworkflowid");
                entity.Property(e => e.Approverreferenceid).HasColumnName("approverreferenceid");
                entity.Property(e => e.Approvertype).HasColumnName("approvertype");
                entity.Property(e => e.Deleteddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleteddatetime");
                entity.Property(e => e.Escalateafterdays).HasColumnName("escalateafterdays");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Ismandatory).HasColumnName("ismandatory");
                entity.Property(e => e.Issoftdeleted).HasColumnName("issoftdeleted");
                entity.Property(e => e.Remark)
                    .HasMaxLength(250)
                    .HasColumnName("remark");
                entity.Property(e => e.Softdeletedbyid).HasColumnName("softdeletedbyid");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
            });

            modelBuilder.Entity<Workstationtype>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("workstationtype", "axionpro");

                entity.Property(e => e.Addedbyid).HasColumnName("addedbyid");
                entity.Property(e => e.Addeddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("addeddatetime");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Isactive).HasColumnName("isactive");
                entity.Property(e => e.Updatedbyid).HasColumnName("updatedbyid");
                entity.Property(e => e.Updateddatetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updateddatetime");
                entity.Property(e => e.Workstation)
                    .HasMaxLength(20)
                    .HasColumnName("workstation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


}
