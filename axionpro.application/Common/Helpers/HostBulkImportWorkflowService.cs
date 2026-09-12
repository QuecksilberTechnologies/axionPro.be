using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;

namespace axionpro.application.Common.Helpers;

/// <summary>Host import orchestration using the established persisted permission validator.</summary>
public sealed class HostBulkImportWorkflowService(
    IBulkImportRepository repository, ICommonRequestService common,
    IUnitOfWork unitOfWork, IIdEncoderService ids)
{
    #region Preview and actions

    public async Task<BulkImportPreviewResponseDTO> PreviewAsync(
        BulkImportMaster master, HostBulkImportPreviewRequestDTO request, CancellationToken ct)
    {
        var host = await AuthorizeAsync(master, request.ModuleId, request.OperationId, false, ct);
        var actor = Owner(master, request.TenantId, host);
        var table = await BulkImportTableReader.ReadAsync(request, ct);
        var preview = await repository.PreviewHostAsync(master, table, request.ColumnMappingJson,
            actor.TenantId, host, ct);
        return await repository.SaveDraftAsync(preview, request, actor, ct);
    }

    public async Task<object> ActAsync(BulkImportMaster master, BulkImportAction action,
        HostBulkImportJobRequestDTO request, CancellationToken ct)
    {
        if (action == BulkImportAction.SendInvitations || !Enum.IsDefined(action))
        {
            throw new ValidationErrorException("Unsupported Host import action.");
        }
        var read = action is BulkImportAction.Get or BulkImportAction.List or BulkImportAction.Template;
        var host = await AuthorizeAsync(master, request.ModuleId, request.OperationId, read, ct);
        if (action == BulkImportAction.Template)
        {
            return master == BulkImportMaster.DeviceMaster
                ? "SNo,DeviceCode,DeviceName,CompanyName,ModelNo,DeviceType,IsActive\r\n"
                : string.Join(",", HostBulkImportTableMapper.CardColumns) + "\r\n";
        }
        var actor = Owner(master, request.TenantId, host);
        if (action == BulkImportAction.List)
        {
            return await repository.ListAsync(master, request, actor, ct);
        }
        return await repository.ActAsync(master, action, request, actor, ct);
    }

    #endregion

    #region Host authorization and ownership

    private async Task<HostUserRequestContext> AuthorizeAsync(
        BulkImportMaster master, int moduleId, int operationId, bool read, CancellationToken ct)
    {
        var expected = master switch
        {
            BulkImportMaster.DeviceMaster => BulkImportConstants.HostDeviceBulkModuleCode,
            BulkImportMaster.TenantCard => BulkImportConstants.HostCardBulkModuleCode,
            _ => throw new ValidationErrorException("Unsupported Host import target.")
        };
        var host = await HostRuntimePermissionValidator.ValidateAsync(
            common, unitOfWork.StoreProcedureRepository, moduleId, operationId, ct);
        var module = await unitOfWork.ModuleRepository.GetModuleByIdAsync(moduleId);
        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(operationId);
        if (module?.ModuleCode != expected || module.ModuleScope != 2 || !module.IsActive ||
            operation?.IsActive != true ||
            (operation.OperationType != (int)OperationType.Import &&
             !(read && operation.OperationType == (int)OperationType.View)))
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
        return host;
    }

    private CommonDecodedResult Owner(BulkImportMaster master, string? tenantId, HostUserRequestContext host)
    {
        if (master == BulkImportMaster.DeviceMaster && !string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ValidationErrorException("Device catalogue import does not accept a TenantId.");
        }
        // Shared queue stores an actor ID. Host-only master values partition the actor domain;
        // worker authorization uses Host permission lookup, never TenantEmployee permissions.
        return new CommonDecodedResult
        {
            Success = true,
            LoggedInEmployeeId = host.HostUserId,
            RoleId = checked((int)host.TokenHostRoleId),
            TenantId = master == BulkImportMaster.TenantCard
                ? HostTenantIdentifierProtector.Decrypt(tenantId, host.TenantEncryptionKey, ids) : 0
        };
    }

    #endregion
}
