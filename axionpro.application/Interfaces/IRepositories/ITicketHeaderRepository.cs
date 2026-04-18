

using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace axionpro.application.Interfaces.IRepositories
{

    public interface ITicketHeaderRepository
    {
        /// <summary>
        /// Add new ticket header.
        /// </summary>
     public Task<GetHeaderResponseDTO> AddAsync(AddHeaderRequestDTO dTO);

        /// <summary>
        /// Get header by Id.
        /// </summary>
        Task<List<GetHeaderResponseDTO>> GetAllHeaderAsync(GetHeaderRequestDTO dto);
        Task<GetHeaderResponseDTO> GetByIdAsync(long headerId);
        Task<List<GetHeaderResponseDTO>> GetByClassificationIdAsync(GetTicketHeaderByClassifyIdRequestDTO dto);
        /// <summary>
        /// Get all active header.
        /// </summary>
        Task<List<GetHeaderResponseDTO>> GetAllAsync(GetHeaderRequestDTO dTO);

        /// <summary>
        /// Soft delete header (mark inactive).
        /// </summary>
        Task<bool> DeleteAsync(DeleteHeaderRequestDTO dTO, long EmployeeId);
         
        /// <summary>
        /// Update existing header details.
        /// </summary>
        Task<GetHeaderResponseDTO?> UpdateAsync(UpdateHeaderRequestDTO dTO);
        
    }

}
