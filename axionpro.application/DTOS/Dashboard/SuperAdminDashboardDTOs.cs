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
