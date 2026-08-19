// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Creates server-owned SignalR group and user identifiers for
//               tenant employees and Host users.
// ============================================================================

using axionpro.application.Interfaces.IRealTimeNotification;

namespace axionpro.api.Realtime.Identity
{
    /// <summary>Creates stable, tenant-safe SignalR identifiers from trusted server values.</summary>
    public static class SignalRGroupNameFactory
    {
        #region Group and user names

        /// <summary>Gets the group containing all authenticated connections for a tenant.</summary>
        public static string Tenant(long tenantId)
        {
            EnsurePositiveIdentifier(tenantId, nameof(tenantId));
            return $"tenant:{tenantId}";
        }

        /// <summary>Gets the group containing all authenticated Host-user connections.</summary>
        public static string Host() => "host";

        /// <summary>Gets the unique identifier for an authenticated tenant employee.</summary>
        public static string TenantUser(long tenantId, long userId)
        {
            EnsurePositiveIdentifier(userId, nameof(userId));
            return $"{Tenant(tenantId)}:user:{userId}";
        }

        /// <summary>Gets the unique identifier for an authenticated Host user.</summary>
        public static string HostUser(long hostUserId)
        {
            EnsurePositiveIdentifier(hostUserId, nameof(hostUserId));
            return $"host:user:{hostUserId}";
        }

        /// <summary>
        /// Gets the group containing tenant connections with one active role.
        /// </summary>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="roleId">The authoritative role identifier.</param>
        /// <returns>The tenant role group name.</returns>
        public static string TenantRole(long tenantId, int roleId)
        {
            EnsurePositiveIdentifier(roleId, nameof(roleId));
            // Security: TenantId is mandatory so equal role IDs cannot cross tenant boundaries.
            return $"{Tenant(tenantId)}:role:{roleId}";
        }

        /// <summary>
        /// Gets the group containing tenant connections in one active department.
        /// </summary>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="departmentId">The authoritative department identifier.</param>
        /// <returns>The tenant department group name.</returns>
        public static string TenantDepartment(long tenantId, int departmentId)
        {
            EnsurePositiveIdentifier(departmentId, nameof(departmentId));
            // Security: TenantId is mandatory so equal department IDs cannot cross tenant boundaries.
            return $"{Tenant(tenantId)}:department:{departmentId}";
        }

        /// <summary>
        /// Gets the group containing tenant connections with one proven department-role membership.
        /// </summary>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="departmentId">The authoritative department identifier.</param>
        /// <param name="roleId">The authoritative role identifier.</param>
        /// <returns>The tenant department-role group name.</returns>
        public static string TenantDepartmentRole(long tenantId, int departmentId, int roleId)
        {
            return $"{TenantDepartment(tenantId, departmentId)}:role:{EnsurePositiveIdentifier(roleId, nameof(roleId))}";
        }

        /// <summary>
        /// Gets the group containing authenticated connections from every active tenant.
        /// </summary>
        /// <returns>The all-tenants group name.</returns>
        public static string AllTenants()
        {
            // Security: this group excludes Host users and is used instead of Clients.All for tenant data.
            return "tenants:all";
        }

        /// <summary>
        /// Gets the group containing tenant connections in one country.
        /// Country/state targeting is deferred until Tenant has an authoritative State relationship.
        /// </summary>
        /// <param name="countryId">The authoritative country identifier.</param>
        /// <returns>The country tenant group name.</returns>
        public static string CountryTenants(int countryId)
        {
            EnsurePositiveIdentifier(countryId, nameof(countryId));
            return $"tenants:country:{countryId}";
        }

        /// <summary>
        /// Creates all tenant-only groups proven by an authoritative membership projection.
        /// </summary>
        /// <param name="membership">The authoritative membership projection.</param>
        /// <returns>A distinct collection of server-owned group names.</returns>
        public static IReadOnlyCollection<string> GetTenantConnectionGroups(RealTimeConnectionMembership membership)
        {
            ArgumentNullException.ThrowIfNull(membership);

            // Security: this accepts only the backend membership projection; clients never supply group identifiers.

            var groups = new HashSet<string>(StringComparer.Ordinal)
            {
                Tenant(membership.TenantId),
                AllTenants()
            };

            foreach (var roleId in membership.RoleIds.Where(roleId => roleId > 0).Distinct())
                groups.Add(TenantRole(membership.TenantId, roleId));

            foreach (var departmentId in membership.DepartmentIds.Where(departmentId => departmentId > 0).Distinct())
                groups.Add(TenantDepartment(membership.TenantId, departmentId));

            foreach (var departmentRole in membership.DepartmentRoleMemberships
                         .Where(item => item.DepartmentId > 0 && item.RoleId > 0)
                         .DistinctBy(item => (item.DepartmentId, item.RoleId)))
            {
                groups.Add(TenantDepartmentRole(
                    membership.TenantId,
                    departmentRole.DepartmentId,
                    departmentRole.RoleId));
            }

            if (membership.CountryId is > 0)
                groups.Add(CountryTenants(membership.CountryId.Value));

            return groups.ToArray();
        }

        #endregion

        #region Validation

        /// <summary>Prevents invalid values from becoming server-owned group or user identifiers.</summary>
        /// <param name="identifier">The identifier to validate.</param>
        /// <param name="parameterName">The source parameter name.</param>
        private static long EnsurePositiveIdentifier(long identifier, string parameterName)
        {
            if (identifier <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "A positive identifier is required.");

            return identifier;
        }

        #endregion
    }
}
