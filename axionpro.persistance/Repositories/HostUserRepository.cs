using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories
{
    public class HostUserRepository : IHostUserRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HostUserRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        public HostUserRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<HostUserRepository> logger,
            IPasswordService passwordService,
            IEncryptionService encryptionService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;

            _passwordService = passwordService;
            _encryptionService = encryptionService;
        }

        public async Task<HostUser?> GetByLoginIdAsync(string loginId)
        {
            return await _context.HostUsers
                .FirstOrDefaultAsync(x =>
                    x.LoginId == loginId &&
                    x.IsActive &&
                    !x.IsSoftDeleted);
        }

        public async Task<HostUser> AddAsync(HostUser entity)
        {
            await _context.HostUsers.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
