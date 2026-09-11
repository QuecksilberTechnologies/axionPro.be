using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using Microsoft.Extensions.Configuration;

namespace axionpro.application.Common.Helpers;

/// <summary>Coordinates authenticated durable imports behind existing module permission behaviors.</summary>
public sealed partial class BulkImportWorkflowService(
    BulkImportPreviewService previewService,
    IBulkImportRepository repository,
    ICommonRequestService commonRequestService,
    IUnitOfWork unitOfWork,
    ITokenService? tokenService = null,
    IEmailService? emailService = null,
    IIdEncoderService? idEncoderService = null,
    IConfiguration? configuration = null)
{
    public async Task<BulkImportPreviewResponseDTO> PreviewAsync(
        BulkImportMaster master,
        BulkImportPreviewRequestDTO request,
        CancellationToken cancellationToken)
    {
        var actor = await ValidateAsync(request.OperationId, false);
        var preview = master == BulkImportMaster.Employee
            ? await repository.PreviewEmployeesAsync(
                await BulkImportTableReader.ReadAsync(request, cancellationToken),
                request.ColumnMappingJson, actor, cancellationToken)
            : await previewService.PreviewAsync(master, request, cancellationToken);
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
        if (action == BulkImportAction.SendInvitations)
        {
            if (master != BulkImportMaster.Employee)
            {
                throw new ValidationErrorException("Invitations are supported only for Employee imports.");
            }
            return await SendEmployeeInvitationsAsync(request, actor, cancellationToken);
        }
        if (action == BulkImportAction.Template)
        {
            return master switch
            {
                BulkImportMaster.Employee => string.Join(",", BulkImportConstants.EmployeeColumns) + "\r\n",
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
