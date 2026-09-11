// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Binds TenantLocation and employee-code requests to their active module before the
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
/// Centrally authorizes the six TenantLocation requests and two employee-code writes. Host
/// roles use persisted module-operation permissions and Tenant employees use
/// the tenant permission function without changing endpoint contracts.
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
        if (!IsTenantLocationRequest() && !IsEmployeeCodePatternRequest())
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var principal = await commonRequestService.ValidateAuthenticatedRequestAsync();

        if (principal.UserType == LoginUserType.Host)
        {
            if (IsEmployeeCodePatternRequest())
            {
                throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
            }

            await HostRuntimePermissionValidator.ValidateAsync(
                commonRequestService,
                unitOfWork.StoreProcedureRepository,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

            await EnsureExpectedModuleCodeAsync(permissionRequest, cancellationToken, LoginUserType.Host);
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
        CancellationToken cancellationToken,
        LoginUserType userType = LoginUserType.TenantEmployee)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var moduleCode = await commonRequestService
            .GetModuleCodeAsync(permissionRequest.ModuleId);
        var expectedModuleCode = IsEmployeeCodePatternRequest()
            ? BulkImportConstants.EmployeeCodeModuleCode
            : userType == LoginUserType.Host
                ? "HOST_TENANT_LOCATION_LIST"
                : "TENANT_LOCATIONS";
        if (string.Equals(moduleCode, expectedModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        logger.LogWarning(
            "Tenant configuration module-code mismatch for {Request}. ModuleId: {ModuleId}, ModuleCode: {ModuleCode}, ExpectedModuleCode: {ExpectedModuleCode}",
            typeof(TRequest).Name,
            permissionRequest.ModuleId,
            moduleCode,
            expectedModuleCode);
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

    /// <summary>Reuses this configuration pipeline for the two tenant pattern writes.</summary>
    private static bool IsEmployeeCodePatternRequest()
    {
        return typeof(TRequest) == typeof(Configuration.EmployeeCodeCmd.Handlers.CreateEmployeeCodePatternCommand) ||
               typeof(TRequest) == typeof(Configuration.EmployeeCodeCmd.Handlers.UpdateEmployeeCodePatternCommand);
    }
}
