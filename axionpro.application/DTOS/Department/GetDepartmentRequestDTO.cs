// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines client-supplied department listing filters.
// ================================================================

using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOs.Department
{
    /// <summary>
    /// Represents filters and paging inputs for a department listing request.
    /// </summary>
    public class GetDepartmentRequestDTO : BaseRequest
    {
        public string? DepartmentName { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int? Id { get; set; }
    }
}
