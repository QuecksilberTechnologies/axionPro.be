// ================================================================
// Company : Quecksilber Technologies
// Purpose : Previews Designation imports through the existing permission pipeline.
// ================================================================

using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.DesignationCmd.Handlers;

#region Query

/// <summary>Requests a saved-draft Designation import preview using the existing module permission context.</summary>
public sealed record PreviewDesignationImportQuery(BulkImportPreviewRequestDTO DTO)
    : IRequest<ApiResponse<BulkImportPreviewResponseDTO>>;

#endregion

#region Handler

/// <summary>Validates source data without creating masters, assigning roles or granting permissions.</summary>
public sealed class PreviewDesignationImportQueryHandler(BulkImportWorkflowService previewService)
    : IRequestHandler<PreviewDesignationImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>
{
    public async Task<ApiResponse<BulkImportPreviewResponseDTO>> Handle(
        PreviewDesignationImportQuery request,
        CancellationToken cancellationToken)
    {
        var preview = await previewService.PreviewAsync(
            BulkImportMaster.Designation,
            request.DTO,
            cancellationToken);
        return ApiResponse<BulkImportPreviewResponseDTO>.Success(
            preview,
            "Draft saved. Review and confirm to queue import. No master records have been created.");
    }
}

#endregion
