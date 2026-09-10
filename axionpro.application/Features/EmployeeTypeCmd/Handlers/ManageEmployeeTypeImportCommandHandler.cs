using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeTypeCmd.Handlers;

#region Command

/// <summary>Controls saved EmployeeType imports through the existing module permission behavior.</summary>
public sealed record ManageEmployeeTypeImportCommand(
    BulkImportJobRequestDTO DTO,
    BulkImportAction Action) : IRequest<ApiResponse<object>>;

#endregion

#region Handler

public sealed class ManageEmployeeTypeImportCommandHandler(BulkImportWorkflowService workflow)
    : IRequestHandler<ManageEmployeeTypeImportCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        ManageEmployeeTypeImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = await workflow.ActAsync(
            BulkImportMaster.EmployeeType, request.Action, request.DTO, cancellationToken);
        return ApiResponse<object>.Success(result);
    }
}

#endregion
