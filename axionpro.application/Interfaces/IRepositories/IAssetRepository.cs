

using axionpro.application.DTOS.AssetDTO.asset;
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
        Task<List<GetAssetResponseDTO>> GetAssetsByFilterAsync(GetAssetRequestDTO? asset);
        Task<bool> DeleteAssetAsync(DeleteAssetReqestDTO? asset);
        
        #endregion
        //Task<List<AssetCategory>> AddAssetCategoryAsync(AddCategoryRequestDTO asset);
 
        

         
    }
}
