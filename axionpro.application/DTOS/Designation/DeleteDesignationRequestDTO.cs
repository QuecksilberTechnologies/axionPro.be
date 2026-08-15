// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines a client request to soft delete a designation.
// ================================================================

namespace axionpro.application.DTOs.Designation
{
    /// <summary>
    /// Represents the identifier of a designation to soft delete.
    /// </summary>
    public class DeleteDesignationRequestDTO
    {
        public required int Id { get; set; }
    }
}
