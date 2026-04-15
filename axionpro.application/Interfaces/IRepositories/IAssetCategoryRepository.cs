using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetCategoryRepository
    {

      

        #region AssetCategory        
        Task<PagedResponseDTO<GetCategoryResponseDTO>> GetAllAsync(GetCategoryReqestDTO? dto);
        Task<GetCategoryResponseDTO> AddAsync(AddCategoryReqestDTO? dtO);
        Task<bool> UpdateAsync(UpdateCategoryReqestDTO? dtO);
        Task<bool> DeleteAsync(DeleteCategoryReqestDTO dtO);

      
        #endregion
    }
}
