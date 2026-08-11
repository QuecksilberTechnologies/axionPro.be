// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines read operations for effective Host role permissions.
// ============================================================================

using axionpro.application.DTOS.Host;

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

        #endregion
    }
}
