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
        /// Validates the current authenticated tenant request and resolves the trusted tenant, employee, and role context.
        /// </summary>
        /// <returns>The validated tenant request context.</returns>
        Task<CommonDecodedResult> ValidateRequestAsync();

        /// <summary>
        /// Validates that the current JWT belongs to an active Host user with an active Host role.
        /// </summary>
        /// <returns>The validated Host user identifier.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current request is not authenticated as a valid Host user.</exception>
        Task<long> ValidateHostUserRequestAsync();
    }
}
