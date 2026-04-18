using axionpro.domain.Entity;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketAttachmentRepository
    {
        Task AddAsync(TicketAttachment ticketAttachment);
        Task<TicketAttachment> GetByIdAsync(long id);
    }

   

    
}
