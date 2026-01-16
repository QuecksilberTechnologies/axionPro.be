 

using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
