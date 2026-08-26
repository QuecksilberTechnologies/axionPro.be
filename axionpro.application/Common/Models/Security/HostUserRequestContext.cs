// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents the trusted Host JWT context required for runtime permission validation and Host-facing Tenant identifier protection.
// ================================================================

namespace axionpro.application.Common.Models.Security;

/// <summary>
/// Represents the authenticated Host identity, login-time role snapshot, and signed session key.
/// </summary>
public sealed class HostUserRequestContext
{
    /// <summary>
    /// Gets the validated Host user primary identifier.
    /// </summary>
    public long HostUserId { get; init; }

    /// <summary>
    /// Gets the Host role identifier captured when the JWT was issued.
    /// </summary>
    public long TokenHostRoleId { get; init; }

    /// <summary>
    /// Gets the Host-scoped key carried in the established Tenant encryption-key claim.
    /// </summary>
    public string TenantEncryptionKey { get; init; } = string.Empty;
}
