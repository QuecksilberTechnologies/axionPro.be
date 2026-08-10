// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Persists and retrieves host users.
// ================================================================

using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Provides persistence operations for host users.
    /// </summary>
    public class HostUserRepository : IHostUserRepository
    {
        #region Fields

        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HostUserRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HostUserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for persistence.</param>
        /// <param name="mapper">The mapper supplied to the repository.</param>
        /// <param name="logger">The logger supplied to the repository.</param>
        /// <param name="passwordService">The password service supplied to the repository.</param>
        /// <param name="encryptionService">The encryption service supplied to the repository.</param>
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

        #endregion

        #region Host User Methods

        /// <summary>
        /// Retrieves an active, non-soft-deleted host user by login identifier.
        /// </summary>
        /// <param name="loginId">The login identifier to search for.</param>
        /// <returns>The matching host user, or <see langword="null"/> when no active match exists.</returns>

        public async Task<HostUser?> GetByLoginIdAsync(string loginId)
        {
            return await _context.HostUsers
                .FirstOrDefaultAsync(x =>
                    x.LoginId == loginId &&
                    x.IsActive &&
                    !x.IsSoftDeleted);
        }

        /// <summary>
        /// Retrieves a non-soft-deleted host user by identifier with its host role.
        /// </summary>
        /// <param name="id">The host-user identifier.</param>
        /// <returns>The matching host user, or <see langword="null"/> when it is not found.</returns>
        public async Task<HostUser?> GetByIdAsync(long id)
        {
            return await _context.HostUsers
                .AsNoTracking()
                .Include(x => x.HostRole)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsSoftDeleted);
        }

        /// <summary>
        /// Retrieves all non-soft-deleted host users with their host roles.
        /// </summary>
        /// <returns>A list of host users, which is empty when no matching users exist.</returns>
        public async Task<List<HostUser>> GetAllAsync()
        {
            return await _context.HostUsers
                .AsNoTracking()
                .Include(x => x.HostRole)
                .Where(x => !x.IsSoftDeleted)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Determines whether an active, non-soft-deleted host user is assigned to a host role.
        /// </summary>
        /// <param name="hostRoleId">The host-role identifier to check.</param>
        /// <returns><see langword="true"/> when at least one active host user is assigned; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> AnyActiveUserByHostRoleIdAsync(long hostRoleId)
        {
            return await _context.HostUsers.AnyAsync(x =>
                x.HostRoleId == hostRoleId &&
                x.IsActive &&
                !x.IsSoftDeleted);
        }

        /// <summary>
        /// Adds a host user and persists it using the repository's existing convention.
        /// </summary>
        /// <param name="entity">The host-user entity to add.</param>
        /// <returns>The persisted host-user entity.</returns>
        public async Task<HostUser> AddAsync(HostUser entity)
        {
            await _context.HostUsers.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Updates a host user and persists the changes using the repository's existing convention.
        /// </summary>
        /// <param name="entity">The host-user entity to update.</param>
        /// <returns>The persisted host-user entity.</returns>
        public async Task<HostUser> UpdateAsync(HostUser entity)
        {
            _context.HostUsers.Update(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Persists a host user's prepared soft-delete state.
        /// </summary>
        /// <param name="entity">The host-user entity marked for soft deletion.</param>
        /// <returns><see langword="true"/> when at least one record is persisted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteAsync(HostUser entity)
        {
            _context.HostUsers.Update(entity);

            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
