// ================================================================
// Purpose : Enforces Host module-operation permission for central SMTP configuration management.
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

namespace axionpro.application.Features.DefaultEmailConfigCmd;

/// <summary>
/// Restricts every DefaultEmailConfig request to the Host-only
/// <c>HOST_DEFAULT_EMAIL_CONFIG</c> module. Non-Super-Admin Host users must have
/// the supplied module-operation permission in the current database state.
/// </summary>
public sealed class DefaultEmailConfigPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<DefaultEmailConfigPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const string DefaultEmailConfigModuleCode = "HOST_DEFAULT_EMAIL_CONFIG";

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith(
                "axionpro.application.Features.DefaultEmailConfigCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request);
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            commonRequestService,
            unitOfWork.StoreProcedureRepository,
            permissionRequest?.ModuleId ?? 0,
            permissionRequest?.OperationId ?? 0,
            cancellationToken);

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
        if (!string.Equals(
                activeModuleCode,
                DefaultEmailConfigModuleCode,
                StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Default email configuration module-code mismatch for {RequestName}. ModuleId: {ModuleId}; ActiveModuleCode: {ActiveModuleCode}; ExpectedModuleCode: {ExpectedModuleCode}",
                typeof(TRequest).Name,
                permissionRequest.ModuleId,
                activeModuleCode,
                DefaultEmailConfigModuleCode);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        return await next();
    }

    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        foreach (var memberName in new[] { "PermissionRequest", "RequestDTO", "DTO", "Filter" })
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
