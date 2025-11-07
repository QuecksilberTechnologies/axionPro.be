using axionpro.application.Constants;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
 
using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<UserRoleRepository>? _logger;

        public UserRoleRepository(WorkforceDbContext context, ILogger<UserRoleRepository>? logger)
        {
            _context = context;
             _logger = logger;
        }

        public async Task<List<UserRole>> GetUsersRoleByIdAsync(long userId)
        {
            try
            {
                _logger?.LogInformation("Fetching roles for user with ID: {UserId}", userId);

                    // Fetch all roles for the user with related role details if necessary
                var userRoles = await _context.UserRoles.Where(ur => ur.EmployeeId == userId)
                                                .ToListAsync();

                // If no roles are found, log a warning and return null
                if (userRoles == null || userRoles.Count == 0)
                {
                    _logger?.LogWarning("No roles found for user with ID: {UserId}", userId);
                    return null;
                }


                // Log the total number of roles fetched for the user
                 _logger?.LogInformation("Fetched {Count} roles for user with ID: {UserId}", userRoles.Count, userId);
                return userRoles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while fetching the roles for user with ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<int?> AddUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return userRole.RoleId;
        }

        public async Task DeleteUserRoleAsync(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserRole>> GetAllUserRolesAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole> GetUserRoleByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<int?> UpdateUserRoleAsync(UserRole userRole)
        {
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
            return userRole.RoleId;
        }
        public async Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(long employeeId, long? tenantId)
        {
            try
            {
                _logger?.LogInformation("Fetching active roles for EmployeeId: {EmployeeId}", employeeId);

                var userRoles = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur =>
                        ur.EmployeeId == employeeId &&
                        ur.IsActive == ConstantValues.IsByDefaultTrue &&
                        (ur.IsSoftDeleted != ConstantValues.IsByDefaultTrue) &&
                        ur.Role != null &&
                        ur.Role.IsActive == ConstantValues.IsByDefaultTrue &&
                        (ur.Role.IsSoftDeleted != ConstantValues.IsByDefaultTrue) &&
                        ur.Role.TenantId == tenantId)
                    .ToListAsync();

                if (userRoles.Count == 0)
                {
                    _logger?.LogWarning("No active roles found for EmployeeId: {EmployeeId}", employeeId);
                    return new List<UserRole>();
                }

                _logger?.LogInformation("Fetched {Count} active roles for EmployeeId: {EmployeeId}", userRoles.Count, employeeId);
                return userRoles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching user roles for EmployeeId: {EmployeeId}", employeeId);
                return new List<UserRole>();
            }
        }



    }
}
