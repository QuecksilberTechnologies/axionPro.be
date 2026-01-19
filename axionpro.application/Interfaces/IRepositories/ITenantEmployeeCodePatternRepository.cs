using axionpro.application.DTOS.Tenant;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantEmployeeCodePatternRepository
    {
        /// <summary>
        /// Gets the active employee code pattern for a specific tenant.
        /// </summary>
        Task<GetEmployeeCodePatternResponseDTO?> GetTenantEmployeeCodePatternAsync(long tenantId ,bool IsActive);

        /// <summary>
        /// Creates a new employee code pattern for a tenant.
        /// </summary>
        Task<bool> CreatePatternAsync(EmployeeCodePattern entity);

        /// <summary>
        /// Updates an existing employee code pattern.
        /// </summary>
        Task<bool> UpdatePatternAsync(EmployeeCodePattern entity);

        /// <summary>
        /// Deactivates all existing patterns for a tenant.
        /// (Ensures only one active pattern exists)
        /// </summary>
        Task<bool> DeactivateExistingPatternsAsync(long tenantId);

        /// <summary>
        /// Increments the running number and returns the updated sequence atomically.
        /// (This avoids race conditions when generating employee codes)
        /// </summary>
        Task<int> IncrementAndGetNextRunningNumberAsync(long tenantId);

        /// <summary>
        /// Generates employee code from pattern (prefix/year/month/etc.)
        /// </summary>
        Task<string> GenerateEmployeeCodeAsync(long tenantId, int? departmentId = null);
    }

}
