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
public sealed class UpdateTenantRequestDTO
{
    #region Editable Tenant Properties

    /// <summary>
    /// Gets or sets the Tenant identifier to update.
    /// </summary>
    public long TenantId { get; set; }

    /// <summary>
    /// Gets or sets the Tenant industry identifier.
    /// </summary>
    public int TenantIndustryId { get; set; }

    /// <summary>
    /// Gets or sets the Tenant company name.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the Tenant code.
    /// </summary>
    public string? TenantCode { get; set; }

    /// <summary>
    /// Gets or sets the company email domain.
    /// </summary>
    public string? CompanyEmailDomain { get; set; }

    /// <summary>
    /// Gets or sets the Tenant email address.
    /// </summary>
    public string? TenantEmail { get; set; }

    /// <summary>
    /// Gets or sets the Tenant contact person name.
    /// </summary>
    public string? ContactPersonName { get; set; }

    /// <summary>
    /// Gets or sets the configured gender identifier.
    /// </summary>
    public int? GenderId { get; set; }

    /// <summary>
    /// Gets or sets the Tenant contact number.
    /// </summary>
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Gets or sets the Tenant country identifier.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets the Tenant default currency identifier.
    /// </summary>
    public int? DefaultCurrency { get; set; }

    #endregion
}
