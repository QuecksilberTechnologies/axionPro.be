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

        var principal = await commonRequestService.ValidateAuthenticatedRequestAsync();
        var permissionRequest = ResolvePermissionRequest(request)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);

        if (principal.UserType == LoginUserType.Host)
        {
            var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
                commonRequestService,
                unitOfWork.StoreProcedureRepository,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

            // Host access is intentionally limited to device provisioning and
            // issuing/reissuing a gateway URL. The Tenant owns all connection,
            // runtime, and device-operation configuration after assignment.
            if (!IsHostAllowedRequest())
            {
                throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
            }

            // Super Admin is the Host-wide authority and therefore does not need a
            // per-module mapping. Other Host users must both pass the database
            // permission check above and submit the expected device module.
            if (hostContext.CurrentHostRoleId != AppConstants.SuperAdminHostRoleId)
            {
                await EnsureExpectedModuleCodeAsync(permissionRequest, expectedModuleCode);
            }

            return await next();
        }

        await EnsureExpectedModuleCodeAsync(permissionRequest, expectedModuleCode);

        if (string.Equals(expectedModuleCode, HostInitialDeviceConfigurationModuleCode, StringComparison.Ordinal))
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
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
               requestType == typeof(RebootTenantDeviceCommand) ||
               requestType == typeof(ApplyTenantDeviceSettingsCommand) ||
               requestType == typeof(SyncTenantDeviceTimeCommand) ||
               requestType == typeof(UpdateTenantDeviceLocationCommand) ||
               requestType == typeof(GetTenantDeviceGatewayAddressQuery) ||
               requestType == typeof(ReplaceTenantDeviceHttpsGatewayUrlCommand) ||
               requestType == typeof(DispatchTenantDeviceMqttsNowCommand)
             ? TenantDeviceConfigurationModuleCode
             : null;
    }

    /// <summary>
    /// Host users provision unassigned inventory and may issue an HTTPS gateway
    /// URL for a device. Once assigned, Tenant admins own all configuration,
    /// runtime settings, and device operations.
    /// </summary>
    private static bool IsHostAllowedRequest() =>
        typeof(TRequest) == typeof(IssueInitialDeviceBootstrapCommand) ||
        typeof(TRequest) == typeof(RotateTenantDeviceHttpsIngressTokenCommand);

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
