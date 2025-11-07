using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEmailConfigRepository
    {
        Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? TenantId);
        Task<TenantEmailConfig?> UpdateEmailConfigAsync(long? TenantId);
    }
}
