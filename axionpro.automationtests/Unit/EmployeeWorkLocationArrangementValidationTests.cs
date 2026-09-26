using axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo;
using axionpro.domain.Entity;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeWorkArrangement")]
[Category("EmployeeWorkLocation")]
public sealed class EmployeeWorkLocationArrangementValidationTests
{
    [TestCase(WorkMode.ClientSite, TenantLocationType.ClientSite, true)]
    [TestCase(WorkMode.ClientSite, TenantLocationType.HeadOffice, false)]
    [TestCase(WorkMode.Office, TenantLocationType.HeadOffice, true)]
    [TestCase(WorkMode.Office, TenantLocationType.ClientSite, false)]
    [TestCase(WorkMode.Hybrid, TenantLocationType.Branch, true)]
    [TestCase(WorkMode.Field, TenantLocationType.ProjectSite, true)]
    [TestCase(WorkMode.WorkFromHome, TenantLocationType.RemoteOffice, false)]
    public void Work_mode_accepts_only_compatible_primary_location_types(WorkMode workMode, TenantLocationType locationType, bool expected)
    {
        Assert.That(EmployeeWorkConfigurationRules.IsLocationTypeCompatible(workMode, locationType), Is.EqualTo(expected));
    }

    [Test]
    public void Inclusive_effective_windows_detect_overlap_and_allow_adjacent_history()
    {
        Assert.Multiple(() =>
        {
            Assert.That(EmployeeWorkConfigurationRules.EffectiveWindowsOverlap(
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31),
                new DateOnly(2026, 1, 31),
                new DateOnly(2026, 2, 15)), Is.True);
            Assert.That(EmployeeWorkConfigurationRules.EffectiveWindowsOverlap(
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31),
                new DateOnly(2026, 2, 1),
                null), Is.False);
            Assert.That(EmployeeWorkConfigurationRules.EffectiveWindowsOverlap(
                new DateOnly(2026, 1, 1),
                null,
                new DateOnly(2030, 1, 1),
                null), Is.True);
        });
    }

    [Test]
    public void Assignment_must_cover_the_complete_arrangement_window()
    {
        Assert.Multiple(() =>
        {
            Assert.That(EmployeeWorkConfigurationRules.AssignmentCoversArrangement(
                new DateOnly(2026, 1, 1), null, new DateOnly(2026, 2, 1), null), Is.True);
            Assert.That(EmployeeWorkConfigurationRules.AssignmentCoversArrangement(
                new DateOnly(2026, 2, 2), null, new DateOnly(2026, 2, 1), null), Is.False);
            Assert.That(EmployeeWorkConfigurationRules.AssignmentCoversArrangement(
                new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new DateOnly(2026, 2, 1), null), Is.False);
            Assert.That(EmployeeWorkConfigurationRules.AssignmentCoversArrangement(
                new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new DateOnly(2026, 2, 1), new DateOnly(2026, 11, 30)), Is.True);
        });
    }

    [Test]
    public void Attendance_policy_configuration_controls_physical_and_wfh_arrangements_without_device_enrollment()
    {
        var configuration = new AttendancePolicyVersionConfiguration
        {
            AttendanceLocationScope = (short)AttendanceLocationScope.AssignedLocations,
            AllowMobile = true,
            AllowWorkFromHome = true
        };

        Assert.Multiple(() =>
        {
            Assert.That(EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(
                WorkMode.Office, 10, configuration), Is.True);
            Assert.That(EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(
                WorkMode.WorkFromHome, null, configuration), Is.True);
            Assert.That(EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(
                WorkMode.WorkFromHome, 10, configuration), Is.False);

            configuration.AttendanceLocationScope = (short)AttendanceLocationScope.RemoteAnywhere;
            Assert.That(EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(
                WorkMode.Office, 10, configuration), Is.False);

            configuration.AllowWorkFromHome = false;
            Assert.That(EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(
                WorkMode.WorkFromHome, null, configuration), Is.False);
        });
    }

    [Test]
    public void Typed_attendance_policy_version_schema_is_one_to_one_and_backfills_existing_versions()
    {
        var root = FindRepositoryRoot();
        var context = File.ReadAllText(Path.Combine(root, "axionpro.persistance", "Data", "Context", "WorkforceDbContext.PolicyFramework.cs"));
        var repository = File.ReadAllText(Path.Combine(root, "axionpro.persistance", "Repositories", "GenericPolicyRepository.cs"));
        var script = File.ReadAllText(Path.Combine(root, "database-scripts", "AddAttendancePolicyVersionConfiguration.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(context, Does.Contain("WithOne(e => e.AttendanceConfiguration)"));
            Assert.That(context, Does.Contain("HasIndex(e => e.PolicyVersionId).IsUnique()"));
            Assert.That(repository, Does.Contain("ValidateAttendanceConfigurationAsync"));
            Assert.That(repository, Does.Contain("UpsertAttendanceConfigurationAsync"));
            Assert.That(repository, Does.Contain("ToDto(attendanceConfiguration)"));
            Assert.That(script, Does.Contain("CK_AttendancePolicyVersionConfiguration_Channel"));
            Assert.That(script, Does.Contain("upper(category.\"CategoryCode\") = 'ATTENDANCE'"));
            Assert.That(script, Does.Contain("NOT EXISTS"));
        });
    }

    [Test]
    public void Repository_and_handlers_enforce_cross_record_and_date_window_rules()
    {
        var root = FindRepositoryRoot();
        var repository = File.ReadAllText(Path.Combine(root, "axionpro.persistance", "Repositories", "TenantConfigurationRepositories.cs"));
        var arrangementHandler = File.ReadAllText(Path.Combine(root, "axionpro.application", "Features", "EmployeeCmd", "EmployeeWorkInfo", "Handlers", "EmployeeWorkArrangementHandler.cs"));
        var assignmentHandler = File.ReadAllText(Path.Combine(root, "axionpro.application", "Features", "EmployeeCmd", "EmployeeWorkInfo", "Handlers", "EmployeeLocationAssignmentHandler.cs"));

        Assert.Multiple(() =>
        {
            Assert.That(repository, Does.Contain("HasCoveringPrimaryLocationAssignmentAsync"));
            Assert.That(repository, Does.Contain("GetLocationAssignmentsForValidationAsync"));
            Assert.That(repository, Does.Contain("&& !x.IsSoftDeleted"));
            Assert.That(repository, Does.Contain("x.IsPrimary"));
            Assert.That(repository, Does.Contain("x.IsAttendanceAllowed"));
            Assert.That(repository, Does.Contain("x.EffectiveFrom <= effectiveFrom"));
            Assert.That(repository, Does.Contain("x.EffectiveTo.Value >= effectiveTo.Value"));
            Assert.That(arrangementHandler, Does.Contain("EmployeeWorkConfigurationRules.IsLocationTypeCompatible"));
            Assert.That(arrangementHandler, Does.Contain("WorkArrangementEmployeeLocationStartsLate"));
            Assert.That(arrangementHandler, Does.Contain("WorkArrangementEmployeeLocationEndsEarly"));
            Assert.That(arrangementHandler, Does.Contain("WorkArrangementEmployeeLocationMustBeOpenEnded"));
            Assert.That(arrangementHandler, Does.Contain("WorkArrangementEmployeeLocationAttendanceDisabled"));
            Assert.That(arrangementHandler, Does.Contain("WorkArrangementEmployeeLocationNotPrimary"));
            Assert.That(arrangementHandler, Does.Contain("dto.EffectiveFrom, dto.EffectiveTo, excludeId"));
            Assert.That(assignmentHandler, Does.Contain("WouldInvalidatePrimaryWorkArrangementAsync"));
            Assert.That(assignmentHandler, Does.Contain("dto.EffectiveFrom, dto.EffectiveTo, excludeId"));
        });
    }

    [Test]
    public void Database_model_and_deployment_script_allow_non_overlapping_schedules()
    {
        var root = FindRepositoryRoot();
        var context = File.ReadAllText(Path.Combine(root, "axionpro.persistance", "Data", "Context", "WorkforceDbContext.cs"));
        var script = File.ReadAllText(Path.Combine(root, "database-scripts", "AlignEmployeeWorkLocationAndArrangementValidation.sql"));

        Assert.Multiple(() =>
        {
            Assert.That(context, Does.Not.Contain("UX_EmployeeWorkArrangement_Current"));
            Assert.That(context, Does.Not.Contain("UX_EmployeeLocationAssignment_Primary"));
            Assert.That(script, Does.Contain("DROP INDEX IF EXISTS axionpro.\"UX_EmployeeWorkArrangement_Current\""));
            Assert.That(script, Does.Contain("IX_EmployeeWorkArrangement_Employee_EffectiveFrom"));
            Assert.That(script, Does.Contain("IX_EmployeeLocationAssignment_Primary_EffectiveFrom"));
            Assert.That(script, Does.Contain("CREATE EXTENSION IF NOT EXISTS btree_gist"));
            Assert.That(script, Does.Contain("EX_EmployeeLocationAssignment_Employee_Location_Window"));
            Assert.That(script, Does.Contain("EX_EmployeeLocationAssignment_Primary_Window"));
            Assert.That(script, Does.Contain("EX_EmployeeWorkArrangement_Employee_Window"));
            Assert.That(script, Does.Contain("WITH &&"));
        });
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        Assert.That(directory, Is.Not.Null, "Repository root containing AxionPro.sln was not found.");
        return directory!.FullName;
    }
}
