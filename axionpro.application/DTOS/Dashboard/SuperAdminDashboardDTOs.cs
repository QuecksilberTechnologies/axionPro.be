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

/// <summary>Describes the authenticated persona and the widgets its dashboard may request.</summary>
public sealed record DashboardConfigurationDTO(
    string RoleTypeCode,
    string RoleTypeName,
    IReadOnlyList<DashboardWidgetDefinitionDTO> Widgets);

/// <summary>Describes one independently loadable dashboard widget.</summary>
public sealed record DashboardWidgetDefinitionDTO(
    string Code,
    string Title,
    int DisplayOrder,
    bool IsPlaceholder);

/// <summary>Provides a generic widget payload without exposing internal numeric role identifiers.</summary>
public sealed record DashboardWidgetDTO(
    string Code,
    string Title,
    bool IsPlaceholder,
    string Source,
    IReadOnlyList<DashboardMetricDTO> Metrics,
    IReadOnlyList<DashboardWidgetItemDTO> Items);

/// <summary>Represents one dashboard metric.</summary>
public sealed record DashboardMetricDTO(string Code, string Label, decimal Value, string? Unit = null);

/// <summary>Represents one lightweight dashboard list item.</summary>
public sealed record DashboardWidgetItemDTO(
    string Code,
    string Title,
    string? Subtitle,
    string? Status,
    string? Date);
