// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the compact Host and Tenant Employee session bootstrap response returned by the NewLogin endpoint.
// ================================================================

using System.Text.Json.Serialization;

namespace axionpro.application.DTOs.UserLogin;

/// <summary>
/// Represents the compact session bootstrap returned after a Host user or Tenant Employee authenticates successfully.
/// </summary>
public sealed class NewLoginResponseDTO
{
    #region Principal Type

    /// <summary>
    /// Gets or sets the existing authenticated principal type that owns the session.
    /// </summary>
    public string UserType { get; set; } = string.Empty;

    #endregion

    #region Token Information

    /// <summary>
    /// Gets or sets the signed access token issued for the authenticated Tenant Employee.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the opaque refresh token issued for the authenticated Tenant Employee.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC expiration timestamp embedded in the issued access token.
    /// </summary>
    public DateTime AccessTokenExpiresAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the UTC expiration timestamp persisted for the issued refresh token.
    /// </summary>
    public DateTime RefreshTokenExpiresAtUtc { get; set; }

    #endregion

    #region User Context

    /// <summary>
    /// Gets or sets the minimal Tenant Employee context needed to bootstrap an HRMS session.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NewLoginUserContextDTO? User { get; set; }

    /// <summary>
    /// Gets or sets the minimal Host-user context when the authenticated principal is a Host user.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NewLoginHostUserContextDTO? HostUser { get; set; }

    /// <summary>
    /// Gets or sets the active Host role when the authenticated principal is a Host user.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NewLoginHostRoleDTO? HostRole { get; set; }

    #endregion
}

/// <summary>
/// Represents the minimal Tenant Employee context needed immediately after authentication.
/// </summary>
public sealed class NewLoginUserContextDTO
{
    #region Identity

    /// <summary>
    /// Gets or sets the client-safe encoded employee identifier.
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's display name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's official email address.
    /// </summary>
    public string? OfficialEmail { get; set; }

    /// <summary>
    /// Gets or sets the resolved profile-image URL when a profile image exists.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    #endregion

    #region Tenant Context

    /// <summary>
    /// Gets or sets the client-safe encoded tenant identifier.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authenticated tenant's company name.
    /// </summary>
    public string TenantName { get; set; } = string.Empty;

    #endregion

    #region Employment Context

    /// <summary>
    /// Gets or sets the single effective primary role assigned to the employee.
    /// </summary>
    public NewLoginRoleDTO PrimaryRole { get; set; } = null!;

    /// <summary>
    /// Gets or sets the effective non-primary roles assigned to the employee.
    /// The collection is always present and does not include navigation or permission data.
    /// </summary>
    public IReadOnlyCollection<NewLoginRoleDTO> SecondaryRoles { get; set; }
        = Array.Empty<NewLoginRoleDTO>();

    /// <summary>
    /// Gets or sets the employee-type identifier.
    /// </summary>
    public int EmployeeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the employee-type name.
    /// </summary>
    public string? EmployeeTypeName { get; set; }

    /// <summary>
    /// Gets or sets the department identifier when the employee has an active department assignment.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the department name when the employee has an active department assignment.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the designation identifier when the employee has an active designation assignment.
    /// </summary>
    public int? DesignationId { get; set; }

    /// <summary>
    /// Gets or sets the designation name when the employee has an active designation assignment.
    /// </summary>
    public string? DesignationName { get; set; }

    #endregion

    #region Session Flags

    /// <summary>
    /// Gets or sets whether the employee must change the current password.
    /// </summary>
    public bool IsPasswordChangeRequired { get; set; }

    /// <summary>
    /// Gets or sets whether the employee has completed onboarding.
    /// </summary>
    public bool IsOnboard { get; set; }

    #endregion
}

/// <summary>
/// Represents one effective Tenant-scoped role returned in the NewLogin bootstrap response.
/// </summary>
public sealed class NewLoginRoleDTO
{
    #region Role Identity

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role-type identifier.
    /// </summary>
    public int RoleTypeId { get; set; }

    /// <summary>
    /// Gets or sets the role-type name used by the existing Tenant login contract.
    /// </summary>
    public string RoleTypeName { get; set; } = string.Empty;

    #endregion
}

/// <summary>
/// Represents the minimal Host-user context returned after Host authentication.
/// </summary>
public sealed class NewLoginHostUserContextDTO
{
    #region Host User Identity

    /// <summary>
    /// Gets or sets the Host-user identifier.
    /// </summary>
    public long HostUserId { get; set; }

    /// <summary>
    /// Gets or sets the Host user's display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Host user's login identifier.
    /// </summary>
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Host user's email address when it is available.
    /// </summary>
    public string? Email { get; set; }

    #endregion
}

/// <summary>
/// Represents the active Host role returned after Host authentication.
/// </summary>
public sealed class NewLoginHostRoleDTO
{
    #region Host Role Identity

    /// <summary>
    /// Gets or sets the Host-role identifier.
    /// </summary>
    public long HostRoleId { get; set; }

    /// <summary>
    /// Gets or sets the Host-role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    #endregion
}
