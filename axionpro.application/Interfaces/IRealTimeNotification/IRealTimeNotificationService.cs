// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines transport-agnostic, tenant-safe real-time notification
//               publishing for application handlers.
// ============================================================================

namespace axionpro.application.Interfaces.IRealTimeNotification
{
    /// <summary>
    /// Publishes real-time notifications without coupling application code to a transport implementation.
    /// Live delivery is not a substitute for permanent notification storage or audit records.
    /// </summary>
    public interface IRealTimeNotificationService
    {
        #region Publishing

        /// <summary>Sends an event to authenticated connections that belong to one tenant.</summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToTenantAsync<T>(long tenantId, string eventName, T payload, CancellationToken cancellationToken = default);

        /// <summary>Sends an event to one authenticated tenant employee.</summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="userId">The trusted employee identifier represented by the access-token EmployeeId claim.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToUserAsync<T>(long tenantId, long userId, string eventName, T payload, CancellationToken cancellationToken = default);

        /// <summary>Sends an event to one authenticated Host user.</summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="hostUserId">The trusted Host-user identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToHostUserAsync<T>(long hostUserId, string eventName, T payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to tenant users with one authoritative role membership.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="roleId">The authoritative tenant role identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToTenantRoleAsync<T>(
            long tenantId,
            int roleId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to tenant users with any selected authoritative role membership.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="roleIds">The selected authoritative tenant role identifiers.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToTenantRolesAsync<T>(
            long tenantId,
            IReadOnlyCollection<int> roleIds,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to tenant users in one authoritative department.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="departmentId">The authoritative tenant department identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToDepartmentAsync<T>(
            long tenantId,
            int departmentId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to tenant users with one authoritative department-role membership.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="departmentId">The authoritative tenant department identifier.</param>
        /// <param name="roleId">The authoritative tenant role identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToDepartmentRoleAsync<T>(
            long tenantId,
            int departmentId,
            int roleId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to tenant users with any selected authoritative role membership in one department.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="tenantId">The trusted tenant identifier.</param>
        /// <param name="departmentId">The authoritative tenant department identifier.</param>
        /// <param name="roleIds">The selected authoritative tenant role identifiers.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToDepartmentRolesAsync<T>(
            long tenantId,
            int departmentId,
            IReadOnlyCollection<int> roleIds,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to all active tenant connections.
        /// Future Host command/API callers must run existing ValidateTenantUserRequestAsync(), applicable Host validation, and Host permission checks before invoking this capability.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToAllTenantsAsync<T>(
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an event to active tenant connections in one authoritative country.
        /// Country/state targeting is deferred until Tenant has an authoritative State relationship.
        /// Future Host command/API callers must run existing ValidateTenantUserRequestAsync(), applicable Host validation, and Host permission checks before invoking this capability.
        /// </summary>
        /// <typeparam name="T">The event payload type.</typeparam>
        /// <param name="countryId">The authoritative country identifier.</param>
        /// <param name="eventName">The client event name.</param>
        /// <param name="payload">The event payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the transport accepts the event.</returns>
        Task SendToCountryTenantsAsync<T>(
            int countryId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
