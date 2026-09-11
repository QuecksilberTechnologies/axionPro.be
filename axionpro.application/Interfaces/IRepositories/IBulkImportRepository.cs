using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.Common;

namespace axionpro.application.Interfaces.IRepositories;

public interface IBulkImportRepository
{
    Task<BulkImportPreviewResponseDTO> PreviewEmployeesAsync(
        BulkImportTableDTO table, string? mappingJson, CommonDecodedResult actor,
        CancellationToken cancellationToken);

    Task<BulkImportPreviewResponseDTO> SaveDraftAsync(
        BulkImportPreviewResponseDTO preview,
        BulkImportPreviewRequestDTO request,
        CommonDecodedResult actor,
        CancellationToken cancellationToken);

    Task<BulkImportJobResponseDTO> ActAsync(
        BulkImportMaster master,
        BulkImportAction action,
        BulkImportJobRequestDTO request,
        CommonDecodedResult actor,
        CancellationToken cancellationToken);

    Task<List<BulkImportJobResponseDTO>> ListAsync(
        BulkImportMaster master,
        BulkImportJobRequestDTO request,
        CommonDecodedResult actor,
        CancellationToken cancellationToken);

    Task<bool> ProcessNextBatchAsync(CancellationToken cancellationToken);

    Task<EmployeeImportInvitationDTO?> ClaimEmployeeInvitationAsync(
        BulkImportJobRequestDTO request, CommonDecodedResult actor, IReadOnlyCollection<int> attemptedRows,
        CancellationToken cancellationToken);

    Task CompleteEmployeeInvitationAsync(BulkImportJobRequestDTO request, CommonDecodedResult actor,
        EmployeeImportInvitationDTO invitation, BulkImportInvitationStatus status, CancellationToken cancellationToken);
}
