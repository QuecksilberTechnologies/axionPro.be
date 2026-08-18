// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for assigning module operations to a role.
// ================================================================

namespace axionpro.application.DTOs.RoleModulePermission
{
    /// <summary>
    /// Defines client-editable role permission assignments.
    /// </summary>
    public class CreateModuleOperationRolePermissionsRequestDTO
    {
        public int RoleId { get; set; }

        public List<ModulePermissionDTO>? ModuleOperations { get; set; }
    }
}
