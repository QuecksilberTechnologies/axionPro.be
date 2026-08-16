// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-editable values for creating a tenant role.
// ================================================================

namespace axionpro.application.DTOs.Role
{
    /// <summary>
    /// Represents client-supplied values for a new tenant role.
    /// </summary>
    public class CreateRoleRequestDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public int RoleType { get; set; }
        public string? Remark { get; set; }
        public bool IsActive { get; set; }
    }
}
