// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side Tenant onboarding request and initial configuration.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Extends the established Tenant registration contract with initial profile and location configuration.
/// </summary>
public sealed class NewTenantCreationRequestDTO : PermissionRequestDTO, INewTenantOnboardingConfiguration
{
    public int SubscriptionPlanId { get; set; }
    public int TenantIndustryId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyEmailDomain { get; set; } = string.Empty;
    public int GenderId { get; set; }
    public string TenantEmail { get; set; } = string.Empty;
    public string? ContactPersonName { get; set; }
    public string? ContactNumber { get; set; }
    public int CountryId { get; set; }

    /// <summary>Gets or sets the initial Tenant profile values.</summary>
    public NewTenantProfileRequestDTO Profile { get; set; } = new();

    /// <summary>Gets or sets the first Tenant location created during onboarding.</summary>
    public NewTenantLocationRequestDTO InitialLocation { get; set; } = new();

    /// <summary>Gets or sets the first employee-code generation pattern.</summary>
    public NewTenantEmployeeCodePatternRequestDTO EmployeeCodePattern { get; set; } = new();

    /// <summary>Gets or sets the initial Tenant email configuration.</summary>
    public NewTenantEmailConfigurationRequestDTO EmailConfiguration { get; set; } = new();
}

/// <summary>Supplies extended onboarding values to the established transactional creation handler.</summary>
public interface INewTenantOnboardingConfiguration
{
    NewTenantProfileRequestDTO Profile { get; }
    NewTenantLocationRequestDTO InitialLocation { get; }
    NewTenantEmailConfigurationRequestDTO EmailConfiguration { get; }
}

/// <summary>Defines the initial Tenant employee-code generation pattern.</summary>
public sealed class NewTenantEmployeeCodePatternRequestDTO
{
    public string Prefix { get; set; } = "EMP";
    public bool IncludeYear { get; set; } = true;
    public bool IncludeMonth { get; set; } = true;
    public bool IncludeDepartment { get; set; }
    public string Separator { get; set; } = "/";
    public string RunningNumberLength { get; set; } = "4";
}

/// <summary>Defines the Tenant-owned SMTP settings used after onboarding.</summary>
public sealed class NewTenantEmailConfigurationRequestDTO
{
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPasswordEncrypted { get; set; }
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SecrateKey { get; set; }
}

/// <summary>Defines editable values for the initial Tenant profile.</summary>
public sealed class NewTenantProfileRequestDTO
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

/// <summary>Defines the first physical Tenant location.</summary>
public sealed class NewTenantLocationRequestDTO
{
    public string LocationCode { get; set; } = "HQ";
    public string? LocationName { get; set; }
    public short LocationType { get; set; } = 1;
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string TimeZoneId { get; set; } = "Asia/Kolkata";
    public bool IsGeoFenceEnabled { get; set; }
    public bool IsAttendanceAllowed { get; set; } = true;
    public bool IsBiometricEnabled { get; set; }
}
