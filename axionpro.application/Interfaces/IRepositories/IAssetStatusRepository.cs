// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant-owned asset statuses.
// ================================================================

using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Defines persistence operations for asset statuses.
/// </summary>
public interface IAssetStatusRepository
{
    #region Queries

    /// <summary>
    /// Retrieves asset statuses for the supplied tenant and client filters.
    /// </summary>
    Task<PagedResponseDTO<GetStatusResponseDTO>> GetAllAsync(
        long tenantId,
        GetStatusRequestDTO dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a non-deleted asset status owned by a tenant.
    /// </summary>
    Task<AssetStatus?> GetByIdForTenantAsync(
        int id,
        long tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a non-deleted asset status by its identifier for existing cross-module validation.
    /// </summary>
    Task<AssetStatus?> GetByIdAsync(int? id);

    #endregion

    #region Create

    /// <summary>
    /// Persists a new, fully prepared asset status entity.
    /// </summary>
    Task<AssetStatus?> CreateAsync(AssetStatus entity, CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    /// Persists changes to a tenant-owned asset status entity.
    /// </summary>
    Task<bool> UpdateAsync(AssetStatus entity, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>
    /// Soft-deletes an asset status owned by a tenant.
    /// </summary>
    Task<bool> DeleteAsync(
        int id,
        long tenantId,
        long employeeId,
        CancellationToken cancellationToken = default);

    #endregion
}
