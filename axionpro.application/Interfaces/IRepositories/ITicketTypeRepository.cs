using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketTypeRepository
    {



        public Task<List<GetTicketTypeResponseDTO>> AddAsync(AddTicketTypeRequestDTO dTO);
        public Task<List<GetTicketTypeResponseDTO>> AllAsync(GetTicketTypeRequestDTO dTO);
        public Task<List<GetTicketTypeResponseDTO>> AllByHeaderIdAsync(GetTicketTypeByHeaderIdRequestDTO dTO);
        public Task<List<GetTicketTypeRoleResponseDTO>> AllByRoleIdAsync(GetTicketTypeByRoleIdRequestDTO dTO);
        public Task<GetTicketTypeResponseDTO?> GetByIdAsync(long id);
        public  Task<bool> DeleteAsync(long id, long employeeId);
        public  Task<bool> UpdateAsync(UpdateTicketTypeRequestDTO dto);




    }
}
