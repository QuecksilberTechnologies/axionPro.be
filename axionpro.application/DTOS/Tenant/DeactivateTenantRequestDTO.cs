// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for deactivating a Tenant with an administrative remark.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the Host-side request to deactivate a Tenant.
/// </summary>
public sealed class DeactivateTenantRequestDTO : PermissionRequestDTO
{
    #region Deactivation Properties

    /// <summary>
    /// Gets or sets the encrypted Tenant identifier to deactivate.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the administrative reason for deactivating the Tenant.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    #endregion
}
