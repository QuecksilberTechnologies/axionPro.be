using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class ThreadMessageRepository : IThreadMessageRepository
    {
        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<ThreadMessageRepository> _logger;

        public ThreadMessageRepository(
            WorkforceDbContext context,
            ILogger<ThreadMessageRepository> logger,
            IMapper mapper
          )
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }
        public Task<List<GetHeaderResponseDTO>> AddAsync(AddHeaderRequestDTO dTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(DeleteHeaderRequestDTO dTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetHeaderResponseDTO>> GetAllAsync(GetHeaderRequestDTO dTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetHeaderResponseDTO>> GetAllHeaderAsync(GetHeaderRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetHeaderResponseDTO>> GetByClassificationIdAsync(GetTicketHeaderByClassifyIdRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<GetHeaderResponseDTO> GetByIdAsync(long headerId)
        {
            throw new NotImplementedException();
        }

        public Task<GetHeaderResponseDTO?> UpdateAsync(UpdateHeaderRequestDTO dTO)
        {
            throw new NotImplementedException();
        }
    }
}
