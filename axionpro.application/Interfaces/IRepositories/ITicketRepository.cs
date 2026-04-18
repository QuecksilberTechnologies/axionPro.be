using axionpro.application.DTOS.TicketDTO.Ticket;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketGenrationRepository
    {
        Task AddAsync( axionpro.domain.Entity.Ticket ticket);
 
        Task<GetTicketResponseDTO?> GetByIdAsync(long ticketId);
    }

   

    
}
