using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class TenantEmailConfigRepository : ITenantEmailConfigRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantEmailConfigRepository> _logger;

        public TenantEmailConfigRepository(WorkforceDbContext context, ILogger<TenantEmailConfigRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? tenantId)
        {
            try
            {
                return await _context.TenantEmailConfigs.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching email config for TenantId: {TenantId}", tenantId);
                return null;
            }
        }

        public Task<TenantEmailConfig?> UpdateEmailConfigAsync(long? tenantId)
        {
            throw new NotImplementedException();
        }
    }

}
