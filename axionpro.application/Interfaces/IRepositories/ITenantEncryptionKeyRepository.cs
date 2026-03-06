
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEncryptionKeyRepository
    {
      public  Task<TenantEncryptionKey> GetActiveKeyByTenantIdAsync(long tenantId);
      public  Task<int> AddAsync(TenantEncryptionKey tenantKey);
      public  Task UpdateAsync(TenantEncryptionKey tenantKey);
    }
}
