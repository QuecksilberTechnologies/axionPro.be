// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-editable values for updating a tenant role.
// ================================================================

namespace axionpro.application.DTOs.Role
{
    /// <summary>
    /// Represents client-editable values for an existing tenant role.
    /// </summary>
    public class UpdateRoleRequestDTO
    {
        public required int Id { get; set; }
        public string? RoleName { get; set; }
        public int RoleType { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
    }
}
