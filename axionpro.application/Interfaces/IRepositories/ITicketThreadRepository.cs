using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITicketThreadRepository
    {
        Task<long> AddAsync(axionpro.domain.Entity.Thread thread);
    }
}
