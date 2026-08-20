// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for activating a Tenant with an administrative remark.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the Host-side request to activate a Tenant.
/// </summary>
public sealed class ActivateTenantRequestDTO
{
    #region Activation Properties

    /// <summary>
    /// Gets or sets the Tenant identifier to activate.
    /// </summary>
    public long TenantId { get; set; }

    /// <summary>
    /// Gets or sets the administrative reason for activating the Tenant.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    #endregion
}
