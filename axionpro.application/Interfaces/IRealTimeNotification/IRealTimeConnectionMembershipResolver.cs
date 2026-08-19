// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines authoritative, transport-neutral tenant connection
//               membership data for real-time group assignment.
// ============================================================================

namespace axionpro.application.Interfaces.IRealTimeNotification
{
    /// <summary>
    /// Resolves the current backend-authoritative memberships required to group a tenant user connection.
    /// </summary>
    public interface IRealTimeConnectionMembershipResolver
    {
        #region Membership resolution

        /// <summary>
        /// Resolves active tenant, employee, role, department, and country memberships for one connection.
        /// Country/state targeting remains deferred until Tenant has an authoritative State relationship.
        /// </summary>
        /// <param name="tenantId">The tenant identifier already validated from the authenticated token.</param>
        /// <param name="employeeId">The employee identifier already validated from the authenticated token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The current membership projection, or <see langword="null"/> when the tenant employee is no longer active.
        /// </returns>
        Task<RealTimeConnectionMembership?> ResolveTenantUserMembershipAsync(
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default);

        #endregion
    }

    /// <summary>
    /// Contains only the IDs required for server-controlled real-time group membership.
    /// </summary>
    public sealed class RealTimeConnectionMembership
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeConnectionMembership"/> class.
        /// </summary>
        /// <param name="tenantId">The active tenant identifier.</param>
        /// <param name="employeeId">The active employee identifier.</param>
        /// <param name="roleIds">The employee's distinct active tenant role identifiers.</param>
        /// <param name="departmentIds">The employee's distinct active tenant department identifiers.</param>
        /// <param name="departmentRoleMemberships">The employee's authoritative department-role intersections.</param>
        /// <param name="countryId">The active tenant country identifier, when available.</param>
        public RealTimeConnectionMembership(
            long tenantId,
            long employeeId,
            IReadOnlyCollection<int> roleIds,
            IReadOnlyCollection<int> departmentIds,
            IReadOnlyCollection<RealTimeDepartmentRoleMembership> departmentRoleMemberships,
            int? countryId)
        {
            TenantId = tenantId;
            EmployeeId = employeeId;
            RoleIds = roleIds;
            DepartmentIds = departmentIds;
            DepartmentRoleMemberships = departmentRoleMemberships;
            CountryId = countryId;
        }

        /// <summary>Gets the active tenant identifier.</summary>
        public long TenantId { get; }

        /// <summary>Gets the active employee identifier.</summary>
        public long EmployeeId { get; }

        /// <summary>Gets the employee's distinct active tenant role identifiers.</summary>
        public IReadOnlyCollection<int> RoleIds { get; }

        /// <summary>Gets the employee's distinct active tenant department identifiers.</summary>
        public IReadOnlyCollection<int> DepartmentIds { get; }

        /// <summary>Gets only department-role intersections proven for this employee.</summary>
        public IReadOnlyCollection<RealTimeDepartmentRoleMembership> DepartmentRoleMemberships { get; }

        /// <summary>Gets the active tenant country identifier, when available.</summary>
        public int? CountryId { get; }

    }

    /// <summary>
    /// Represents one department and role combination currently held by the same tenant employee.
    /// </summary>
    public sealed class RealTimeDepartmentRoleMembership
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeDepartmentRoleMembership"/> class.
        /// </summary>
        /// <param name="departmentId">The active tenant department identifier.</param>
        /// <param name="roleId">The active tenant role identifier.</param>
        public RealTimeDepartmentRoleMembership(int departmentId, int roleId)
        {
            DepartmentId = departmentId;
            RoleId = roleId;
        }

        /// <summary>Gets the active tenant department identifier.</summary>
        public int DepartmentId { get; }

        /// <summary>Gets the active tenant role identifier.</summary>
        public int RoleId { get; }
    }
}
