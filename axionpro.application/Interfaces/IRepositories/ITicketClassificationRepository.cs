using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
   
        public interface ITicketClassificationRepository
        {
        /// <summary>
        /// Add new ticket classification.
        /// </summary>
           Task<List<GetClassificationResponseDTO>> AddAsync(AddClassificationRequestDTO dTO);

            /// <summary>
            /// Get classification by Id.
            /// </summary>
            Task<GetClassificationResponseDTO?> GetByIdAsync(GetClassificationRequestDTO dTO);

            /// <summary>
            /// Get all active classifications.
            /// </summary>
            Task<List<GetClassificationResponseDTO>> GetAllAsync(GetClassificationRequestDTO dTO);

            /// <summary>
            /// Soft delete classification (mark inactive).
            /// </summary>
            Task<bool> DeleteAsync(DeleteClassificationRequestDTO dTO);

            /// <summary>
            /// Update existing classification details.
            /// </summary>
            Task<GetClassificationResponseDTO?> UpdateAsync(UpdateClassificationRequestDTO dTO);
        }
   
}
