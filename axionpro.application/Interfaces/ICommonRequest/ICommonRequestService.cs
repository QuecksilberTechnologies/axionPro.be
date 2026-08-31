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
        /// Determines whether the authenticated tenant employee may access data that belongs to
        /// the specified employee. The target is always verified against the authenticated tenant;
        /// callers must never use an unverified client-supplied employee identifier as authority.
        /// </summary>
        /// <param name="requestContext">The trusted tenant context obtained from <see cref="ValidateTenantUserRequestAsync"/>.</param>
        /// <param name="targetEmployeeId">The decoded employee identifier whose data is requested.</param>
        /// <param name="requirement">The minimum visibility level required by the endpoint.</param>
        /// <param name="cancellationToken">Token used to cancel the database lookup.</param>
        /// <returns><see langword="true"/> only when the requester is entitled to the target data.</returns>
        Task<bool> CanAccessEmployeeDataAsync(
            CommonDecodedResult requestContext,
            long targetEmployeeId,
            EmployeeDataAccessRequirement requirement,
            CancellationToken cancellationToken = default);
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

    /// <summary>
    /// Defines the information category requested from an employee-owned endpoint.
    /// </summary>
    public enum EmployeeDataAccessRequirement
    {
        /// <summary>Directory fields only: name, organization, approved contact, and profile image.</summary>
        DirectoryBasic = 1,

        /// <summary>Personal or administrative employee information.</summary>
        PersonalDetails = 2,

        /// <summary>Employee work-location assignments.</summary>
        WorkLocation = 3
    }
}
