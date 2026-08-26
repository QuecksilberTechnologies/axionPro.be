// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-facing Tenant response projection with a protected Tenant identifier.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents Tenant management data returned to an authorized Host user.
/// </summary>
public sealed class HostTenantResponseDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier for Host API use.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Tenant company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Tenant code.
    /// </summary>
    public string? TenantCode { get; set; }

    /// <summary>
    /// Gets or sets the Tenant company email domain.
    /// </summary>
    public string CompanyEmailDomain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Tenant email address.
    /// </summary>
    public string TenantEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Tenant contact person name.
    /// </summary>
    public string? ContactPersonName { get; set; }

    /// <summary>
    /// Gets or sets the Tenant contact number.
    /// </summary>
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Gets or sets the Tenant country identifier.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets whether the Tenant email is verified.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets whether the Tenant is active.
    /// </summary>
    public bool IsActive { get; set; }
}
