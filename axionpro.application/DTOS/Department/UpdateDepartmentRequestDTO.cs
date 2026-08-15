// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-editable fields for updating a department.
// ================================================================

namespace axionpro.application.DTOs.Department
{
    /// <summary>
    /// Represents the client-editable values for an existing department.
    /// </summary>
    public class UpdateDepartmentRequestDTO
    {
        public required int Id { get; set; }
        public string? DepartmentName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? Remark { get; set; }
    }
}
