// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-safe employee-code pattern response with an encrypted Tenant identifier.
// ================================================================

namespace axionpro.application.DTOS.Tenant;

/// <summary>
/// Represents an employee-code pattern returned to a Host user without exposing a raw Tenant identifier.
/// </summary>
public sealed class HostEmployeeCodePatternResponseDTO
{
    /// <summary>Gets or sets the employee-code pattern identifier.</summary>
    public long Id { get; init; }
    /// <summary>Gets or sets the encrypted Tenant identifier.</summary>
    public string TenantId { get; init; } = string.Empty;
    /// <summary>Gets or sets the optional employee-code prefix.</summary>
    public string? Prefix { get; init; }
    /// <summary>Gets or sets whether the year is included.</summary>
    public bool IncludeYear { get; init; }
    /// <summary>Gets or sets whether the month is included.</summary>
    public bool IncludeMonth { get; init; }
    /// <summary>Gets or sets whether the department is included.</summary>
    public bool IncludeDepartment { get; init; }
    /// <summary>Gets or sets the code-part separator.</summary>
    public string Separator { get; init; } = "/";
    /// <summary>Gets or sets the running-number length.</summary>
    public int RunningNumberLength { get; init; }
    /// <summary>Gets or sets the last used running number.</summary>
    public int LastUsedNumber { get; init; }
    /// <summary>Gets or sets whether the pattern is active.</summary>
    public bool IsActive { get; init; }
    /// <summary>Gets or sets the creator identifier.</summary>
    public long AddedById { get; init; }
    /// <summary>Gets or sets when the pattern was created.</summary>
    public DateTime AddedDateTime { get; init; }
    /// <summary>Gets or sets the optional updater identifier.</summary>
    public long? UpdatedById { get; init; }
    /// <summary>Gets or sets when the pattern was last updated.</summary>
    public DateTime? UpdatedDateTime { get; init; }
}
