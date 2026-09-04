// ================================================================
// Purpose : Defines safe request and response contracts for Tenant SMTP configuration.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Configruations;

/// <summary>
/// Carries the encrypted Host-selected Tenant identifier and permission
/// metadata. Tenant Employee callers are always restricted to the Tenant in
/// their authenticated token, so their submitted TenantId is ignored.
/// </summary>
public class TenantEmailConfigAccessRequestDTO : PermissionRequestDTO
{
    public string? TenantId { get; set; }
}

/// <summary>Supplies the SMTP settings to create for one Tenant.</summary>
public class CreateTenantEmailConfigRequestDTO : TenantEmailConfigAccessRequestDTO
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>Write-only SMTP secret. It is never returned by this API.</summary>
    public string SmtpPassword { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies updated SMTP settings for an existing Tenant configuration.</summary>
public sealed class UpdateTenantEmailConfigRequestDTO : TenantEmailConfigAccessRequestDTO
{
    public int Id { get; set; }
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>When omitted, the existing SMTP secret is retained.</summary>
    public string? SmtpPassword { get; set; }

    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>Safe Tenant SMTP configuration representation; it excludes all SMTP secret material.</summary>
public sealed class TenantEmailConfigResponseDTO
{
    public int Id { get; init; }
    public string TenantId { get; set; } = string.Empty;
    public string? TenantName { get; init; }
    public string? SmtpHost { get; init; }
    public int? SmtpPort { get; init; }
    public string? SmtpUsername { get; init; }
    public string? FromEmail { get; init; }
    public string? FromName { get; init; }
    public bool IsActive { get; init; }
    public bool HasSmtpPassword { get; init; }
}
