using axionpro.application.Interfaces.ICacheService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.infrastructure.CacheMemory
{
    public class TenantKeyMemoryCache : ITenantKeyCache
    {
        private readonly IMemoryCache _cache;

        public TenantKeyMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        private static string CacheKey(long tenantId)
            => $"TENANT_KEY_{tenantId}";

        public string? Get(long tenantId)
        {
            _cache.TryGetValue(CacheKey(tenantId), out string? key);
            return key;
        }

        public void Set(long tenantId, string encryptedKey)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
                SlidingExpiration = TimeSpan.FromHours(2),
                Priority = CacheItemPriority.High
            };

            _cache.Set(CacheKey(tenantId), encryptedKey, options);
        }

        public void Remove(long tenantId)
        {
            _cache.Remove(CacheKey(tenantId));
        }
    }
}
