// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines editable Tenant fields for Host-side Tenant management.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the client-editable Tenant fields for a Host-side update request.
/// </summary>
public sealed class UpdateTenantRequestDTO : UpdateHostManagedTenantRequestDTO
{
    #region Tenant Identifier

    /// <summary>
    /// Gets or sets the encrypted Tenant identifier to update.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    #endregion
}
