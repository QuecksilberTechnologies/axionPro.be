// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines filtering criteria for tenant role option queries.
// ================================================================
using axionpro.application.DTOs.BaseDTO;

    /// <summary>
    /// Defines client-editable filtering criteria for role option queries.
    /// </summary>
namespace axionpro.application.DTOS.Role
{
   
    public class GetRoleOptionRequestDTO : PermissionRequestDTO
    {

        public int? RoleType { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
