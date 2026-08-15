// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-supplied filters for designation options.
// ================================================================

namespace axionpro.application.DTOS.Designation
{
    /// <summary>
    /// Represents filters for active designation options.
    /// </summary>
    public class GetDesignationOptionRequestDTO
    {
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
