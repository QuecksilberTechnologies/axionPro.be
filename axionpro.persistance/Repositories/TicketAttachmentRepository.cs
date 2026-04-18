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
    public class TicketAttachmentRepository : ITicketAttachmentRepository
    {

        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<TicketAttachmentRepository> _logger;

        public TicketAttachmentRepository(
            WorkforceDbContext context,
            ILogger<TicketAttachmentRepository> logger,
            IMapper mapper)

        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }
        public async Task<bool> AddAsync(TicketAttachment entity)
        {
            await _context.TicketAttachment.AddAsync(entity);
            return true;
        }
        public Task<TicketAttachment> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

       
    }
}
