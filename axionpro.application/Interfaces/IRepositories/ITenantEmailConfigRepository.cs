
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEmailConfigRepository
    {
        Task<TenantEmailConfig?> GetActiveEmailConfigAsync(long? TenantId);
        Task<TenantEmailConfig?> UpdateEmailConfigAsync(TenantEmailConfig? config);
        Task<TenantEmailConfig?> InsertEmailConfigAsync(TenantEmailConfig? config);
    }
}
