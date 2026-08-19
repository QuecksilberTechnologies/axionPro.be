// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Projects active tenant employee memberships from persistence
//               data for server-controlled real-time grouping.
// ============================================================================

using axionpro.application.Interfaces.IRealTimeNotification;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Realtime
{
    /// <summary>
    /// Resolves the current active tenant membership projection without returning tracked entities.
    /// </summary>
    public sealed class RealTimeConnectionMembershipResolver : IRealTimeConnectionMembershipResolver
    {
        #region Fields

        private readonly WorkforceDbContext _context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeConnectionMembershipResolver"/> class.
        /// </summary>
        /// <param name="context">The workforce database context.</param>
        public RealTimeConnectionMembershipResolver(WorkforceDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Membership resolution

        /// <inheritdoc />
        public async Task<RealTimeConnectionMembership?> ResolveTenantUserMembershipAsync(
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            if (tenantId <= 0 || employeeId <= 0)
                return null;

            var tenantEmployee = await (
                from employee in _context.Employees.AsNoTracking()
                join tenant in _context.Tenants.AsNoTracking() on employee.TenantId equals tenant.Id
                where employee.Id == employeeId &&
                      employee.TenantId == tenantId &&
                      employee.IsActive &&
                      employee.IsSoftDeleted != true &&
                      tenant.IsActive &&
                      tenant.IsSoftDeleted != true
                select new
                {
                    DepartmentId = _context.Departments
                        .AsNoTracking()
                        .Where(department =>
                            department.Id == employee.DepartmentId &&
                            department.TenantId == tenant.Id &&
                            department.IsActive &&
                            department.IsSoftDeleted != true)
                        .Select(department => (int?)department.Id)
                        .FirstOrDefault(),
                    CountryId = _context.Countries
                        .AsNoTracking()
                        .Where(country => country.Id == tenant.CountryId && country.IsActive == true)
                        .Select(country => (int?)country.Id)
                        .FirstOrDefault()
                }).FirstOrDefaultAsync(cancellationToken);

            if (tenantEmployee == null)
                return null;

            var roleIds = await _context.UserRoles
                .AsNoTracking()
                .Where(userRole =>
                    userRole.EmployeeId == employeeId &&
                    userRole.IsActive &&
                    userRole.IsSoftDeleted != true &&
                    userRole.RoleId.HasValue &&
                    userRole.Role != null &&
                    userRole.Role.IsActive &&
                    userRole.Role.IsSoftDeleted != true &&
                    userRole.Role.TenantId == tenantId)
                .Select(userRole => userRole.RoleId!.Value)
                .Distinct()
                .ToArrayAsync(cancellationToken);

            var departmentIds = tenantEmployee.DepartmentId is > 0
                ? new[] { tenantEmployee.DepartmentId.Value }
                : Array.Empty<int>();
            var departmentRoleMemberships = departmentIds
                .SelectMany(departmentId => roleIds.Select(roleId =>
                    new RealTimeDepartmentRoleMembership(departmentId, roleId)))
                .ToArray();

            // Country/state targeting is deferred until Tenant has an authoritative State relationship.
            return new RealTimeConnectionMembership(
                tenantId,
                employeeId,
                roleIds,
                departmentIds,
                departmentRoleMemberships,
                tenantEmployee.CountryId);
        }

        #endregion
    }
}
