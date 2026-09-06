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
    // Device-global commands deliberately have no raw API escape hatch. Each one
    // must gain a typed request, validation rules, and a Tenant-admin policy
    // before it can be exposed in the product UI.
    private static readonly HashSet<string> TypedDeviceAdministrationCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        DeviceCommands.CleanAdmin,
        DeviceCommands.CleanDatabase,
        DeviceCommands.CleanInactiveUser,
        DeviceCommands.CleanLog,
        DeviceCommands.CleanLogPhoto,
        DeviceCommands.CleanUser,
        DeviceCommands.CleanUserLock,
        DeviceCommands.DisableDevice,
        DeviceCommands.EnableDevice,
        DeviceCommands.ForceOta,
        DeviceCommands.InitializeMenu,
        DeviceCommands.InitializeSystem,
        DeviceCommands.Keypad,
        DeviceCommands.Reboot,
        DeviceCommands.SetBellTime,
        DeviceCommands.SetCompanyName,
        DeviceCommands.SetDepartment,
        DeviceCommands.SetDeviceInfo,
        DeviceCommands.SetDeviceLock,
        DeviceCommands.SetHoliday,
        DeviceCommands.SetOtaServer,
        DeviceCommands.SetQuestionnaire,
        DeviceCommands.SetScreenSaver,
        DeviceCommands.SetShift,
        DeviceCommands.SetTime,
        DeviceCommands.SetVoice,
        DeviceCommands.Upgrade,
        DeviceCommands.WriteFile
    };

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

        // Device-global configuration and destructive actions are typed,
        // audited operations. They must never be submitted through this generic
        // raw-payload endpoint; otherwise a Host caller could bypass the
        // Tenant-admin policy and field-level validation.
        if (TypedDeviceAdministrationCommands.Contains(definition.Name))
        {
            throw new ValidationErrorException(
                "This device-administration command is not available through the generic endpoint. Use its typed Tenant device-configuration operation.");
        }

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
