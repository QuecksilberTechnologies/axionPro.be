using System;

namespace axionpro.domain.Entity;

/// <summary>
/// Stores the single, centrally managed SMTP configuration used to initialize a Tenant's email configuration at registration.
/// </summary>
public partial class DefaultEmailConfig
{
    public int Id { get; set; }

    public string ConfigName { get; set; } = null!;

    public string SmtpHost { get; set; } = null!;

    public int SmtpPort { get; set; }

    public string SmtpUsername { get; set; } = null!;

    public string SmtpPasswordEncrypted { get; set; } = null!;

    public string FromEmail { get; set; } = null!;

    public string FromName { get; set; } = null!;

    public bool IsActive { get; set; }

    /// <summary>
    /// Identifies the one active SMTP configuration copied into every newly registered Tenant.
    /// </summary>
    public bool IsDefault { get; set; }

    public string SecrateKey { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }
}
