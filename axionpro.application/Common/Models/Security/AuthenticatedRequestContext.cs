// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents the trusted Host or Tenant principal validated for a shared authenticated request.
// ================================================================

using axionpro.application.Common.Enums;

namespace axionpro.application.Common.Models.Security;

/// <summary>
/// Represents the Host or Tenant principal that has passed the established request-validation path.
/// </summary>
public sealed class AuthenticatedRequestContext
{
    #region Principal Context

    /// <summary>
    /// Gets or sets the existing principal type that owns the authenticated session.
    /// </summary>
    public LoginUserType UserType { get; init; }

    /// <summary>
    /// Gets or sets the validated Host-user or Tenant Employee identifier.
    /// </summary>
    public long AuthenticatedUserId { get; init; }

    /// <summary>
    /// Gets or sets the validated tenant identifier for a Tenant Employee request.
    /// </summary>
    public long? TenantId { get; init; }

    /// <summary>
    /// Gets or sets the validated primary role identifier for a Tenant Employee request.
    /// </summary>
    public int? RoleId { get; init; }

    #endregion
}
