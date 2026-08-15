// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-editable values for creating a designation.
// ================================================================

namespace axionpro.application.DTOs.Designation
{
    /// <summary>
    /// Represents the client-supplied values required to create a designation.
    /// </summary>
    public class CreateDesignationRequestDTO
    {
        public int DepartmentId { get; set; }
        public string? DesignationName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
