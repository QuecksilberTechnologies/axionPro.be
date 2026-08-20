// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for deactivating a Tenant with an administrative remark.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the Host-side request to deactivate a Tenant.
/// </summary>
public sealed class DeactivateTenantRequestDTO
{
    #region Deactivation Properties

    /// <summary>
    /// Gets or sets the Tenant identifier to deactivate.
    /// </summary>
    public long TenantId { get; set; }

    /// <summary>
    /// Gets or sets the administrative reason for deactivating the Tenant.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    #endregion
}
