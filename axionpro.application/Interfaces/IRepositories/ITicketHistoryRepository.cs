

using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{

    public interface ITicketHistoryRepository
    {
        /// <summary>
        /// Add new ticket header.
        /// </summary>
        Task<bool> AddEntityAsync(TicketHistory entity);
        

    }

}
