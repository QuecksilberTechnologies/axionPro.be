using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenantEmailConfig
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public string? SmtpHost { get; set; }

    public int? SmtpPort { get; set; }

    public string? SmtpUsername { get; set; }

    public string? SmtpPasswordEncrypted { get; set; }

    public string? FromEmail { get; set; }

    public string? FromName { get; set; }

    public bool IsActive { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
