// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines common current-request validation operations.
// ================================================================

using axionpro.application.Common.Models.Security;

namespace axionpro.application.Interfaces.ICommonRequest
{
    /// <summary>
    /// Defines shared validation operations for the current authenticated request.
    /// </summary>
    public interface ICommonRequestService
    {
        /// <summary>
        /// Resolves the code of an active Module from its identifier.
        /// </summary>
        /// <param name="moduleId">The Module identifier supplied by the request.</param>
        /// <returns>The active module code, or <see langword="null"/> when the module is invalid or inactive.</returns>
        Task<string?> GetActiveModuleCodeAsync(int moduleId);

        /// <summary>
        /// Validates the current authenticated tenant request and resolves the trusted tenant, employee, and role context.
        /// </summary>
        /// <returns>The validated tenant request context.</returns>
        Task<CommonDecodedResult> ValidateTenantUserRequestAsync();
        /// <summary>
        /// Validates the current authenticated Host or Tenant request by delegating to the established principal-specific validation path.
        /// </summary>
        /// <returns>The trusted authenticated principal context.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the authenticated token does not represent a valid active Host or Tenant principal.</exception>
        Task<AuthenticatedRequestContext> ValidateAuthenticatedRequestAsync();

        /// <summary>
        /// Validates that the current JWT belongs to an active Host user with an active Host role.
        /// </summary>
        /// <returns>The validated Host user identifier.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current request is not authenticated as a valid Host user.</exception>
        Task<long> ValidateHostUserRequestAsync();

        /// <summary>
        /// Validates the current Host JWT and requires the current Host user to hold the verified Super Admin role.
        /// </summary>
        /// <returns>The trusted Host context containing the principal type and current database role.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current request is not an authenticated Host Super Admin.</exception>
        Task<HostUserRequestContext> ValidateHostSuperAdminRequestAsync();

        /// <summary>
        /// Validates the current Host JWT and returns the trusted Host context required for per-request runtime permission checks.
        /// </summary>
        /// <returns>The validated Host user identifier, token role snapshot, and Host-scoped identifier protection key.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current request is not authenticated as a valid Host user.</exception>
        Task<HostUserRequestContext> ValidateHostUserPermissionRequestAsync();
    }
}
