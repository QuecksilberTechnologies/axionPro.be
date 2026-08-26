// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for retrieving one Tenant by identifier.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the request to retrieve one Tenant for Host-side details or editing.
/// </summary>
public sealed class GetTenantByIdRequestDTO : PermissionRequestDTO
{
    #region Identifier

    /// <summary>
    /// Gets or sets the encrypted Tenant identifier to retrieve.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    #endregion
}
