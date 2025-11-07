using AutoMapper;
using axionpro.application.DTOs;
 
using axionpro.application.Interfaces.ICacheService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ILogger<PermissionRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly ICacheService _cacheService;

        public PermissionRepository(
            IDbContextFactory<WorkforceDbContext> contextFactory,
            ILogger<PermissionRepository> logger,
            ICacheService cacheService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Fetches permissions based on RoleId (uses Redis cache for performance)
        /// </summary>
        public async Task<List<string>> GetPermissionsByRoleAsync(int roleId)
        {
            try
            {
                string cacheKey = $"RolePermissions_{roleId}";
                var cachedPermissions = await _cacheService.GetAsync<List<string>>(cacheKey);

                if (cachedPermissions != null)
                {
                    _logger.LogInformation($"✅ Cache hit for RoleId {roleId}");
                    return cachedPermissions;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();
                var permissions = await (
                    from rp in context.RoleModuleAndPermissions
                    join op in context.TenantEnabledOperations
                        on rp.OperationId equals op.OperationId
                    where rp.RoleId == roleId
                          && rp.HasAccess==true
                          && rp.IsActive == true
                          && op.IsEnabled == true                          
                    select op.OperationId.ToString()
                ).ToListAsync();


                permissions ??= new List<string>();

                if (permissions.Any())
                {
                    await _cacheService.SetAsync(cacheKey, permissions, 30); // Cache for 30 minutes
                    _logger.LogInformation($"🧠 Cached RolePermissions_{roleId}");
                }

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permissions for RoleId {RoleId}", roleId);
                throw;
            }
        }

        /// <summary>
        /// Clears cache when role or permissions change
        /// </summary>
        public async Task InvalidateRoleCacheAsync(int roleId)
        {
            string cacheKey = $"RolePermissions_{roleId}";
            await _cacheService.RemoveAsync(cacheKey);
            _logger.LogInformation($"🧹 Cache invalidated for RoleId {roleId}");
        }
    }
}
