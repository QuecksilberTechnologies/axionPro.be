// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side request for the transactional nested Tenant update endpoint.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Defines the editable Tenant aggregate values accepted by the Host-managed nested update endpoint.
/// Permission metadata is intentionally supplied through the query string, not this body.
/// </summary>
public sealed class NewTenantUpdateRequestDTO
{
    #region Tenant

    /// <summary>Gets or sets the Tenant industry identifier when it is being changed.</summary>
    public int TenantIndustryId { get; set; }

    /// <summary>Gets or sets the Tenant company name when it is being changed.</summary>
    public string? CompanyName { get; set; }

    /// <summary>Gets or sets the Tenant code when it is being changed.</summary>
    public string? TenantCode { get; set; }

    /// <summary>Gets or sets the Tenant company email domain when it is being changed.</summary>
    public string? CompanyEmailDomain { get; set; }

    /// <summary>Gets or sets the Tenant email address when it is being changed.</summary>
    public string? TenantEmail { get; set; }

    /// <summary>Gets or sets the Tenant contact-person name when it is being changed.</summary>
    public string? ContactPersonName { get; set; }

    /// <summary>Gets or sets the Tenant contact-person gender identifier when it is being changed.</summary>
    public int? GenderId { get; set; }

    /// <summary>Gets or sets the Tenant contact number when it is being changed.</summary>
    public string? ContactNumber { get; set; }

    /// <summary>Gets or sets the Tenant country identifier when it is being changed.</summary>
    public int CountryId { get; set; }

    /// <summary>Gets or sets the Tenant default currency identifier when it is being changed.</summary>
    public int? DefaultCurrency { get; set; }

    #endregion

    #region Nested Configuration

    /// <summary>Gets or sets the Tenant profile values to update.</summary>
    public NewTenantProfileUpdateRequestDTO? Profile { get; set; }

    /// <summary>Gets or sets the one Tenant location selected by the UI dropdown.</summary>
    public NewTenantSelectedLocationUpdateRequestDTO? SelectedLocation { get; set; }

    /// <summary>Gets or sets the active employee-code-pattern values to update.</summary>
    public NewTenantEmployeeCodePatternUpdateRequestDTO? EmployeeCodePattern { get; set; }

    /// <summary>Gets or sets the active Tenant email configuration values to update.</summary>
    public NewTenantEmailConfigurationUpdateRequestDTO? EmailConfiguration { get; set; }

    #endregion
}

/// <summary>Defines editable Tenant profile values.</summary>
public sealed class NewTenantProfileUpdateRequestDTO
{
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? ThemeColor { get; set; }
    public string? BusinessType { get; set; }
    public string? Industry { get; set; }
    public int? TotalEmployees { get; set; }
    public int? TotalBranches { get; set; }
    public int? FoundedYear { get; set; }
    public string? WebsiteUrl { get; set; }
}

/// <summary>Defines editable values for exactly one UI-selected Tenant location.</summary>
public sealed class NewTenantSelectedLocationUpdateRequestDTO
{
    /// <summary>Gets or sets the raw identifier selected from the Tenant's own location dropdown.</summary>
    public long TenantLocationId { get; set; }

    public string? LocationCode { get; set; }
    public string? LocationName { get; set; }
    public TenantLocationType? LocationType { get; set; }
    public int? CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string? TimeZoneId { get; set; }
    public bool? IsGeoFenceEnabled { get; set; }
    public bool? IsAttendanceAllowed { get; set; }
    public bool? IsBiometricEnabled { get; set; }
}

/// <summary>Defines editable values for the active Tenant employee-code pattern.</summary>
public sealed class NewTenantEmployeeCodePatternUpdateRequestDTO
{
    public string? Prefix { get; set; }
    public bool? IncludeYear { get; set; }
    public bool? IncludeMonth { get; set; }
    public bool? IncludeDepartment { get; set; }
    public string? Separator { get; set; }
    public string? RunningNumberLength { get; set; }
}

/// <summary>Defines editable non-secret SMTP configuration values.</summary>
public sealed class NewTenantEmailConfigurationUpdateRequestDTO
{
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }

    /// <summary>Gets or sets a replacement encrypted SMTP password; null, empty, and whitespace preserve the current password.</summary>
    public string? SmtpPasswordEncrypted { get; set; }

    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
    public bool? IsActive { get; set; }
}
