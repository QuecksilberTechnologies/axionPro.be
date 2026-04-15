


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.Pagination;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetStatusRepository
    {
                
        #region AssetStatus         
        Task<PagedResponseDTO<GetStatusResponseDTO>> GetAllAsync(GetStatusRequestDTO? assetStatus);        
        Task<AssetStatus> GetByIdAsync(int?  id);        
        Task<GetStatusResponseDTO>AddAsync(AssetStatus? assetStatus);
        Task<bool> UpdateAsync(UpdateStatusRequestDTO assetStatus);
      
        Task<bool>DeleteAsync(DeleteStatusReqestDTO assetStatus);
        #endregion


       
    }
}
