using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;

namespace axionpro.application.Common.Helpers;

/// <summary>Coordinates authenticated durable imports behind existing module permission behaviors.</summary>
public sealed class BulkImportWorkflowService(
    BulkImportPreviewService previewService,
    IBulkImportRepository repository,
    ICommonRequestService commonRequestService,
    IUnitOfWork unitOfWork)
{
    public async Task<BulkImportPreviewResponseDTO> PreviewAsync(
        BulkImportMaster master,
        BulkImportPreviewRequestDTO request,
        CancellationToken cancellationToken)
    {
        var actor = await ValidateAsync(request.OperationId, false);
        var preview = await previewService.PreviewAsync(master, request, cancellationToken);
        return await repository.SaveDraftAsync(preview, request, actor, cancellationToken);
    }

    public async Task<object> ActAsync(
        BulkImportMaster master,
        BulkImportAction action,
        BulkImportJobRequestDTO request,
        CancellationToken cancellationToken)
    {
        var actor = await ValidateAsync(request.OperationId,
            action is BulkImportAction.Get or BulkImportAction.List or BulkImportAction.Template);
        if (action == BulkImportAction.Template)
        {
            return master switch
            {
                BulkImportMaster.Department => "DepartmentName,Description,Remark,IsActive\r\n",
                BulkImportMaster.Designation => "DesignationName,DepartmentName,Description,IsActive\r\n",
                BulkImportMaster.EmployeeType => string.Join(",", BulkImportConstants.TypeName,
                    BulkImportConstants.Description, BulkImportConstants.Remark, BulkImportConstants.IsActive) + "\r\n",
                _ => "RoleName,RoleType,Remark,IsActive\r\n"
            };
        }
        if (action == BulkImportAction.List)
        {
            return await repository.ListAsync(master, request, actor, cancellationToken);
        }
        return await repository.ActAsync(master, action, request, actor, cancellationToken);
    }

    private async Task<CommonDecodedResult> ValidateAsync(int operationId, bool read)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0 || actor.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        // The existing pipeline checks the actual grant. This binds the endpoint to its action type.
        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(operationId);
        var allowedType = operation?.OperationType == (int)OperationType.Add ||
            operation?.OperationType == (int)OperationType.Import ||
            (read && operation?.OperationType == (int)OperationType.View);
        if (operation?.IsActive != true || !allowedType)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
        return actor;
    }
}
