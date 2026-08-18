// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Ticket Header Repository.
// ================================================================



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

    /// <summary>
    /// Defines the contract for TicketHeaderRepository.
    /// </summary>
    public interface ITicketHeaderRepository
    {
        /// <summary>
        /// Add new ticket header.
        /// </summary>
        Task<TicketHeader?> AddAsync(TicketHeader entity);

        /// <summary>
        /// Get header by Id.
        /// </summary>
        Task<List<GetHeaderResponseDTO>> GetAllHeaderAsync(GetHeaderRequestDTO dto);
        Task<GetHeaderResponseDTO> GetByIdAsync(long headerId);
        Task<List<GetHeaderResponseDTO>> GetByClassificationIdAsync(long tenantId, GetTicketHeaderByClassifyIdRequestDTO dto);
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
        Task<TicketHeader?> GetByIdForTenantAsync(long id, long tenantId);
        Task<TicketHeader?> UpdateAsync(TicketHeader entity);
        
    }

}
