using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEntitySecurityValidator
    {
        Task<bool> HasPermissionAsync(long userId, string permissionCode);
        Task<bool> IsTenantValidAsync(long userId, long? TenantId);
        Task<bool> IsDuplicateAssetTypeAsync(long? TenantId, string typeName);
    }

}
