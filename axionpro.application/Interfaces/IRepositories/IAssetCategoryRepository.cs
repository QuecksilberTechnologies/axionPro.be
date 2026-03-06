using axionpro.application.DTOS.AssetDTO.category;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetCategoryRepository
    {

      

        #region AssetCategory        
        Task<List<GetCategoryResponseDTO>> GetAllAsync(GetCategoryReqestDTO? dto);
        Task<List<GetCategoryResponseDTO>> AddAsync(AddCategoryReqestDTO? dtO);
        Task<bool> UpdateAsync(UpdateCategoryReqestDTO? dtO);
        Task<bool> DeleteAsync(DeleteCategoryReqestDTO dtO);

      
        #endregion
    }
}
