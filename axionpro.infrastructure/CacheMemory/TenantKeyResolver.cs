using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICacheService;
using axionpro.application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.infrastructure.CacheMemory
{
    public class TenantKeyResolver : ITenantKeyResolver
    {
        private readonly ITenantKeyCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public TenantKeyResolver(
            ITenantKeyCache cache,
            IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> ResolveAsync(long tenantId)
        {
            // 1️⃣ Try RAM first
            var key = _cache.Get(tenantId);
            if (!string.IsNullOrEmpty(key))
                return key;

            // 2️⃣ DB hit (only once)
            var tenant = await _unitOfWork.TenantEncryptionKeyRepository.GetActiveKeyByTenantIdAsync(tenantId);

            if (tenant == null)
                throw new UnauthorizedAccessException("Invalid tenant");

            // 3️⃣ Store in RAM
            _cache.Set(tenantId, tenant.EncryptionKey);

            return tenant.EncryptionKey;
        }
    }
}
