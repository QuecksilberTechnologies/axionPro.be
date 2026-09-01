// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Binds TenantLocation requests to their active module before the
//           established Host or Tenant runtime permission flow is used.
// ================================================================

using System.Reflection;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd;

/// <summary>
/// Centrally authorizes only the six TenantLocation commands and queries. It
/// supports the existing Host Super Admin bypass, normal Host permissions, and
/// Tenant employee permissions without changing endpoint contracts.
/// </summary>
public sealed class TenantLocationPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantLocationPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsTenantLocationRequest())
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var principal = await commonRequestService.ValidateAuthenticatedRequestAsync();

        if (principal.UserType == LoginUserType.Host)
        {
            var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
                commonRequestService,
                unitOfWork.StoreProcedureRepository,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

            if (hostContext.CurrentHostRoleId == AppConstants.SuperAdminHostRoleId)
            {
                return await next();
            }

            await EnsureExpectedModuleCodeAsync(permissionRequest, cancellationToken);
            return await next();
        }

        if (principal.UserType != LoginUserType.TenantEmployee)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        await EnsureExpectedModuleCodeAsync(permissionRequest, cancellationToken);

        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        if (validation.TenantId <= 0 || validation.LoggedInEmployeeId <= 0 || validation.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var permissionResult = await unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                validation.TenantId,
                validation.LoggedInEmployeeId,
                validation.RoleId,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);
        return await next();
    }

    /// <summary>
    /// Enforces the seeded leaf-module code after the request principal type is
    /// known. A mismatched client-supplied ModuleId is permission denied.
    /// </summary>
    private async Task EnsureExpectedModuleCodeAsync(
        PermissionRequestDTO permissionRequest,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var moduleCode = await commonRequestService
            .GetModuleCodeAsync(permissionRequest.ModuleId);
        if (string.Equals(moduleCode, "TENANT_LOCATIONS", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        logger.LogWarning(
            "TenantLocation module-code mismatch for {TenantLocationRequest}. ModuleId: {ModuleId}, ModuleCode: {ModuleCode}, ExpectedModuleCode: TENANT_LOCATIONS",
            typeof(TRequest).Name,
            permissionRequest.ModuleId,
            moduleCode);
        throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
    }

    /// <summary>
    /// Retrieves the existing permission DTO shape used by the location
    /// controller's commands and queries.
    /// </summary>
    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        foreach (var memberName in new[] { "DTO", "Filter", "AccessRequest", "PermissionRequest" })
        {
            var property = typeof(TRequest).GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property?.GetValue(request) is PermissionRequestDTO permissionRequest)
            {
                return permissionRequest;
            }

            var field = typeof(TRequest).GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (field?.GetValue(request) is PermissionRequestDTO fieldPermissionRequest)
            {
                return fieldPermissionRequest;
            }
        }

        return null;
    }

    /// <summary>
    /// Identifies only the TenantLocation request types; other Tenant
    /// configuration endpoints continue using their existing authorization.
    /// </summary>
    private static bool IsTenantLocationRequest() => typeof(TRequest).Name is
        "CreateTenantLocationCommand" or
        "UpdateTenantLocationCommand" or
        "DeleteTenantLocationCommand" or
        "UpdateTenantLocationStatusCommand" or
        "GetTenantLocationByIdQuery" or
        "GetTenantLocationsQuery";
}
