using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class EntitySecurityValidator : IEntitySecurityValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public EntitySecurityValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HasPermissionAsync(long userId, string permissionCode)
        {
            //// Fetch user's roles and permissions
            //return await _unitOfWork.UserRoleRepository
            //       .HasPermission(userId, permissionCode);
            return false;
        }

        public async Task<bool> IsTenantValidAsync(long userId, long? tenantId)
        {
            // return await _unitOfWork.TenantRepository.IsValidTenant(userId, tenantId);
            return false;
        }

        public async Task<bool> IsDuplicateAssetTypeAsync(long? tenantId, string typeName)
        {
          //  return await _unitOfWork.AssetRepository.CheckDuplicateAssetType(tenantId, typeName);
            return false;
        }
    }

}
