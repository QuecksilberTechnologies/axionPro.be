// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for Tenant deletion.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the Host-side request to place a Tenant into the existing soft-delete lifecycle.
/// </summary>
public sealed class DeleteTenantRequestDTO : PermissionRequestDTO
{
    #region Deletion Properties

    /// <summary>
    /// Gets or sets the encrypted Tenant identifier to delete.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional administrative reason for deletion.
    /// </summary>
    public string? Remark { get; set; }

    #endregion
}
