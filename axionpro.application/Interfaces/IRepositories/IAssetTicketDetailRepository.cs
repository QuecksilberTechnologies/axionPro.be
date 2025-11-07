

using axionpro.application.DTOS.TicketDTO.AssetTicketMap;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{

    public interface IAssetTicketDetailRepository
  {
    /// <summary>
    /// Add new AssetTicketType .
    /// </summary>
    public Task<List<GetAssetTicketResponseDTO>> AddAsync(AddTicketTypeRequestDTO dTO);

      
        /// <summary>
        /// Get all AssetTicketType By filter header.
        /// </summary>
        Task<List<GetAssetTicketResponseDTO>> GetAllByFilerAsync(GetAssetTicketFilterRequestDTO dTO);

    /// <summary>
    /// Soft delete AssetTicketType (mark inactive).
    /// </summary>
    Task<bool> DeleteAsync(DeleteAssetTicketRequestDTO dTO);
         
        /// <summary>
        /// Update existing AssetTicketType details.
        /// </summary>
        Task<GetAssetTicketResponseDTO?> UpdateAsync(UpdateHeaderRequestDTO dTO);
        
    }

}
