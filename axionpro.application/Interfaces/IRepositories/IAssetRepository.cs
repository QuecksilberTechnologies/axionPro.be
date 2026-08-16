

// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant-owned assets.
// ================================================================

using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetRepository
    {


        #region asset
        Task<GetAssetResponseDTO> UpdateAsync(Asset asset, string path);
        Task<GetAssetResponseDTO> AddAsync(Asset asset, string path);
    
        Task UpdateQrCodeAsync(long Id, string qrJson);
        // All assets
        Task<List<GetAssetResponseDTO>> GetAllAsync(long tenantId, bool Isactive);
       
        public Task<Asset> GetSingleRecordAsync(long Id, bool? IsActive);  // Ensure this returns 

        Task<List<GetAssetResponseDTO>> GetInsertedAssetAsync(long tenantId, bool Isactive);
        Task<PagedResponseDTO<GetAssetResponseDTO>> GetAssetsByFilterAsync(
            long tenantId,
            GetAssetRequestDTO asset,
            CancellationToken cancellationToken = default);

        Task<Asset?> GetSingleRecordForTenantAsync(
            long id,
            long tenantId,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAssetAsync(
            long id,
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default);
        
        #endregion
        //Task<List<AssetCategory>> AddAssetCategoryAsync(AddCategoryRequestDTO asset);
 
        

         
    }
}
