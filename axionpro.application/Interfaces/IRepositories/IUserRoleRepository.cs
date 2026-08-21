
// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for Tenant employee role assignments.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for Tenant employee role assignments.
    /// </summary>
    public interface IUserRoleRepository
    {
        #region Effective Role Queries

        /// <summary>
        /// Retrieves non-deleted role assignments for an employee.
        /// </summary>
        /// <param name="userId">The employee identifier.</param>
        /// <returns>The employee's persisted role assignments.</returns>
        Task<List<UserRole>> GetUsersRoleByIdAsync(long userId);

        /// <summary>
        /// Retrieves the current active, non-deleted role assignments for a Tenant employee.
        /// </summary>
        /// <param name="employeeId">The authenticated employee identifier.</param>
        /// <param name="tenantId">The validated Tenant identifier that owns the effective roles.</param>
        /// <param name="cancellationToken">A token used to cancel the database operation.</param>
        /// <returns>The current effective assignments with their active Tenant roles.</returns>
        Task<List<UserRole>> GetEmployeeRolesWithDetailsByIdAsync(
            long employeeId,
            long? tenantId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Role Assignment Writes

        /// <summary>
        /// Adds role assignments to the current unit-of-work change set.
        /// </summary>
        /// <param name="entities">The role assignments to add.</param>
        /// <returns>A task that completes after the assignments are staged.</returns>
        Task AddRangeAsync(List<UserRole> entities);

        /// <summary>
        /// Marks role assignments for update in the current unit-of-work change set.
        /// </summary>
        /// <param name="entities">The role assignments to update.</param>
        void UpdateRange(List<UserRole> entities);

        #endregion
    }
     

}
