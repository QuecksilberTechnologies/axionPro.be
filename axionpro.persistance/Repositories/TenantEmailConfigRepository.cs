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
            return await _context.TenantEmailConfigs
                .Include(x => x.Tenant)          // 🔥 YAHI LINE IMPORTANT HAI
                .Include(x => x.Tenant.TenantProfiles) // logo etc ke liye
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.IsActive  && x.Tenant.IsSoftDeleted!=true);
        }

        public Task<TenantEmailConfig?> UpdateEmailConfigAsync(long? tenantId)
        {
            throw new NotImplementedException();
        }
    }

}
