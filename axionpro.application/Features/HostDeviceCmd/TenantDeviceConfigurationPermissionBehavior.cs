// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Applies the established module-code and current role-permission
//           behavior to initial and runtime Tenant device configuration.
// ================================================================

using System.Reflection;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HostDeviceCmd;

/// <summary>
/// Applies the same trusted module-code and database-backed permission contract
/// used by Employee feature requests to the device configuration request set.
/// </summary>
/// <typeparam name="TRequest">The MediatR command or query.</typeparam>
/// <typeparam name="TResponse">The MediatR response.</typeparam>
public sealed class TenantDeviceConfigurationPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantDeviceConfigurationPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const string HostInitialDeviceConfigurationModuleCode = "HOST_INITIAL_DEVICE_CONFIGURATION";
    private const string TenantDeviceConfigurationModuleCode = "TENANT_DEVICE_CONFIGURATION";

    #region Permission Pipeline

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith(
                "axionpro.application.Features.HostDeviceCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        var expectedModuleCode = ResolveExpectedModuleCode();
        if (expectedModuleCode is null)
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        await EnsureExpectedModuleCodeAsync(permissionRequest, expectedModuleCode);

        var principal = await commonRequestService.ValidateAuthenticatedRequestAsync();
        if (string.Equals(expectedModuleCode, HostInitialDeviceConfigurationModuleCode, StringComparison.Ordinal))
        {
            if (principal.UserType != LoginUserType.Host)
            {
                throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
            }

            await HostRuntimePermissionValidator.ValidateAsync(
                commonRequestService,
                unitOfWork.StoreProcedureRepository,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);
            return await next();
        }

        if (principal.UserType != LoginUserType.TenantEmployee)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var tenantContext = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!tenantContext.Success ||
            tenantContext.TenantId <= 0 ||
            tenantContext.LoggedInEmployeeId <= 0 ||
            tenantContext.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(
                tenantContext.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var permissionResult = await unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(
            tenantContext.TenantId,
            tenantContext.LoggedInEmployeeId,
            tenantContext.RoleId,
            permissionRequest.ModuleId,
            permissionRequest.OperationId,
            cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        return await next();
    }

    #endregion

    #region Module Ownership Mapping

    /// <summary>Resolves the leaf module that owns only the secured device configuration request set.</summary>
    private static string? ResolveExpectedModuleCode()
    {
        var requestType = typeof(TRequest);
        if (requestType == typeof(IssueInitialDeviceBootstrapCommand))
        {
            return HostInitialDeviceConfigurationModuleCode;
        }

        return requestType == typeof(CreateTenantDeviceConfigurationCommand) ||
               requestType == typeof(UpdateTenantDeviceConfigurationCommand) ||
               requestType == typeof(DeleteTenantDeviceConfigurationCommand) ||
               requestType == typeof(GetTenantDeviceConfigurationByIdQuery) ||
               requestType == typeof(GetAllTenantDeviceConfigurationsQuery) ||
               requestType == typeof(RotateTenantDeviceHttpsIngressTokenCommand) ||
               requestType == typeof(ApplyTenantDeviceRuntimeConfigurationCommand) ||
               requestType == typeof(RebootTenantDeviceCommand)
             ? TenantDeviceConfigurationModuleCode
             : null;
    }

    #endregion

    #region Request Contract Resolution

    /// <summary>Checks that client-supplied module and operation IDs refer to the expected active leaf module.</summary>
    private async Task EnsureExpectedModuleCodeAsync(
        PermissionRequestDTO permissionRequest,
        string expectedModuleCode)
    {
        if (permissionRequest.ModuleId <= 0 || permissionRequest.OperationId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var moduleCode = await commonRequestService.GetModuleCodeAsync(permissionRequest.ModuleId);
        if (string.Equals(moduleCode, expectedModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        logger.LogWarning(
            "Device configuration module-code mismatch for {RequestName}. ModuleId: {ModuleId}; ModuleCode: {ModuleCode}; ExpectedModuleCode: {ExpectedModuleCode}",
            typeof(TRequest).Name,
            permissionRequest.ModuleId,
            moduleCode,
            expectedModuleCode);
        throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
    }

    /// <summary>Retrieves the request DTO carrying ModuleId and OperationId, following the existing Employee behavior convention.</summary>
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

    #endregion
}
