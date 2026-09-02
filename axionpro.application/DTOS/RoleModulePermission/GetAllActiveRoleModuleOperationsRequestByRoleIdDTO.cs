// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving role module operations.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.RoleModulePermission
{
    /// <summary>
    /// Defines the role identifier used to retrieve assigned module operations.
    /// </summary>
    public class GetAllActiveRoleModuleOperationsRequestByRoleIdDTO : PermissionRequestDTO
    {
        public int RoleId { get; set; }
    }
}
