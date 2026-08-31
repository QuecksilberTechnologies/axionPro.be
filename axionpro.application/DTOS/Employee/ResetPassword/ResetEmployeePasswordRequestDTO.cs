// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the authorized Tenant administrator request for
//           resetting a selected Employee login password.
// ================================================================

using axionpro.application.DTOs.BaseDTO;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.ResetPassword;

/// <summary>
/// Represents an administrator-initiated password reset for one Employee in
/// the authenticated Tenant. The employee identifier is client-safe encoded.
/// </summary>
public sealed class ResetEmployeePasswordRequestDTO : PermissionRequestDTO
{
    /// <summary>
    /// Gets or sets the client-safe encoded Employee identifier whose password
    /// is being reset.
    /// </summary>
    [Required]
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the replacement password.
    /// </summary>
    [Required]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation of the replacement password.
    /// </summary>
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}
