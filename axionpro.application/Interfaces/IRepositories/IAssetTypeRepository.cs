using axionpro.application.DTOS.AssetDTO.type;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAssetTypeRepository
    {

      

        #region AsseType      
        Task<List<GetTypeResponseDTO>> GetAllAsync(GetTypeRequestDTO? dto);
      
        Task<List<GetTypeResponseDTO>> AddAsync(AddTypeRequestDTO? dtO);
        Task<bool> UpdateAsync(UpdateTypeRequestDTO? dtO);
        Task<bool> DeleteAsync(DeleteTypeRequestDTO dtO);

      
        #endregion
    }
}
