using axionpro.application.DTOS.Dashboard;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

public sealed class SuperAdminDashboardRepository(
    WorkforceDbContext context,
    IIdEncoderService idEncoderService) : ISuperAdminDashboardRepository
{
    public async Task<DashboardSummaryDTO> GetSummaryAsync(long tenantId, DateTime today, CancellationToken cancellationToken)
    {
        var employees = context.Employees.AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        var total = await employees.CountAsync(cancellationToken);
        var newHires = await employees.CountAsync(x => x.DateOfOnBoarding.HasValue
            && x.DateOfOnBoarding.Value.Year == today.Year
            && x.DateOfOnBoarding.Value.Month == today.Month, cancellationToken);
        return new DashboardSummaryDTO(total, newHires, 10, 3);
    }

    public async Task<EmployeeOverviewDTO> GetEmployeeOverviewAsync(long tenantId, DateOnly today, CancellationToken cancellationToken)
    {
        var employees = context.Employees.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        var total = await employees.CountAsync(cancellationToken);
        var active = await employees.CountAsync(x => x.IsActive, cancellationToken);
        var onLeave = await context.LeaveRequests.AsNoTracking().CountAsync(x => x.TenantId == tenantId
            && x.ApprovedById.HasValue && !x.CancellationDate.HasValue
            && x.FromDate <= today && x.ToDate >= today, cancellationToken);
        return new EmployeeOverviewDTO(total, active, total - active, onLeave);
    }

    public async Task<IReadOnlyList<DashboardEmployeeDTO>> GetUpcomingBirthdaysAsync(long tenantId, DateTime today, int days, CancellationToken cancellationToken)
    {
        var employees = await BaseEmployeeQuery(tenantId).Where(x => x.Employee.DateOfBirth.HasValue).ToListAsync(cancellationToken);
        return employees.Select(x => new { Row = x, Next = NextOccurrence(x.Employee.DateOfBirth!.Value, today) })
            .Where(x => x.Next <= today.Date.AddDays(days)).OrderBy(x => x.Next).ThenBy(x => x.Row.Employee.FirstName)
            .Select(x => MapEmployee(x.Row, x.Next)).ToList();
    }

    public async Task<IReadOnlyList<DashboardEmployeeDTO>> GetRecentOnboardingAsync(long tenantId, int limit, CancellationToken cancellationToken)
    {
        var rows = await BaseEmployeeQuery(tenantId).Where(x => x.Employee.DateOfOnBoarding.HasValue)
            .OrderByDescending(x => x.Employee.DateOfOnBoarding).Take(limit).ToListAsync(cancellationToken);
        return rows.Select(x => MapEmployee(x, x.Employee.DateOfOnBoarding)).ToList();
    }

    public async Task<IReadOnlyList<DepartmentHeadcountDTO>> GetDepartmentHeadcountAsync(
        long tenantId,
        CancellationToken cancellationToken)
    {
        return await context.Departments.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted)
            .GroupJoin(context.Employees.AsNoTracking().Where(x => x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted),
                department => department.Id,
                employee => employee.DepartmentId,
                (department, employees) => new DepartmentHeadcountDTO(
                    department.Id,
                    department.DepartmentName,
                    employees.Count()))
            .OrderByDescending(x => x.Headcount).ThenBy(x => x.DepartmentName).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DashboardLeaveDTO>> GetCurrentlyOnLeaveAsync(long tenantId, DateOnly today, CancellationToken cancellationToken)
    {
        var rows = await (from leave in context.LeaveRequests.AsNoTracking()
                          join employee in context.Employees.AsNoTracking() on leave.EmployeeId equals employee.Id
                          join department in context.Departments.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted)
                              on employee.DepartmentId equals (int?)department.Id into departments
                          from department in departments.DefaultIfEmpty()
                          where leave.TenantId == tenantId && employee.TenantId == tenantId && !employee.IsSoftDeleted
                              && leave.ApprovedById.HasValue && !leave.CancellationDate.HasValue
                              && leave.FromDate <= today && leave.ToDate >= today
                          orderby leave.ToDate, employee.FirstName
                          select new { Employee = employee, DepartmentName = department == null ? null : department.DepartmentName, leave.FromDate, leave.ToDate }).ToListAsync(cancellationToken);
        return rows.Select(x => new DashboardLeaveDTO(
            Encode(x.Employee.Id),
            FullName(x.Employee.FirstName, x.Employee.MiddleName, x.Employee.LastName),
            x.DepartmentName,
            x.FromDate,
            x.ToDate,
            x.Employee.MobileNumber)).ToList();
    }

    public async Task<LocationOverviewDTO> GetLocationsAsync(long tenantId, int limit, CancellationToken cancellationToken)
    {
        var query = context.TenantLocations.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        var total = await query.CountAsync(cancellationToken);
        var active = await query.CountAsync(x => x.IsActive, cancellationToken);
        var headOffice = await query.CountAsync(x => x.IsHeadOffice, cancellationToken);
        var rows = await query.OrderByDescending(x => x.IsHeadOffice).ThenBy(x => x.LocationName).Take(limit)
            .Select(x => new DashboardLocationDTO(x.Id, x.LocationCode, x.LocationName, x.LocationType, x.IsHeadOffice, x.IsActive)).ToListAsync(cancellationToken);
        return new LocationOverviewDTO(total, active, headOffice, rows);
    }

    private IQueryable<EmployeeDashboardRow> BaseEmployeeQuery(long tenantId) =>
        from employee in context.Employees.AsNoTracking()
        join department in context.Departments.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted)
            on employee.DepartmentId equals (int?)department.Id into departments
        from department in departments.DefaultIfEmpty()
        join designation in context.Designations.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted)
            on employee.DesignationId equals (int?)designation.Id into designations
        from designation in designations.DefaultIfEmpty()
        let roleName = (from userRole in context.UserRoles.AsNoTracking()
                        join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                        where userRole.EmployeeId == employee.Id && userRole.IsActive && userRole.IsSoftDeleted != true
                            && role.TenantId == tenantId && role.IsActive && role.IsSoftDeleted != true
                        orderby userRole.IsPrimaryRole descending, userRole.Id
                        select role.RoleName).FirstOrDefault()
        where employee.TenantId == tenantId && employee.IsActive && !employee.IsSoftDeleted
        select new EmployeeDashboardRow(employee, department == null ? null : department.DepartmentName, designation == null ? null : designation.DesignationName, roleName);

    private DashboardEmployeeDTO MapEmployee(EmployeeDashboardRow row, DateTime? date)
    {
        return new DashboardEmployeeDTO(
            Encode(row.Employee.Id),
            FullName(row.Employee.FirstName, row.Employee.MiddleName, row.Employee.LastName),
            row.DepartmentName,
            row.DesignationName,
            row.RoleName,
            date);
    }

    private string Encode(long id)
    {
        return idEncoderService.EncodeId_long(id, string.Empty);
    }

    private static string FullName(string? first, string? middle, string? last)
    {
        return string.Join(' ', new[] { first, middle, last }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static DateTime NextOccurrence(DateTime birthday, DateTime today)
    {
        var day = Math.Min(birthday.Day, DateTime.DaysInMonth(today.Year, birthday.Month));
        var next = new DateTime(today.Year, birthday.Month, day);
        if (next < today.Date)
        {
            var year = today.Year + 1;
            next = new DateTime(
                year,
                birthday.Month,
                Math.Min(birthday.Day, DateTime.DaysInMonth(year, birthday.Month)));
        }

        return next;
    }
    private sealed record EmployeeDashboardRow(
        axionpro.domain.Entity.Employee Employee,
        string? DepartmentName,
        string? DesignationName,
        string? RoleName);
}
