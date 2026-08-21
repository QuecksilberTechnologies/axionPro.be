// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides current Tenant employee role-assignment persistence queries.
// ================================================================

using axionpro.application.Interfaces.IRepositories;

using axionpro.domain.Entity;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace axionpro.persistance.Repositories;

/// <summary>
/// Provides persistence operations for Tenant employee role assignments.
/// </summary>
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

    #region Effective Role Queries

    /// <summary>
    /// Retrieves non-deleted role assignments for an employee.
    /// </summary>
    /// <param name="userId">The employee identifier.</param>
    /// <returns>The employee's persisted role assignments.</returns>
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

    /// <summary>
    /// Retrieves current active, non-deleted role assignments for an authenticated Tenant employee.
    /// </summary>
    /// <param name="employeeId">The trusted employee identifier.</param>
    /// <param name="tenantId">The trusted Tenant identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the database operation.</param>
    /// <returns>The effective role assignments with active, non-deleted Tenant roles.</returns>
    public async Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(
        long employeeId,
        long? tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Fetching roles for EmployeeId: {EmployeeId}", employeeId);

            var userRoles = await _context.UserRoles
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.IsActive == true &&
                    x.IsSoftDeleted != true &&
                    x.Role != null &&
                    x.Role.IsActive == true &&
                    x.Role.IsSoftDeleted != true &&
                    x.Role.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            return userRoles ?? new List<UserRole>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error fetching role details for EmployeeId: {EmployeeId}", employeeId);
            throw;
        }
    }

    #endregion

    #region Role Assignment Writes

    /// <summary>
    /// Adds role assignments to the current unit-of-work change set.
    /// </summary>
    /// <param name="entities">The role assignments to add.</param>
    /// <returns>A task that completes after the assignments are staged.</returns>
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

    /// <summary>
    /// Marks role assignments for update in the current unit-of-work change set.
    /// </summary>
    /// <param name="entities">The role assignments to update.</param>
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

    #endregion
}
