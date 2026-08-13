// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Retrieves effective Host role module-operation permissions.
// ============================================================================

using AutoMapper;
using axionpro.application.DTOS.Host;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Provides read operations over <c>HostRoleModuleAndPermission</c> assignments.
    /// </summary>
    public class HostRolePermissionRepository : IHostRolePermissionRepository
    {
        #region Fields

        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HostRolePermissionRepository> _logger;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HostRolePermissionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used to read Host permissions.</param>
        /// <param name="mapper">The mapper retained for consistency with the Host repository construction convention.</param>
        /// <param name="logger">The logger retained for repository diagnostics.</param>
        /// <param name="passwordService">The password service retained for repository construction consistency.</param>
        /// <param name="encryptionService">The encryption service retained for repository construction consistency.</param>
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

        #endregion

        #region Host Permission Operations

        /// <inheritdoc />
        public Task<List<HostUserPermissionResponseDTO>> GetHostUserPermissionsAsync(
            long hostRoleId,
            CancellationToken cancellationToken = default)
        {
            return _context.HostRoleModuleAndPermissions
                .AsNoTracking()
                .Where(permission =>
                    permission.HostRoleId == hostRoleId &&
                    permission.IsActive &&
                    !permission.IsSoftDeleted)
                .OrderBy(permission => permission.Module.ModuleName)
                .ThenBy(permission => permission.Operation.OperationName)
                .Select(permission => new HostUserPermissionResponseDTO
                {
                    ModuleId = permission.ModuleId,
                    ModuleName = permission.Module.ModuleName,
                    DisplayName = permission.Module.DisplayName,
                    OperationId = permission.OperationId,
                    OperationName = permission.Operation.OperationName
                })
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<HostRoleModuleAndPermission>> GetByHostRoleIdAsync(
            long hostRoleId,
            CancellationToken cancellationToken = default)
        {
            return _context.HostRoleModuleAndPermissions
                .Where(permission => permission.HostRoleId == hostRoleId)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task BulkInsertAsync(
            List<HostRoleModuleAndPermission> permissions,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(permissions);

            if (permissions.Count == 0)
            {
                return;
            }

            await _context.HostRoleModuleAndPermissions.AddRangeAsync(permissions, cancellationToken);
        }

        #endregion
    }
}
