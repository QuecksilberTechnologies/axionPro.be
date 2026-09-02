// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the authorized request model for department options.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Department;

/// <summary>
/// Carries the Department module-operation context for option lookups while
/// retaining the legacy optional request values.
/// </summary>
public sealed class GetDepartmentOptionRequestDTO : PermissionRequestDTO
{
    public string? UserEmployeeId { get; set; }

    public DateTime? TodaysDate { get; set; }
}
