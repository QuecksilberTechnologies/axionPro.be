using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;

namespace axionpro.application.Features.TenantEmailTemplateCmd;

public interface ITenantEmailTemplatePermissionRequest
{
    PermissionRequestDTO? PermissionRequest { get; }
}

public sealed class TenantEmailTemplatePermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private const string ModuleCode = "TENANT_EMAIL_TEMPLATE";

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITenantEmailTemplatePermissionRequest protectedRequest)
        {
            return await next();
        }

        var permission = protectedRequest.PermissionRequest
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (permission.ModuleId <= 0 || permission.OperationId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var moduleCode = await commonRequestService.GetModuleCodeAsync(permission.ModuleId);
        if (!string.Equals(moduleCode, ModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0 || actor.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var result = await unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(
            actor.TenantId, actor.LoggedInEmployeeId, actor.RoleId,
            permission.ModuleId, permission.OperationId, cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(result);
        return await next();
    }
}
