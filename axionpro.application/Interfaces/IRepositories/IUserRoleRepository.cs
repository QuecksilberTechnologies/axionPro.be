using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUserRoleRepository
    {
        // Get role name by user ID (for your login process)
        Task<List<UserRole>> GetUsersRoleByIdAsync(long userId);
        Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(long employeeId, long? TenantId);       
         

        // CRUD Operations
        // Task<UserRole> GetUserRoleByIdAsync(int id);
        Task<List<UserRole>> GetAllUserRolesAsync();
        Task<int?> AddUserRoleAsync(UserRole userRole);
        Task<int?> UpdateUserRoleAsync(UserRole userRole);
        Task DeleteUserRoleAsync(int id);
    }
     

}
