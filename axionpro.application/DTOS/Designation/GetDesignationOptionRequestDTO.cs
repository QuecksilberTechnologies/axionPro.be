// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-supplied filters for designation options.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Designation
{
    /// <summary>
    /// Represents filters for active designation options.
    /// </summary>
    public class GetDesignationOptionRequestDTO : PermissionRequestDTO
    {
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
