// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant-owned asset types.
// ================================================================

using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Defines persistence operations for asset types.
/// </summary>
public interface IAssetTypeRepository
{
    #region Queries

    /// <summary>
    /// Retrieves asset types for the supplied tenant and client filters.
    /// </summary>
    /// <param name="tenantId">The trusted tenant identifier.</param>
    /// <param name="dto">The client-supplied filters.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A paged collection of asset types.</returns>
    Task<PagedResponseDTO<GetTypeResponseDTO>> GetAllAsync(
        long tenantId,
        GetTypeRequestDTO dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an active, non-deleted asset type owned by a tenant.
    /// </summary>
    /// <param name="id">The asset type identifier.</param>
    /// <param name="tenantId">The trusted tenant identifier.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>The asset type when it exists for the tenant; otherwise, <see langword="null"/>.</returns>
    Task<AssetType?> GetByIdForTenantAsync(
        long id,
        long tenantId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Create

    /// <summary>
    /// Persists a new asset type entity.
    /// </summary>
    /// <param name="entity">The fully prepared asset type entity.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>The persisted asset type.</returns>
    Task<AssetType?> CreateAsync(AssetType entity, CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    /// Persists changes to an existing asset type entity.
    /// </summary>
    /// <param name="entity">The tenant-owned asset type entity to update.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns><see langword="true"/> when changes were saved; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateAsync(AssetType entity, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>
    /// Soft-deletes an asset type owned by a tenant.
    /// </summary>
    /// <param name="id">The asset type identifier.</param>
    /// <param name="tenantId">The trusted tenant identifier.</param>
    /// <param name="employeeId">The authenticated employee performing the deletion.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns><see langword="true"/> when the type was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(
        long id,
        long tenantId,
        long employeeId,
        CancellationToken cancellationToken = default);

    #endregion
}
