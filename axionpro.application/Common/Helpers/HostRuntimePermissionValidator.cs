// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Applies current database Host module-operation authorization to Host-managed application requests.
// ================================================================

using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Validates the trusted Host request context against current Host role permissions.
/// </summary>
public static class HostRuntimePermissionValidator
{
    /// <summary>
    /// Validates the Host JWT context and verifies its requested module-operation permission against current database state.
    /// </summary>
    /// <param name="commonRequestService">The trusted Host request validator.</param>
    /// <param name="storeProcedureRepository">The repository used to invoke the Host permission function.</param>
    /// <param name="moduleId">The module identifier supplied by the request permission contract.</param>
    /// <param name="operationId">The operation identifier supplied by the request permission contract.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The trusted Host context used by the handler after authorization succeeds.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the Host session is invalid or its role snapshot is stale.</exception>
    /// <exception cref="ForbiddenAccessException">Thrown when the active Host role lacks the requested permission.</exception>
    public static async Task<HostUserRequestContext> ValidateAsync(
        ICommonRequestService commonRequestService,
        IStoreProcedureRepository storeProcedureRepository,
        int moduleId,
        int operationId,
        CancellationToken cancellationToken)
    {
        if (moduleId <= 0 || operationId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var hostContext = await commonRequestService.ValidateHostUserPermissionRequestAsync();
        var permission = await storeProcedureRepository.CheckHostUserPermissionAsync(
            hostContext.HostUserId,
            hostContext.TokenHostRoleId,
            moduleId,
            operationId,
            cancellationToken);

        return permission.ResultCode switch
        {
            1 => hostContext,
            -1 or -2 => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized),
            0 => throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied),
            _ => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized)
        };
    }
}
