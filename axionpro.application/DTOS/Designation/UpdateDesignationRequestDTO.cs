// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-editable values for updating a designation.
// ================================================================

namespace axionpro.application.DTOs.Designation
{
    /// <summary>
    /// Represents the client-editable values for an existing designation.
    /// </summary>
    public class UpdateDesignationRequestDTO
    {
        public required int Id { get; set; }
        public int DepartmentId { get; set; }
        public string? DesignationName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
