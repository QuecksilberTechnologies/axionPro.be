namespace axionpro.application.DTOS.Dashboard;

public sealed record DashboardSummaryDTO(int TotalEmployees, int NewHiresThisMonth, int OpenPositions, int PendingApprovals);
public sealed record EmployeeOverviewDTO(int Total, int Active, int Inactive, int OnLeave);
public sealed record DashboardEmployeeDTO(string EmployeeId, string EmployeeName, string? DepartmentName, string? DesignationName, string? RoleName, DateTime? Date);
public sealed record DashboardLeaveDTO(string EmployeeId, string EmployeeName, string? DepartmentName, DateOnly FromDate, DateOnly ToDate, string? MobileNumber);
public sealed record DashboardLocationDTO(long Id, string Code, string Name, short LocationType, bool IsHeadOffice, bool IsActive);
public sealed record LocationOverviewDTO(int Total, int Active, int HeadOffice, IReadOnlyList<DashboardLocationDTO> Locations);
public sealed record DepartmentHeadcountDTO(int DepartmentId, string DepartmentName, int Headcount);
public sealed record StorageCategoryDTO(string Name, decimal UsedGigabytes);
public sealed record StorageStatusDTO(decimal CapacityGigabytes, decimal UsedGigabytes, IReadOnlyList<StorageCategoryDTO> Categories);
public sealed record HiringStageDTO(string Stage, int Count);
public sealed record HiringPipelineDTO(int TotalApplicants, IReadOnlyList<HiringStageDTO> Stages);

/// <summary>Represents one dashboard metric.</summary>
public sealed record DashboardMetricDTO(string Code, string Label, decimal Value, string? Unit = null);

/// <summary>Represents one lightweight dashboard list item.</summary>
public sealed record DashboardWidgetItemDTO(
    string Code,
    string Title,
    string? Subtitle,
    string? Status,
    string? Date);

public sealed record DashboardSectionDTO<T>(
    T Data,
    string Source,
    bool IsPlaceholder,
    IReadOnlyList<string>? PlaceholderFields = null);

public sealed record DashboardMetricCollectionDTO(IReadOnlyList<DashboardMetricDTO> Metrics);

public sealed record RecentExitedEmployeeDTO(
    string EmployeeId,
    string EmployeeName,
    string? DepartmentName,
    string? DesignationName,
    DateTime DateOfExit);

public sealed record NoticePeriodEmployeeDTO(
    string Code,
    string EmployeeName,
    string? DepartmentName,
    string? DesignationName,
    DateOnly ExpectedLastWorkingDate,
    int NoticeDaysRemaining,
    string Status);

public sealed record TenantAdministratorDashboardDTO(
    DashboardSectionDTO<DashboardSummaryDTO> Summary,
    DashboardSectionDTO<EmployeeOverviewDTO> EmployeeOverview,
    DashboardSectionDTO<IReadOnlyList<DashboardLeaveDTO>> CurrentlyOnLeave,
    DashboardSectionDTO<IReadOnlyList<NoticePeriodEmployeeDTO>> EmployeesOnNotice,
    DashboardSectionDTO<IReadOnlyList<RecentExitedEmployeeDTO>> ExitedEmployees,
    DashboardSectionDTO<LocationOverviewDTO> Locations,
    DashboardSectionDTO<IReadOnlyList<DashboardEmployeeDTO>> UpcomingBirthdays,
    DashboardSectionDTO<IReadOnlyList<DashboardEmployeeDTO>> RecentOnboarding,
    DashboardSectionDTO<IReadOnlyList<DepartmentHeadcountDTO>> DepartmentHeadcount,
    DashboardSectionDTO<StorageStatusDTO> StorageStatus,
    DashboardSectionDTO<HiringPipelineDTO> HiringPipeline);

public sealed record PeopleManagerDashboardDTO(
    DashboardSectionDTO<DashboardMetricCollectionDTO> TeamSummary,
    DashboardSectionDTO<DashboardMetricCollectionDTO> TeamAttendance,
    DashboardSectionDTO<IReadOnlyList<NoticePeriodEmployeeDTO>> TeamMembersOnNotice,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> TeamLeave,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> TeamBirthdays,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> RecentTeamOnboarding);

public sealed record WorkforceUserDashboardDTO(
    DashboardSectionDTO<DashboardMetricCollectionDTO> MyProfile,
    DashboardSectionDTO<DashboardMetricCollectionDTO> MyAttendance,
    DashboardSectionDTO<DashboardMetricCollectionDTO> MyLeave,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> MyTasks,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> MyDocuments,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> Announcements);

public sealed record ExternalUserDashboardDTO(
    DashboardSectionDTO<DashboardMetricCollectionDTO> ClientSummary,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> OpenTickets,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> RecentActivity,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> ClientSiteEmployees,
    DashboardSectionDTO<IReadOnlyList<DashboardWidgetItemDTO>> Documents);

public sealed record DashboardDataDTO(
    string RoleTypeCode,
    string RoleTypeName,
    DateTime GeneratedAtUtc,
    TenantAdministratorDashboardDTO? TenantAdministrator,
    PeopleManagerDashboardDTO? PeopleManager,
    WorkforceUserDashboardDTO? WorkforceUser,
    ExternalUserDashboardDTO? ExternalUser);
