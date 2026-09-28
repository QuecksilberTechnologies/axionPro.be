using axionpro.application.DTOS.Dashboard;

namespace axionpro.application.Interfaces.IRepositories;

public interface ISuperAdminDashboardRepository
{
    Task<DashboardSummaryDTO> GetSummaryAsync(long tenantId, DateTime today, CancellationToken cancellationToken);
    Task<EmployeeOverviewDTO> GetEmployeeOverviewAsync(long tenantId, DateOnly today, CancellationToken cancellationToken);
    Task<IReadOnlyList<DashboardEmployeeDTO>> GetUpcomingBirthdaysAsync(long tenantId, DateTime today, int days, CancellationToken cancellationToken);
    Task<IReadOnlyList<DashboardEmployeeDTO>> GetRecentOnboardingAsync(long tenantId, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<DepartmentHeadcountDTO>> GetDepartmentHeadcountAsync(long tenantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<DashboardLeaveDTO>> GetCurrentlyOnLeaveAsync(long tenantId, DateOnly today, CancellationToken cancellationToken);
    Task<LocationOverviewDTO> GetLocationsAsync(long tenantId, int limit, CancellationToken cancellationToken);
}
