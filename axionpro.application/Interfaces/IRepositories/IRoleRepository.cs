// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for tenant roles and role projections.
// ================================================================

using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Role;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for tenant roles and role projections.
    /// </summary>
    public interface IRoleRepository
    {
        Task<GetSingleRoleResponseDTO?> GetByIdAsync1(GetSingleRoleRequestDTO dto);
        Task<ApiResponse<List<GetRoleOptionResponseDTO?>>> GetOptionAsync(GetRoleOptionRequestDTO dto);

        /// <summary>
        /// Soft deletes a tenant role using trusted tenant and actor identifiers.
        /// </summary>
        Task<bool> DeleteAsync(int id, long tenantId, long employeeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a prepared tenant role entity.
        /// </summary>
        Task<Role?> CreateAsync(Role entity, CancellationToken cancellationToken = default);

        Task<PagedResponseDTO<GetRoleResponseDTO>> GetAsync(GetRoleRequestDTO dto);
        Task<List<GetRoleResponseDTO>> GetRoleAsync(long tenantId, int roleTypeId, bool isActive);

        /// <summary>
        /// Gets a mutable role after enforcing tenant ownership.
        /// </summary>
        Task<Role?> GetByIdForTenantAsync(int id, long tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a prepared tenant role entity.
        /// </summary>
        Task<bool> UpdateAsync(Role entity, CancellationToken cancellationToken = default);

        Task<Role> AutoCreatedSingleTenantRoleAsync(Role role);
        Task<bool> AutoCreatedForTenantRoleAsync(List<Role> roles);
        Task<int> AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(long? TenantId, long employeeId, int role);
        Task<Role?> GetTenantAdminRoleAsync(long tenantId);
    }
}
