// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for activating a Tenant with an administrative remark.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the Host-side request to activate a Tenant.
/// </summary>
public sealed class ActivateTenantRequestDTO : PermissionRequestDTO
{
    #region Activation Properties

    /// <summary>
    /// Gets or sets the encrypted Tenant identifier to activate.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the administrative reason for activating the Tenant.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    #endregion
}
