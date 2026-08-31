// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant-scoped designations.
// ================================================================

using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for tenant-scoped designations.
    /// </summary>
    public interface IDesignationRepository
    {
        /// <summary>
        /// Gets active designation options for a department within a trusted tenant.
        /// </summary>
        Task<List<GetDesignationOptionResponseDTO>> GetOptionAsync(
            int departmentId,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists designation seed entities and returns the executive-office designation identifier.
        /// </summary>
        Task<int> AutoCreateDesignationAsync(List<Designation> designations, int departmentId);

        /// <summary>
        /// Persists a prepared designation entity.
        /// </summary>
        Task<Designation?> CreateAsync(Designation entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes a designation after enforcing tenant ownership.
        /// </summary>
        Task<bool> DeleteDesignationAsync(
            int id,
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether a Designation is still assigned to a non-soft-deleted employee.
        /// Inactive employees are intentionally included because they can be reactivated later.
        /// </summary>
        Task<bool> HasNonDeletedEmployeesAsync(
            int designationId,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets paged designation projections for a trusted tenant.
        /// </summary>
        Task<PagedResponseDTO<GetDesignationResponseDTO>> GetAsync(
            GetDesignationRequestDTO request,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a mutable designation entity after enforcing tenant ownership.
        /// </summary>
        Task<Designation?> GetByIdForTenantAsync(
            int id,
            long tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a prepared designation entity.
        /// </summary>
        Task<bool> UpdateDesignationAsync(Designation entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether a designation name already exists within a tenant.
        /// </summary>
        Task<bool> CheckDuplicateValueAsync(long tenantId, string value);

        /// <summary>
        /// Gets a designation projection by identifier.
        /// </summary>
        Task<GetSingleDesignationResponseDTO?> GetByIdAsync(GetSingleDesignationRequestDTO dto);
    }
}
