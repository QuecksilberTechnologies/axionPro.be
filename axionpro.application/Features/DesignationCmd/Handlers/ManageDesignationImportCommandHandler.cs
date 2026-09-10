using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.DesignationCmd.Handlers;

#region Command

/// <summary>Controls saved Designation imports through the existing module permission behavior.</summary>
public sealed record ManageDesignationImportCommand(
    BulkImportJobRequestDTO DTO,
    BulkImportAction Action) : IRequest<ApiResponse<object>>;

#endregion

#region Handler

public sealed class ManageDesignationImportCommandHandler(BulkImportWorkflowService workflow)
    : IRequestHandler<ManageDesignationImportCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        ManageDesignationImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = await workflow.ActAsync(
            BulkImportMaster.Designation, request.Action, request.DTO, cancellationToken);
        return ApiResponse<object>.Success(result);
    }
}

#endregion
