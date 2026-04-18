using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Ticket;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
        public async Task<bool> AddAsync(Ticket entity)
        {
            await _context.Ticket.AddAsync(entity);
            return true;
        }

      
        Task ITicketGenrationRepository.AddAsync(Ticket ticket)
        {
            return AddAsync(ticket);
        }

        public async Task<GetTicketResponseDTO?> GetByIdAsync(long ticketId)
        {
            return await _context.Ticket
                .Where(t => t.Id == ticketId)
                .Select(t => new GetTicketResponseDTO
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    AddedDateTime = t.AddedDateTime
                })
                .FirstOrDefaultAsync();
        }
    }
}
