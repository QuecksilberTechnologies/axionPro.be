// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the permission-aware request for tenant EmployeeType deletion.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Employee.Type;

/// <summary>
/// Represents a request to soft delete one tenant-owned EmployeeType.
/// </summary>
public sealed class DeleteEmployeeTypeRequestDTO : PermissionRequestDTO
{
    #region Properties

    /// <summary>
    /// Gets or sets the EmployeeType identifier to delete.
    /// </summary>
    public required int Id { get; set; }

    #endregion
}
