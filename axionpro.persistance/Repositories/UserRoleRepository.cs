using axionpro.application.Interfaces.IRepositories;

using axionpro.domain.Entity;

using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace axionpro.persistance.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly WorkforceDbContext _context;
    private readonly ILogger<UserRoleRepository>? _logger;

    public UserRoleRepository(
        WorkforceDbContext context,
        ILogger<UserRoleRepository>? logger)
    {
        _context = context;
        _logger = logger;
    }

    // ==========================================================
    // 🔹 GET USER ROLES BY USER ID
    // ==========================================================
    public async Task<List<UserRole>> GetUsersRoleByIdAsync(long userId)
    {
        try
        {
            _logger?.LogInformation("Fetching roles for user with ID: {UserId}", userId);

            var userRoles = await _context.UserRoles
                .Where(ur => ur.EmployeeId == userId && ur.IsSoftDeleted != true)
                .ToListAsync();

            return userRoles ?? new List<UserRole>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error fetching roles for userId: {UserId}", userId);
            throw;
        }
    }

    // ==========================================================
    // 🔹 GET USER ROLES WITH ROLE DETAILS
    // ==========================================================
    public async Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(long employeeId, long? tenantId)
    {
        try
        {
            _logger?.LogInformation("Fetching roles for EmployeeId: {EmployeeId}", employeeId);

            var userRoles = await _context.UserRoles
                .Include(x => x.Role)
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.IsActive == true &&
                    x.IsSoftDeleted != true &&
                    x.Role != null &&
                    x.Role.IsActive == true &&
                    x.Role.IsSoftDeleted != true &&
                    x.Role.TenantId == tenantId)
                .ToListAsync();

            return userRoles ?? new List<UserRole>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error fetching role details for EmployeeId: {EmployeeId}", employeeId);
            throw;
        }
    }

    // ==========================================================
    // 🔥 BULK INSERT
    // ==========================================================
    public async Task AddRangeAsync(List<UserRole> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                return;

            await _context.UserRoles.AddRangeAsync(entities);

            _logger?.LogInformation("Bulk Insert: {Count} roles", entities.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in bulk insert UserRoles");
            throw;
        }
    }

    // ==========================================================
    // 🔥 BULK UPDATE (USED FOR UPDATE + DELETE)
    // ==========================================================
    public void UpdateRange(List<UserRole> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                return;

            _context.UserRoles.UpdateRange(entities);

            _logger?.LogInformation("Bulk Update: {Count} roles", entities.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in bulk update UserRoles");
            throw;
        }
    }
}