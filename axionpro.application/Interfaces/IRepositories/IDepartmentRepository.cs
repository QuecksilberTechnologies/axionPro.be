// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant-scoped departments.
// ================================================================

using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for tenant-scoped departments.
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Gets a department response by its identifier.
        /// </summary>
        Task<GetSingleDepartmentResponseDTO?> GetByIdAsync(
            GetSingleDepartmentRequestDTO dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets active department options for a trusted tenant.
        /// </summary>
        Task<List<GetDepartmentOptionResponse>> GetOptionAsync(
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes a department owned by the supplied tenant.
        /// </summary>
        Task<bool> DeleteAsync(
            int id,
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a department using the supplied domain entity.
        /// </summary>
        Task<Department?> CreateAsync(
            Department entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a paged department projection for a trusted tenant.
        /// </summary>
        Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(
            GetDepartmentRequestDTO request,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds department seed entities to the current persistence context.
        /// </summary>
        Task<bool> AutoCreateDepartmentSeedAsync(
            List<Department>? departments,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a mutable department entity after enforcing tenant ownership.
        /// </summary>
        Task<Department?> GetByIdForTenantAsync(
            int id,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists changes to an existing department entity.
        /// </summary>
        Task<bool> UpdateAsync(
            Department entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether a department belongs to a tenant.
        /// </summary>
        Task<bool> ExistsAsync(
            long id,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets department-name to identifier mappings for a tenant.
        /// </summary>
        Task<Dictionary<string, int>> GetDepartmentNameIdMapAsync(
            long tenantId,
            CancellationToken cancellationToken = default);
    }
}
