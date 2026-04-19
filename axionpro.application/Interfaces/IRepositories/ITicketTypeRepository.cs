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
        public Task<GetTicketTypeResponseDTO> AddAsync(TicketType entity);
        public Task<PagedResponseDTO<GetTicketTypeResponseDTO>> AllAsync(GetTicketTypeRequestDTO dTO);
        public Task<List<GetTicketTypeResponseDTO>> AllByHeaderIdAsync(GetTicketTypeByHeaderIdRequestDTO dTO);
        public Task<List<GetTicketTypeRoleResponseDTO>> AllByRoleIdAsync(GetTicketTypeByRoleIdRequestDTO dTO);
        public Task<GetTicketTypeResponseDTO?> GetByIdAsync(long id, bool isActive);
        public  Task<bool> DeleteAsync(long id, long employeeId);
        public  Task<bool> UpdateAsync(UpdateTicketTypeRequestDTO dto, long employeeId);




    }
}
