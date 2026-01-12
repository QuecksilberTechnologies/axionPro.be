

using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.AssetDTO.status;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetStatusRepository
    {
                
        #region AssetStatus         
        Task<List<GetStatusResponseDTO>> GetAllAsync(GetStatusRequestDTO? assetStatus);        
        Task<GetStatusResponseDTO>AddAsync(AssetStatus? assetStatus);
        Task<bool> UpdateAsync(UpdateStatusRequestDTO assetStatus);
      
        Task<bool>DeleteAsync(DeleteStatusReqestDTO assetStatus);
        #endregion


       
    }
}
