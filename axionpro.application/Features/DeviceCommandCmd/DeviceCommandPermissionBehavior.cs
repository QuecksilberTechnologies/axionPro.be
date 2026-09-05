// ================================================================
// Purpose : Enforces current Host/Tenant module-operation permissions before
//           a command reaches the command submission handler.
// ================================================================

using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.DeviceCommandCmd;

/// <summary>Applies authorization policy based on the confirmed vendor command catalog.</summary>
public sealed class DeviceCommandPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not SubmitDeviceCommand deviceCommand)
        {
            return await next();
        }

        var dto = deviceCommand.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var definition = DeviceProtocolCommandCatalog.GetRequired(dto.CommandName);
        var principal = await commonRequestService.ValidateAuthenticatedRequestAsync();

        if (principal.UserType == LoginUserType.Host)
        {
            await HostRuntimePermissionValidator.ValidateAsync(
                commonRequestService,
                unitOfWork.StoreProcedureRepository,
                dto.ModuleId,
                dto.OperationId,
                cancellationToken);
            return await next();
        }

        if (principal.UserType != LoginUserType.TenantEmployee ||
            definition.AccessLevel == DeviceCommandAccessLevel.HostOnly)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var tenantContext = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!tenantContext.Success || tenantContext.TenantId <= 0 ||
            tenantContext.LoggedInEmployeeId <= 0 || tenantContext.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(tenantContext.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var permission = await unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(
            tenantContext.TenantId,
            tenantContext.LoggedInEmployeeId,
            tenantContext.RoleId,
            dto.ModuleId,
            dto.OperationId,
            cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permission);
        return await next();
    }
}
