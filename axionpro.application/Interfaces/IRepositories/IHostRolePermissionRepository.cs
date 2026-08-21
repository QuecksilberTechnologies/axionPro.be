// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines read operations for effective Host role permissions.
// ============================================================================

using axionpro.application.DTOS.Host;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines read operations over permissions assigned to a Host role.
    /// </summary>
    public interface IHostRolePermissionRepository
    {
        #region Host Permission Operations

        /// <summary>
        /// Retrieves the active, non-deleted module-operation permissions effective for a Host role.
        /// </summary>
        /// <param name="hostRoleId">The Host-role primary key whose effective permissions are requested.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The effective Host permissions projected with module and operation display data.</returns>
        Task<List<HostUserPermissionResponseDTO>> GetHostUserPermissionsAsync(
            long hostRoleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves every persisted permission assignment for a Host role as tracked entities.
        /// </summary>
        /// <param name="hostRoleId">The Host-role primary key whose assignments are requested.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>All active and soft-deactivated assignments for the Host role.</returns>
        Task<List<HostRoleModuleAndPermission>> GetByHostRoleIdAsync(
            long hostRoleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds new Host-role permission assignments to the current unit-of-work change set.
        /// </summary>
        /// <param name="permissions">The new unique permission assignments to add.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task that completes when the assignments have been staged for persistence.</returns>
        Task BulkInsertAsync(
            List<HostRoleModuleAndPermission> permissions,
            CancellationToken cancellationToken = default);

        #endregion

        #region Host Access Queries

        /// <summary>
        /// Retrieves the current Host authorization rows that remain valid against active Host modules,
        /// active module-operation mappings, and active operations.
        /// </summary>
        /// <param name="hostRoleId">The validated current Host-role identifier.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The current Host module-operation permissions available for runtime access.</returns>
        Task<List<HostUserPermissionResponseDTO>> GetCurrentHostAccessPermissionsAsync(
            long hostRoleId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
