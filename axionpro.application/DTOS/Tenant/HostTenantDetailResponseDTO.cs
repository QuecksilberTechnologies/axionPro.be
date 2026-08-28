// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the safe Host-side Tenant detail projection, including active Tenant-owned configuration collections.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents the safe, complete Host-visible Tenant configuration detail. The Tenant identifier is encrypted and no child DTO exposes a raw TenantId.
/// </summary>
public sealed class HostTenantDetailResponseDTO
{
    #region Tenant

    /// <summary>Gets or sets the encrypted Tenant identifier.</summary>
    public string Id { get; set; } = string.Empty;
    public int TenantIndustryId { get; set; }
    public string? TenantIndustryName { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TenantCode { get; set; }
    public string CompanyEmailDomain { get; set; } = string.Empty;
    public string TenantEmail { get; set; } = string.Empty;
    public string? ContactPersonName { get; set; }
    public int? GenderId { get; set; }
    public string? ContactNumber { get; set; }
    public int CountryId { get; set; }
    public int? DefaultCurrency { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }

    #endregion

    #region Tenant-Created Records

    public List<HostTenantProfileDetailDTO> Profiles { get; set; } = new();
    public List<HostTenantLocationDetailDTO> Locations { get; set; } = new();
    public List<HostTenantSubscriptionDetailDTO> Subscriptions { get; set; } = new();
    public List<HostTenantEnabledModuleDetailDTO> EnabledModules { get; set; } = new();
    public List<HostTenantEnabledOperationDetailDTO> EnabledOperations { get; set; } = new();
    public List<HostTenantDepartmentDetailDTO> Departments { get; set; } = new();
    public List<HostTenantDesignationDetailDTO> Designations { get; set; } = new();
    public List<HostTenantEmployeeCodePatternDetailDTO> EmployeeCodePatterns { get; set; } = new();
    public List<HostTenantRoleDetailDTO> Roles { get; set; } = new();
    public List<HostTenantRolePermissionDetailDTO> RolePermissions { get; set; } = new();
    public List<HostTenantEmployeeDetailDTO> Employees { get; set; } = new();
    public List<HostTenantUserRoleDetailDTO> UserRoles { get; set; } = new();
    public List<HostTenantLoginCredentialDetailDTO> LoginCredentials { get; set; } = new();
    public List<HostTenantPolicyTypeDetailDTO> PolicyTypes { get; set; } = new();
    public List<HostTenantEmailConfigurationDetailDTO> EmailConfigurations { get; set; } = new();

    #endregion
}

/// <summary>Represents a Tenant profile record.</summary>
public sealed class HostTenantProfileDetailDTO
{
    public long Id { get; set; }
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

/// <summary>Represents an active, non-soft-deleted Tenant location.</summary>
public sealed class HostTenantLocationDetailDTO
{
    public long Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public short LocationType { get; set; }
    public int CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public bool IsHeadOffice { get; set; }
    public bool IsGeoFenceEnabled { get; set; }
    public bool IsAttendanceAllowed { get; set; }
    public bool IsBiometricEnabled { get; set; }
}

/// <summary>Represents an active Tenant subscription and the selected plan summary.</summary>
public sealed class HostTenantSubscriptionDetailDTO
{
    public long Id { get; set; }
    public int SubscriptionPlanId { get; set; }
    public string? SubscriptionPlanName { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
    public bool IsTrial { get; set; }
    public string? PaymentTxnId { get; set; }
    public string? PaymentMode { get; set; }
}

/// <summary>Represents an enabled Tenant module.</summary>
public sealed class HostTenantEnabledModuleDetailDTO
{
    public long Id { get; set; }
    public int ModuleId { get; set; }
    public int? ParentModuleId { get; set; }
    public bool? IsLeafNode { get; set; }
    public string? ModuleCode { get; set; }
    public string? ModuleName { get; set; }
    public string? DisplayName { get; set; }
}

/// <summary>Represents an enabled Tenant operation.</summary>
public sealed class HostTenantEnabledOperationDetailDTO
{
    public long Id { get; set; }
    public int ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public int OperationId { get; set; }
    public string? OperationName { get; set; }
    public bool? IsOperationUsed { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant department.</summary>
public sealed class HostTenantDepartmentDetailDTO
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public bool IsExecutiveOffice { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant designation.</summary>
public sealed class HostTenantDesignationDetailDTO
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>Represents an active Tenant employee-code pattern.</summary>
public sealed class HostTenantEmployeeCodePatternDetailDTO
{
    public long Id { get; set; }
    public string? Prefix { get; set; }
    public bool IncludeYear { get; set; }
    public bool IncludeMonth { get; set; }
    public bool IncludeDepartment { get; set; }
    public string Separator { get; set; } = string.Empty;
    public int RunningNumberLength { get; set; }
    public int LastUsedNumber { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant role.</summary>
public sealed class HostTenantRoleDetailDTO
{
    public int Id { get; set; }
    public string? RoleName { get; set; }
    public int RoleType { get; set; }
    public string? Remark { get; set; }
    public bool? IsSystemDefault { get; set; }
}

/// <summary>Represents an active, non-soft-deleted permission assigned to a Tenant role.</summary>
public sealed class HostTenantRolePermissionDetailDTO
{
    public int Id { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public int? ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public int? OperationId { get; set; }
    public string? OperationName { get; set; }
    public bool? HasAccess { get; set; }
    public bool? IsOperational { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant employee without credentials or security data.</summary>
public sealed class HostTenantEmployeeDetailDTO
{
    public long Id { get; set; }
    public string? EmployementCode { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public int? GenderId { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? DesignationId { get; set; }
    public string? DesignationName { get; set; }
    public int? EmployeeTypeId { get; set; }
    public string? OfficialEmail { get; set; }
    public DateTime? DateOfOnBoarding { get; set; }
    public int CountryId { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant employee-role assignment.</summary>
public sealed class HostTenantUserRoleDetailDTO
{
    public long Id { get; set; }
    public long? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool? IsPrimaryRole { get; set; }
    public string? Remark { get; set; }
    public DateTime? AssignedDateTime { get; set; }
    public DateTime? RoleStartDate { get; set; }
    public bool? ApprovalRequired { get; set; }
    public string? ApprovalStatus { get; set; }
}

/// <summary>Represents a safe active login record without its password, tokens, device data, or network data.</summary>
public sealed class HostTenantLoginCredentialDetailDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string LoginId { get; set; } = string.Empty;
    public bool HasFirstLogin { get; set; }
    public bool? IsPasswordChangeRequired { get; set; }
    public bool IsOnboard { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Represents an active, non-soft-deleted Tenant policy type.</summary>
public sealed class HostTenantPolicyTypeDetailDTO
{
    public int Id { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsStructured { get; set; }
    public int PolicyTypeEnumVal { get; set; }
    public bool HasPolicyDocUploaded { get; set; }
}

/// <summary>Represents an active Tenant SMTP configuration without password or secret-key material.</summary>
public sealed class HostTenantEmailConfigurationDetailDTO
{
    public int Id { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
}
