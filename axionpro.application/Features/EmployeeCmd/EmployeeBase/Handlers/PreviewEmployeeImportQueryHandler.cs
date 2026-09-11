// ================================================================
// Company : Quecksilber Technologies
// Purpose : Previews Employee imports through the existing permission pipeline.
// ================================================================

using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;

#region Query

/// <summary>Requests a saved-draft Employee import preview using the existing module permission context.</summary>
public sealed record PreviewEmployeeImportQuery(BulkImportPreviewRequestDTO DTO)
    : IRequest<ApiResponse<BulkImportPreviewResponseDTO>>;

#endregion

#region Handler

/// <summary>Validates source data without creating masters, assigning roles or granting permissions.</summary>
public sealed class PreviewEmployeeImportQueryHandler(BulkImportWorkflowService previewService)
    : IRequestHandler<PreviewEmployeeImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>
{
    public async Task<ApiResponse<BulkImportPreviewResponseDTO>> Handle(
        PreviewEmployeeImportQuery request,
        CancellationToken cancellationToken)
    {
        var preview = await previewService.PreviewAsync(
            BulkImportMaster.Employee,
            request.DTO,
            cancellationToken);
        return ApiResponse<BulkImportPreviewResponseDTO>.Success(
            preview,
            "Draft saved. Review and confirm to queue import. No master records have been created.");
    }
}

#endregion

