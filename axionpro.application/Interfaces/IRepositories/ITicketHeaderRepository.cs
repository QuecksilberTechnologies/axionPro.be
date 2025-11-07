

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

    public interface ITicketHeaderRepository
    {
        /// <summary>
        /// Add new ticket header.
        /// </summary>
     public Task<List<GetHeaderResponseDTO>> AddAsync(AddHeaderRequestDTO dTO);

        /// <summary>
        /// Get header by Id.
        /// </summary>
        Task<List<GetHeaderResponseDTO>> GetByIdAsync(GetHeaderRequestDTO dto);

        /// <summary>
        /// Get all active header.
        /// </summary>
        Task<List<GetHeaderResponseDTO>> GetAllAsync(GetHeaderRequestDTO dTO);

        /// <summary>
        /// Soft delete header (mark inactive).
        /// </summary>
        Task<bool> DeleteAsync(DeleteHeaderRequestDTO dTO);
         
        /// <summary>
        /// Update existing header details.
        /// </summary>
        Task<GetHeaderResponseDTO?> UpdateAsync(UpdateHeaderRequestDTO dTO);
        
    }

}
