
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUserRoleRepository
    {
        // Get role name by user ID (for your login process)
        Task<List<UserRole>> GetUsersRoleByIdAsync(long userId);
        Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(long employeeId, long? TenantId);
        // 🔥 BULK INSERT
        Task AddRangeAsync(List<UserRole> entities);

        // 🔥 BULK UPDATE
        void UpdateRange(List<UserRole> entities);

 
 
        
    }
     

}
