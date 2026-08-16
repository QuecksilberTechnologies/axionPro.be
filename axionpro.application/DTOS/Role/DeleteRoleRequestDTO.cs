// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines a request to soft delete a tenant role.
// ================================================================

namespace axionpro.application.DTOs.Role
{
    /// <summary>
    /// Represents the identifier of a tenant role to soft delete.
    /// </summary>
    public class DeleteRoleRequestDTO
    {
        public required int Id { get; set; }
    }
}
