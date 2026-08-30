// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Enforces the trusted Tenant role-permission contract for EmployeeCmd requests.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd;

/// <summary>
/// Applies the existing Tenant request and database-backed role permission
/// validation consistently to every EmployeeCmd command and query.
/// </summary>
/// <typeparam name="TRequest">The MediatR command or query.</typeparam>
/// <typeparam name="TResponse">The command or query response.</typeparam>
public sealed class EmployeeTenantPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<EmployeeTenantPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // This behavior is deliberately limited to EmployeeCmd. Host pages do
        // not use these endpoints and therefore cannot receive a Host bypass.
        var requestNamespace = typeof(TRequest).Namespace;
        if (requestNamespace?.StartsWith(
                "axionpro.application.Features.EmployeeCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        // These two groups already derive from TenantConfigurationHandlerBase,
        // which executes the same central tenant permission flow in each handler.
        // Skipping them here prevents duplicate stored-procedure authorization.
        if (requestNamespace.StartsWith(
                "axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo",
                StringComparison.Ordinal) ||
            requestNamespace.StartsWith(
                "axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment",
                StringComparison.Ordinal))
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request);
        if (permissionRequest is null)
        {
            return await next();
        }

        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        long tenantId = validation.TenantId;
        long userEmployeeId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || userEmployeeId <= 0 || tokenRoleId <= 0)
        {
            logger.LogWarning(
                "Invalid Tenant authorization context for {EmployeeRequest}. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                typeof(TRequest).Name,
                tenantId,
                userEmployeeId,
                tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // The stored procedure uses the current role assignments; the JWT role
        // is only a context value and is never the final authorization source.
        var permissionResult = await unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                userEmployeeId,
                tokenRoleId,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

        if (permissionResult.ResultCode != 1)
        {
            logger.LogWarning(
                "Tenant permission denied for {EmployeeRequest}. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}, ResultCode: {ResultCode}",
                typeof(TRequest).Name,
                tenantId,
                userEmployeeId,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                permissionResult.ResultCode);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        return await next();
    }

    /// <summary>
    /// Retrieves the DTO carrying ModuleId and OperationId without changing
    /// existing endpoint routes or response contracts.
    /// </summary>
    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        foreach (var propertyName in new[] { "DTO", "Filter", "PermissionRequest" })
        {
            var property = typeof(TRequest).GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property?.GetValue(request) is PermissionRequestDTO permissionRequest)
            {
                return permissionRequest;
            }
        }

        return null;
    }
}
