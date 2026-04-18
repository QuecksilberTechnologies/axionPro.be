using AutoMapper;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class TicketGenrationRepository : ITicketGenrationRepository
    {
        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<TicketGenrationRepository> _logger;

        public TicketGenrationRepository(
            WorkforceDbContext context,
            ILogger<TicketGenrationRepository> logger,
            IMapper mapper)

        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }
        public Task AddAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetByIdAsync(long typeId)
        {
            throw new NotImplementedException();
        }
    }
}
