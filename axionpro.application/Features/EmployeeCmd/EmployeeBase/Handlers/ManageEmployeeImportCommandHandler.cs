using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;

#region Command

/// <summary>Controls saved Employee imports through the existing module permission behavior.</summary>
public sealed record ManageEmployeeImportCommand(
    BulkImportJobRequestDTO DTO,
    BulkImportAction Action) : IRequest<ApiResponse<object>>;

#endregion

#region Handler

public sealed class ManageEmployeeImportCommandHandler(BulkImportWorkflowService workflow)
    : IRequestHandler<ManageEmployeeImportCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        ManageEmployeeImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = await workflow.ActAsync(
            BulkImportMaster.Employee, request.Action, request.DTO, cancellationToken);
        return ApiResponse<object>.Success(result);
    }
}

#endregion

