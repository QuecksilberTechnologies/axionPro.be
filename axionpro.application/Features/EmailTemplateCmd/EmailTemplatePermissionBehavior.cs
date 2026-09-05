// ================================================================
// Purpose : Enforces Host module-operation permission for central email-template management.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmailTemplateCmd;

/// <summary>
/// Marker used to authorize only the new Host email-template administration requests.
/// Legacy template lookup/send flows intentionally remain outside this module's CRUD permission contract.
/// </summary>
public interface IEmailTemplatePermissionRequest
{
    PermissionRequestDTO? PermissionRequest { get; }
}

/// <summary>
/// Restricts Host email-template administration to the expected active Host module and its current operation permission.
/// </summary>
public sealed class EmailTemplatePermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<EmailTemplatePermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const string EmailTemplateModuleCode = "HOST_EMAIL_TEMPLATE";

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IEmailTemplatePermissionRequest protectedRequest)
        {
            return await next();
        }

        var permissionRequest = protectedRequest.PermissionRequest;
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

        var activeModuleCode = await commonRequestService.GetActiveModuleCodeAsync(permissionRequest.ModuleId);
        if (!string.Equals(activeModuleCode, EmailTemplateModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Email-template module-code mismatch for {RequestName}. ModuleId: {ModuleId}; ActiveModuleCode: {ActiveModuleCode}; ExpectedModuleCode: {ExpectedModuleCode}",
                typeof(TRequest).Name,
                permissionRequest.ModuleId,
                activeModuleCode,
                EmailTemplateModuleCode);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        return await next();
    }
}
