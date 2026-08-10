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

namespace axionpro.persistance.Repositories
{
    public class HostRoleRepository : IHostRoleRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HostRoleRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public HostRoleRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<HostRoleRepository> logger,
            IPasswordService passwordService,
            IEncryptionService encryptionService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _passwordService = passwordService;
            _encryptionService = encryptionService;
        }
        public async Task<HostRole> AddAsync(HostRole entity)
        {
            await _context.HostRoles.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
