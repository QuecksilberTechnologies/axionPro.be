// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Ticket Type Repository.
// ================================================================

using axionpro.application.DTOs.PageTypeEnum;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketTypeRepository
    {


        Task<List<GetDDLTicketTypeResponseDTO>> GetDDLAsync(bool isActive, long tenantId);
        public Task<TicketType?> AddAsync(TicketType entity);
        public Task<PagedResponseDTO<GetTicketTypeResponseDTO>> AllAsync(long tenantId, GetTicketTypeRequestDTO dTO);
        public Task<List<GetTicketTypeResponseDTO>> AllByHeaderIdAsync(long tenantId, GetTicketTypeByHeaderIdRequestDTO dTO);
        public Task<List<GetTicketTypeRoleResponseDTO>> AllByRoleIdAsync(GetTicketTypeByRoleIdRequestDTO dTO);
        public Task<GetTicketTypeResponseDTO?> GetByIdAsync(long id, bool isActive);
        public  Task<bool> DeleteAsync(long id, long employeeId);
        public Task<TicketType?> GetByIdForTenantAsync(long id, long tenantId);
        public Task<TicketType?> UpdateAsync(TicketType entity);




    }
}
