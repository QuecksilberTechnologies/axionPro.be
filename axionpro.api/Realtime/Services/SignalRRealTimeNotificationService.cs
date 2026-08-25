// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Implements application real-time notification publishing with
//               authenticated, tenant-safe SignalR recipients.
// ============================================================================

using axionpro.api.Realtime.Hubs;
using axionpro.api.Realtime.Identity;
using axionpro.application.Interfaces.IRealTimeNotification;
using Microsoft.AspNetCore.SignalR;

namespace axionpro.api.Realtime.Services
{
    /// <summary>Publishes application events to server-derived SignalR groups and users.</summary>
    public sealed class SignalRRealTimeNotificationService : IRealTimeNotificationService
    {
        #region Fields

        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRRealTimeNotificationService> _logger;

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="SignalRRealTimeNotificationService"/> class.</summary>
        /// <param name="hubContext">The authenticated SignalR hub context.</param>
        /// <param name="logger">Logs delivery attempts without payload or token data.</param>
        public SignalRRealTimeNotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRRealTimeNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        #endregion

        #region Publishing

        /// <inheritdoc />
        public async Task SendToTenantAsync<T>(
            long tenantId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var tenantGroup = SignalRGroupNameFactory.Tenant(tenantId);

            // Security: Clients.All must never carry tenant business data because it crosses tenant boundaries.
            await SendAsync(_hubContext.Clients.Group(tenantGroup), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a tenant group. TenantId: {TenantId}", tenantId);
        }

        /// <inheritdoc />
        public async Task SendToUserAsync<T>(
            long tenantId,
            long userId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var userIdentifier = SignalRGroupNameFactory.TenantUser(tenantId, userId);
            await SendAsync(_hubContext.Clients.User(userIdentifier), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a tenant user.");
        }

        /// <inheritdoc />
        public async Task SendToHostUserAsync<T>(
            long hostUserId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var userIdentifier = SignalRGroupNameFactory.HostUser(hostUserId);
            await SendAsync(_hubContext.Clients.User(userIdentifier), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a Host user.");
        }

        /// <inheritdoc />
        public async Task SendToTenantRoleAsync<T>(
            long tenantId,
            int roleId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var roleGroup = SignalRGroupNameFactory.TenantRole(tenantId, roleId);
            await SendAsync(_hubContext.Clients.Group(roleGroup), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a tenant role group.");
        }

        /// <inheritdoc />
        public async Task SendToTenantRolesAsync<T>(
            long tenantId,
            IReadOnlyCollection<int> roleIds,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var roleGroups = CreateTenantRoleGroups(tenantId, roleIds);
            await SendAsync(_hubContext.Clients.Groups(roleGroups), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to {GroupCount} tenant role groups.", roleGroups.Count);
        }

        /// <inheritdoc />
        public async Task SendToDepartmentAsync<T>(
            long tenantId,
            int departmentId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var departmentGroup = SignalRGroupNameFactory.TenantDepartment(tenantId, departmentId);
            await SendAsync(_hubContext.Clients.Group(departmentGroup), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a tenant department group.");
        }

        /// <inheritdoc />
        public async Task SendToDepartmentRoleAsync<T>(
            long tenantId,
            int departmentId,
            int roleId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var departmentRoleGroup = SignalRGroupNameFactory.TenantDepartmentRole(tenantId, departmentId, roleId);
            await SendAsync(_hubContext.Clients.Group(departmentRoleGroup), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a tenant department-role group.");
        }

        /// <inheritdoc />
        public async Task SendToDepartmentRolesAsync<T>(
            long tenantId,
            int departmentId,
            IReadOnlyCollection<int> roleIds,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            var departmentRoleGroups = CreateDepartmentRoleGroups(tenantId, departmentId, roleIds);
            await SendAsync(_hubContext.Clients.Groups(departmentRoleGroups), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to {GroupCount} tenant department-role groups.", departmentRoleGroups.Count);
        }

        /// <inheritdoc />
        public async Task SendToAllTenantsAsync<T>(
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            // Security: a future Host command/API must run ValidateTenantUserRequestAsync(), Host validation, and permission checks first.
            await SendAsync(
                _hubContext.Clients.Group(SignalRGroupNameFactory.AllTenants()),
                eventName,
                payload,
                cancellationToken);
            _logger.LogDebug("Published SignalR event to the all-tenants group.");
        }

        /// <inheritdoc />
        public async Task SendToCountryTenantsAsync<T>(
            int countryId,
            string eventName,
            T payload,
            CancellationToken cancellationToken = default)
        {
            // Security: a future Host command/API must run ValidateTenantUserRequestAsync(), Host validation, and permission checks first.
            var countryGroup = SignalRGroupNameFactory.CountryTenants(countryId);
            await SendAsync(_hubContext.Clients.Group(countryGroup), eventName, payload, cancellationToken);
            _logger.LogDebug("Published SignalR event to a country tenant group.");
        }

        #endregion

        #region Private helpers

        /// <summary>Validates the event boundary and sends without broadcasting to unrelated users.</summary>
        private static Task SendAsync<T>(
            IClientProxy client,
            string eventName,
            T payload,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("An event name is required.", nameof(eventName));

            return client.SendAsync(eventName, payload, cancellationToken);
        }

        /// <summary>
        /// Creates distinct tenant role groups from structurally valid authoritative role identifiers.
        /// </summary>
        private static IReadOnlyList<string> CreateTenantRoleGroups(long tenantId, IReadOnlyCollection<int> roleIds)
        {
            return ValidateAndDistinctRoleIds(roleIds)
                .Select(roleId => SignalRGroupNameFactory.TenantRole(tenantId, roleId))
                .ToArray();
        }

        /// <summary>
        /// Creates distinct tenant department-role groups from structurally valid authoritative role identifiers.
        /// </summary>
        private static IReadOnlyList<string> CreateDepartmentRoleGroups(
            long tenantId,
            int departmentId,
            IReadOnlyCollection<int> roleIds)
        {
            return ValidateAndDistinctRoleIds(roleIds)
                .Select(roleId => SignalRGroupNameFactory.TenantDepartmentRole(tenantId, departmentId, roleId))
                .ToArray();
        }

        /// <summary>
        /// Rejects missing or non-positive role identifiers and removes duplicate selected roles.
        /// </summary>
        private static IReadOnlyCollection<int> ValidateAndDistinctRoleIds(IReadOnlyCollection<int> roleIds)
        {
            ArgumentNullException.ThrowIfNull(roleIds);

            var distinctRoleIds = roleIds.Distinct().ToArray();
            if (distinctRoleIds.Length == 0)
                throw new ArgumentException("At least one role identifier is required.", nameof(roleIds));

            if (distinctRoleIds.Any(roleId => roleId <= 0))
                throw new ArgumentOutOfRangeException(nameof(roleIds), "Role identifiers must be positive.");

            return distinctRoleIds;
        }

        #endregion
    }
}
