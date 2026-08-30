// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides common authenticated Tenant context and response helpers for TenantConfiguration handlers.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Handlers;

/// <summary>Provides common Tenant authentication and paging-response behavior for this module.</summary>
public abstract class TenantConfigurationHandlerBase
{
    /// <summary>Initializes common handler dependencies.</summary>
    protected TenantConfigurationHandlerBase(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger)
    {
        UnitOfWork = unitOfWork;
        CommonRequestService = commonRequestService;
        Logger = logger;
    }

    /// <summary>Gets the unit of work used by TenantConfiguration handlers.</summary>
    protected IUnitOfWork UnitOfWork { get; }

    /// <summary>Gets the service that validates the authenticated Tenant principal.</summary>
    protected ICommonRequestService CommonRequestService { get; }

    /// <summary>Gets the structured logger for TenantConfiguration activity.</summary>
    protected ILogger<TenantConfigurationHandlerBase> Logger { get; }

    /// <summary>Resolves the trusted Tenant and employee audit actor from the current request.</summary>
    /// <returns>The authenticated Tenant identifier and employee identifier.</returns>
    protected async Task<(long TenantId, long ActorId)> ValidateTenantAsync()
    {
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        return (validation.TenantId, validation.LoggedInEmployeeId);
    }

    /// <summary>
    /// Resolves the authenticated Tenant context and enforces the requested
    /// module operation using the current database role assignments.
    /// </summary>
    /// <param name="request">The request carrying the client-supplied module and operation identifiers.</param>
    /// <param name="cancellationToken">Token used to cancel the authorization operation.</param>
    /// <returns>The trusted Tenant identifier and employee audit actor.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the Tenant context is invalid, stale, or denied.</exception>
    protected async Task<(long TenantId, long ActorId)> ValidateTenantPermissionAsync(
        PermissionRequestDTO request,
        CancellationToken cancellationToken)
    {
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId,
                actorId,
                tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                request.ModuleId,
                request.OperationId,
                cancellationToken);

        switch (permissionResult.ResultCode)
        {
            case 1:
                return (tenantId, actorId);

            case -1:
                Logger.LogWarning(
                    "Tenant authorization context changed. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    actorId,
                    tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);

            case -2:
                Logger.LogWarning(
                    "Invalid Tenant role context. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    actorId,
                    tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);

            case 0:
                Logger.LogWarning(
                    "Tenant permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                    tenantId,
                    actorId,
                    request.ModuleId,
                    request.OperationId);
                throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

            default:
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
    }

    /// <summary>Creates a flattened paginated API response without data nesting.</summary>
    protected static ApiResponse<List<TResponse>> Paged<TResponse>(List<TResponse> data, int pageNumber, int pageSize, int totalRecords, string message) =>
        ApiResponse<List<TResponse>>.SuccessPaginated(data, pageNumber, pageSize, totalRecords, (int)Math.Ceiling(totalRecords / (double)pageSize), message);
}
