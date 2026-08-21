// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the minimal persistence read model used to construct a NewLogin Tenant Employee session.
// ================================================================

namespace axionpro.application.DTOs.UserLogin;

/// <summary>
/// Represents the validated, minimal Tenant Employee data projected by the login repository for a NewLogin session.
/// </summary>
public sealed class NewLoginBootstrapReadModel
{
    #region Trusted Identity Data

    /// <summary>
    /// Gets or sets the internal employee identifier used only for trusted server-side processing.
    /// </summary>
    public long EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the internal tenant identifier used only for trusted server-side processing.
    /// </summary>
    public long TenantId { get; set; }

    /// <summary>
    /// Gets or sets the employee's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the employee's middle name.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the employee's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the employee's official email address.
    /// </summary>
    public string? OfficialEmail { get; set; }

    /// <summary>
    /// Gets or sets the active tenant's company name.
    /// </summary>
    public string TenantName { get; set; } = string.Empty;

    #endregion

    #region Employment Data

    /// <summary>
    /// Gets or sets the employee-type identifier.
    /// </summary>
    public int EmployeeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the employee-type name.
    /// </summary>
    public string? EmployeeTypeName { get; set; }

    /// <summary>
    /// Gets or sets the active department identifier when assigned.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the active department name when assigned.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the active designation identifier when assigned.
    /// </summary>
    public int? DesignationId { get; set; }

    /// <summary>
    /// Gets or sets the active designation name when assigned.
    /// </summary>
    public string? DesignationName { get; set; }

    #endregion

    #region Token and Session Data

    /// <summary>
    /// Gets or sets the employee's gender identifier used by the existing Tenant access-token claim set.
    /// </summary>
    public int GenderId { get; set; }

    /// <summary>
    /// Gets or sets the employee's gender name used by the existing Tenant access-token claim set.
    /// </summary>
    public string? GenderName { get; set; }

    /// <summary>
    /// Gets or sets whether the employee has permanent employment status for the existing Tenant access-token claim set.
    /// </summary>
    public bool HasPermanent { get; set; }

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
