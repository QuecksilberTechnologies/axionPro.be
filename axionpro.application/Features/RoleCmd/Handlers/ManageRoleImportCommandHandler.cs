using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.RoleCmd.Handlers;

#region Command

/// <summary>Controls saved Role imports through the existing module permission behavior.</summary>
public sealed record ManageRoleImportCommand(
    BulkImportJobRequestDTO DTO,
    BulkImportAction Action) : IRequest<ApiResponse<object>>;

#endregion

#region Handler

public sealed class ManageRoleImportCommandHandler(BulkImportWorkflowService workflow)
    : IRequestHandler<ManageRoleImportCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        ManageRoleImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = await workflow.ActAsync(
            BulkImportMaster.Role, request.Action, request.DTO, cancellationToken);
        return ApiResponse<object>.Success(result);
    }
}

#endregion
