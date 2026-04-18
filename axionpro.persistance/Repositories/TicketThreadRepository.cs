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
    public class TicketThreadRepository : ITicketThreadRepository
    {
        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<TicketThreadRepository> _logger;

        public TicketThreadRepository(
            WorkforceDbContext context,
            ILogger<TicketThreadRepository> logger,
            IMapper mapper
          )
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<bool> AddAsync(TicketThread entity)
        {
            await _context.TicketThreads.AddAsync(entity);
            return true;
        }
    }
}
