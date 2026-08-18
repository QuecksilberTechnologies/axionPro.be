// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Ticket Classification Repository.
// ================================================================

using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace axionpro.application.Interfaces.IRepositories
{
   
        /// <summary>
        /// Defines the contract for TicketClassificationRepository.
        /// </summary>
        public interface ITicketClassificationRepository
        {
        /// <summary>
        /// Add new ticket classification.
        /// </summary>
           Task<TicketClassification?> AddAsync(TicketClassification entity);

        /// <summary>
        /// Get classification by Id.
        /// </summary>
           Task<GetClassificationResponseDTO?> GetByIdAsync(long id, long tenantId);

            /// <summary>
            /// Get all active classifications.
            /// </summary>
            Task<PagedResponseDTO<GetClassificationResponseDTO>> GetAllAsync(long tenantId, GetAllClassificationRequestDTO dto);
            Task<List<GetClassificationResponseDTO>> GetDDLAsync(bool isActive, long tenantId);

            /// <summary>
            /// Soft delete classification (mark inactive).
            /// </summary>
            Task<bool> DeleteAsync(DeleteClassificationRequestDTO dTO, long employeeId);

            /// <summary>
            /// Update existing classification details.
            /// </summary>
            Task<TicketClassification?> GetByIdForTenantAsync(int id, long tenantId);
            Task<TicketClassification?> UpdateAsync(TicketClassification entity);
        }
   
}
