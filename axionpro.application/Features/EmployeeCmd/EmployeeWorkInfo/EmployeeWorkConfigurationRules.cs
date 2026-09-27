// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Keeps work-mode/location and effective-window rules consistent.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo;

/// <summary>Defines deterministic rules shared by work-location and work-arrangement validation.</summary>
public static class EmployeeWorkConfigurationRules
{
    /// <summary>Returns whether an arrangement requires a primary tenant location.</summary>
    public static bool RequiresPrimaryLocation(WorkMode workMode) =>
        workMode is WorkMode.Office or WorkMode.Hybrid or WorkMode.ClientSite;

    /// <summary>Returns whether a daily pattern or override requires an execution location.</summary>
    public static bool RequiresExecutionLocation(WorkMode workMode) =>
        workMode is WorkMode.Office or WorkMode.Field or WorkMode.ClientSite;

    /// <summary>Returns whether the mode must not carry a tenant location.</summary>
    public static bool DisallowsLocation(WorkMode workMode) => workMode == WorkMode.WorkFromHome;

    /// <summary>Returns whether a work mode is concrete enough for a daily override.</summary>
    public static bool IsConcreteWorkMode(WorkMode workMode) =>
        workMode is WorkMode.Office or WorkMode.WorkFromHome or WorkMode.Field or WorkMode.ClientSite;

    /// <summary>Returns whether an arrangement supports fixed weekday pattern rows.</summary>
    public static bool SupportsWeekdayPattern(WorkMode workMode, short? hybridType) =>
        workMode != WorkMode.Hybrid || hybridType != (short)HybridType.Flexible;

    /// <summary>Returns whether a weekday mode is valid for its arrangement.</summary>
    public static bool IsPatternModeAllowed(WorkMode arrangementWorkMode, WorkMode patternWorkMode) =>
        arrangementWorkMode == WorkMode.Hybrid
            ? IsConcreteWorkMode(patternWorkMode)
            : arrangementWorkMode == patternWorkMode;

    /// <summary>Checks whether a work arrangement can execute under a typed Attendance policy configuration.</summary>
    public static bool IsAllowedByAttendancePolicy(
        WorkMode workMode,
        long? primaryTenantLocationId,
        AttendancePolicyVersionConfiguration configuration)
    {
        var scope = (AttendanceLocationScope)configuration.AttendanceLocationScope;
        if (workMode == WorkMode.WorkFromHome)
        {
            return configuration.AllowWorkFromHome
                && !primaryTenantLocationId.HasValue;
        }

        return scope != AttendanceLocationScope.RemoteAnywhere
            && primaryTenantLocationId.HasValue;
    }

    /// <summary>Returns whether a physical location type can act as primary for the selected work mode.</summary>
    public static bool IsLocationTypeCompatible(WorkMode workMode, TenantLocationType locationType) => workMode switch
    {
        WorkMode.ClientSite => locationType == TenantLocationType.ClientSite,
        WorkMode.Office or WorkMode.Hybrid => locationType is TenantLocationType.HeadOffice
            or TenantLocationType.Branch
            or TenantLocationType.Office
            or TenantLocationType.Plant
            or TenantLocationType.Warehouse
            or TenantLocationType.Campus
            or TenantLocationType.RemoteOffice,
        WorkMode.Field => locationType is TenantLocationType.ClientSite or TenantLocationType.ProjectSite,
        WorkMode.WorkFromHome => false,
        _ => false
    };

    /// <summary>Returns whether two inclusive date windows overlap; a null end date is open-ended.</summary>
    public static bool EffectiveWindowsOverlap(DateOnly firstFrom, DateOnly? firstTo, DateOnly secondFrom, DateOnly? secondTo) =>
        (!firstTo.HasValue || secondFrom <= firstTo.Value)
        && (!secondTo.HasValue || firstFrom <= secondTo.Value);

    /// <summary>Returns whether an assignment covers an arrangement's complete inclusive window.</summary>
    public static bool AssignmentCoversArrangement(DateOnly assignmentFrom, DateOnly? assignmentTo, DateOnly arrangementFrom, DateOnly? arrangementTo) =>
        assignmentFrom <= arrangementFrom
        && (arrangementTo.HasValue
            ? !assignmentTo.HasValue || assignmentTo.Value >= arrangementTo.Value
            : !assignmentTo.HasValue);
}
