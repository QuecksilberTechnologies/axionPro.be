using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class HostRolePermissionRepository : IHostRolePermissionRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HostRolePermissionRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public HostRolePermissionRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<HostRolePermissionRepository> logger,
            IPasswordService passwordService,
            IEncryptionService encryptionService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _passwordService = passwordService;
            _encryptionService = encryptionService;
        }
    }
}
