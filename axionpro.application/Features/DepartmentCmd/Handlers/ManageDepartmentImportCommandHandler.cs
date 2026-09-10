using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.DepartmentCmd.Handlers;

#region Command

/// <summary>Controls saved Department imports through the existing module permission behavior.</summary>
public sealed record ManageDepartmentImportCommand(
    BulkImportJobRequestDTO DTO,
    BulkImportAction Action) : IRequest<ApiResponse<object>>;

#endregion

#region Handler

public sealed class ManageDepartmentImportCommandHandler(BulkImportWorkflowService workflow)
    : IRequestHandler<ManageDepartmentImportCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        ManageDepartmentImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = await workflow.ActAsync(
            BulkImportMaster.Department, request.Action, request.DTO, cancellationToken);
        return ApiResponse<object>.Success(result);
    }
}

#endregion
