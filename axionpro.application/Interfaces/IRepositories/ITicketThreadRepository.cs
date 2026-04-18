using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketThreadRepository
    {
        Task<bool> AddAsync(TicketThread thread);
    }
}
