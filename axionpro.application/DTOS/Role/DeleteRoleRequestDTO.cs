// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines a request to soft delete a tenant role.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Role
{
    /// <summary>
    /// Represents the identifier of a tenant role to soft delete.
    /// </summary>
    public class DeleteRoleRequestDTO : PermissionRequestDTO
    {
        public required int Id { get; set; }
    }
}
