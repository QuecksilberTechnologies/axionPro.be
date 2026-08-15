// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-supplied filters for designation listings.
// ================================================================

using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOs.Designation
{
    /// <summary>
    /// Represents filters and paging inputs for designation projections.
    /// </summary>
    public class GetDesignationRequestDTO : BaseRequest
    {
        public int DepartmentId { get; set; }
        public string? DesignationName { get; set; }
        public bool? IsActive { get; set; }
    }
}
