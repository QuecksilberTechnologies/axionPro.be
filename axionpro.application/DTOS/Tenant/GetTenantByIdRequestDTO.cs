// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request contract for retrieving one Tenant by identifier.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the request to retrieve one Tenant for Host-side details or editing.
/// </summary>
public sealed class GetTenantByIdRequestDTO
{
    #region Identifier

    /// <summary>
    /// Gets or sets the Tenant identifier to retrieve.
    /// </summary>
    public long TenantId { get; set; }

    #endregion
}
