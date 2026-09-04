// ================================================================
// Purpose : Enforces Host and Tenant runtime permission checks for Tenant SMTP configuration management.
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

namespace axionpro.application.Features.TenantEmailConfigCmd;

/// <summary>
/// Validates the separate Host and Tenant module operations for Tenant SMTP
/// configuration. Host Super Admin retains the established Host bypass; all
/// other callers are checked against current database permissions.
/// </summary>
public sealed class TenantEmailConfigPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantEmailConfigPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const string HostTenantEmailConfigModuleCode = "HOST_TENANT_EMAIL_CONFIG";
    private const string TenantEmailConfigModuleCode = "TENANT_EMAIL_CONFIG";

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith(
                "axionpro.application.Features.TenantEmailConfigCmd",
                StringComparison.Ordinal) != true)
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

            if (hostContext.CurrentHostRoleId != AppConstants.SuperAdminHostRoleId)
            {
                await EnsureExpectedModuleCodeAsync(permissionRequest, HostTenantEmailConfigModuleCode);
            }

            return await next();
        }

        if (principal.UserType != LoginUserType.TenantEmployee)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        await EnsureExpectedModuleCodeAsync(permissionRequest, TenantEmailConfigModuleCode);
        var tenantContext = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!tenantContext.Success ||
            tenantContext.TenantId <= 0 ||
            tenantContext.LoggedInEmployeeId <= 0 ||
            tenantContext.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(
                tenantContext.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var permissionResult = await unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantContext.TenantId,
                tenantContext.LoggedInEmployeeId,
                tenantContext.RoleId,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        return await next();
    }

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
            "Tenant email configuration module-code mismatch for {RequestName}. ModuleId: {ModuleId}; ModuleCode: {ModuleCode}; ExpectedModuleCode: {ExpectedModuleCode}",
            typeof(TRequest).Name,
            permissionRequest.ModuleId,
            moduleCode,
            expectedModuleCode);
        throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
    }

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
}
