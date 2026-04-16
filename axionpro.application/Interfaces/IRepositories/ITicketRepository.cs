using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketRepository
    {
        Task AddAsync( axionpro.domain.Entity.Ticket ticket);
        Task<axionpro.domain.Entity.Ticket> GetByIdAsync(long typeId);
    }

   

    
}
