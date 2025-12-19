using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.ICacheService
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, int minutes = 30);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }

    public interface ITenantKeyCache
    {
        string? Get(long tenantId);
        void Set(long tenantId, string encryptedKey);
        void Remove(long tenantId);
    }
}
