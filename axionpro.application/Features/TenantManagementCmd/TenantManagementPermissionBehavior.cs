// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Binds Host Tenant-management requests to their active ModuleCode
//           before the established Host runtime permission flow is used.
// ================================================================

using System.Reflection;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantManagementCmd;

/// <summary>
/// Enforces the Host Tenant-management module contract for every command and
/// query in <c>TenantManagementCmd</c>. Super Admin access remains entirely in
/// the existing <see cref="HostRuntimePermissionValidator"/> flow.
/// </summary>
public sealed class TenantManagementPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantManagementPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith(
                "axionpro.application.Features.TenantManagementCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        var expectedModuleCode = ResolveExpectedModuleCode();
        if (string.IsNullOrWhiteSpace(expectedModuleCode))
        {
            logger.LogWarning(
                "No Host Tenant-management module-code binding exists for request {TenantRequest}.",
                typeof(TRequest).FullName);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var permissionRequest = ResolvePermissionRequest(request);
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            commonRequestService,
            unitOfWork.StoreProcedureRepository,
            permissionRequest?.ModuleId ?? 0,
            permissionRequest?.OperationId ?? 0,
            cancellationToken);

        // The HostRuntimePermissionValidator verifies the current role before
        // permitting Super Admin access, including when the client omits IDs.
        if (hostContext.CurrentHostRoleId == AppConstants.SuperAdminHostRoleId)
        {
            return await next();
        }

        if (permissionRequest is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var activeModuleCode = await commonRequestService
            .GetActiveModuleCodeAsync(permissionRequest.ModuleId);
        if (!string.Equals(activeModuleCode, expectedModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Host Tenant-management module-code mismatch for {TenantRequest}. ModuleId: {ModuleId}, ActiveModuleCode: {ActiveModuleCode}, ExpectedModuleCode: {ExpectedModuleCode}",
                typeof(TRequest).Name,
                permissionRequest.ModuleId,
                activeModuleCode,
                expectedModuleCode);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        return await next();
    }

    /// <summary>
    /// Reads the existing request-carried module-operation values without
    /// changing controller signatures or public request contracts.
    /// </summary>
    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        var requestType = typeof(TRequest);
        var directModule = requestType.GetProperty("ModuleId", BindingFlags.Public | BindingFlags.Instance);
        var directOperation = requestType.GetProperty("OperationId", BindingFlags.Public | BindingFlags.Instance);
        if (directModule?.PropertyType == typeof(int) && directOperation?.PropertyType == typeof(int))
        {
            return new PermissionRequestDTO
            {
                ModuleId = (int)(directModule.GetValue(request) ?? 0),
                OperationId = (int)(directOperation.GetValue(request) ?? 0)
            };
        }

        foreach (var memberName in new[] { "RequestDTO", "DTO", "Filter", "PermissionRequest", "AccessRequest" })
        {
            var property = requestType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property?.GetValue(request) is PermissionRequestDTO permissionRequest)
            {
                return permissionRequest;
            }

            var field = requestType.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (field?.GetValue(request) is PermissionRequestDTO fieldPermissionRequest)
            {
                return fieldPermissionRequest;
            }
        }

        return null;
    }

    /// <summary>
    /// Resolves the only Host Tenant-management leaf module allowed for each
    /// request. The seeded header module is never an authorization target.
    /// </summary>
    private static string? ResolveExpectedModuleCode() => typeof(TRequest).Name switch
    {
        "CreateNewTenantCommand" => "HOST_TENANT_CREATE",

        "GetAllTenantsQuery" or
        "GetTenantByIdQuery" or
        "GetTenantDeleteDependencyInfoQuery" or
        "UpdateTenantCommand" or
        "UpdateHostManagedTenantCommand" or
        "UpdateNewTenantCommand" or
        "SynchronizeTenantPlanEntitlementsCommand" or
        "ResendTenantVerificationCommand" or
        "DeleteHostManagedTenantCommand" or
        "ActivateTenantCommand" or
        "DeactivateTenantCommand" => "HOST_TENANT_LIST",

        _ => null
    };
}
