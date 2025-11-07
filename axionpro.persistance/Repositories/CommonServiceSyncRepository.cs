using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class CommonServiceSyncRepository : ICommonServiceSyncRepository
    {
        private readonly ILogger<CommonServiceSyncRepository> _logger;

        public CommonServiceSyncRepository(ILogger<CommonServiceSyncRepository> logger)
        {
            _logger = logger;
        }

        public async Task SyncAllTenantsNewModulesAndOperationsAsync()
        {
            // your actual module/operation sync logic here
            _logger.LogInformation("Sync logic started...");

            await Task.CompletedTask;
        }
    }

}
