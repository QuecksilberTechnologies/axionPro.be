// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Centrally enforces Department module-operation permissions.
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

namespace axionpro.application.Features.DepartmentCmd;

/// <summary>
/// Enforces the current tenant user's database-backed Department permissions
/// before any Department command or query reaches its handler.
/// </summary>
/// <typeparam name="TRequest">The MediatR command or query.</typeparam>
/// <typeparam name="TResponse">The command or query response.</typeparam>
public sealed class DepartmentPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<DepartmentPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const string DepartmentModuleCode = "TENANT_DEPARTMENTS";

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith(
                "axionpro.application.Features.DepartmentCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);

        if (permissionRequest.ModuleId <= 0 || permissionRequest.OperationId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var moduleCode = await commonRequestService
            .GetModuleCodeAsync(permissionRequest.ModuleId);
        if (!string.Equals(moduleCode, DepartmentModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Department module-code mismatch for {DepartmentRequest}. ModuleId: {ModuleId}, ModuleCode: {ModuleCode}, ExpectedModuleCode: {ExpectedModuleCode}",
                typeof(TRequest).Name,
                permissionRequest.ModuleId,
                moduleCode,
                DepartmentModuleCode);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        if (validation.TenantId <= 0 ||
            validation.LoggedInEmployeeId <= 0 ||
            validation.RoleId <= 0)
        {
            logger.LogWarning(
                "Invalid Tenant authorization context for Department request {DepartmentRequest}. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                typeof(TRequest).Name,
                validation.TenantId,
                validation.LoggedInEmployeeId,
                validation.RoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // The stored function resolves the current primary and secondary role
        // assignments, so a valid but stale JWT cannot grant Department access.
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
    /// Reads the existing command/query DTO shape without altering endpoint routes.
    /// </summary>
    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        foreach (var memberName in new[] { "DTO", "Dto", "OptionDTO", "Filter", "PermissionRequest" })
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
