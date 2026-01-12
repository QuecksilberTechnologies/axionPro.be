 

using axionpro.application.DTOS.AssetDTO.asset;
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
        Task UpdateAsync(Asset asset);
        Task AddAsync(Asset asset);
      
        // All assets
        Task<List<GetAssetResponseDTO>> GetAllAssetAsync(long tenantId, bool Isactive);
        Task<List<GetAssetResponseDTO>> GetAssetsByFilterAsync(GetAssetRequestDTO? asset);
        Task<int> DeleteAssetAsync(DeleteAssetReqestDTO? asset);
         Task<bool> UpdateAssetInfoAsync(UpdateAssetRequestDTO assetDto);

        #endregion
        //Task<List<AssetCategory>> AddAssetCategoryAsync(AddCategoryRequestDTO asset);
 
        

         
    }
}
