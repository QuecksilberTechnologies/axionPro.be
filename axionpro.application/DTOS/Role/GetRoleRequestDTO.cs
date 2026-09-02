// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines filtering and paging criteria for tenant role queries.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOs.Role
{
    /// <summary>
    /// Defines client-editable filtering and paging criteria for role queries.
    /// </summary>
    public class GetRoleRequestDTO : PermissionPagedRequestDTO
    {
        public int Id { get; set; }
        public int RoleType { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RoleName { get; set; }
    }
}
