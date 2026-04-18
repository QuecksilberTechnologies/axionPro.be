using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class TicketHistoryRepository : ITicketHistoryRepository
    {
        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<TicketHistoryRepository> _logger;

        public TicketHistoryRepository(
            WorkforceDbContext context,
            ILogger<TicketHistoryRepository> logger,
            IMapper mapper
          )
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<bool> AddEntityAsync(TicketHistory entity)
        {
            await _context.TicketHistories.AddAsync(entity);
            return true;
        }
    }
}
