using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenantProfile
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string? ThemeColor { get; set; }

    public string? BusinessType { get; set; }

    public string? Industry { get; set; }

    public int? TotalEmployees { get; set; }

    public int? TotalBranches { get; set; }

    public int? FoundedYear { get; set; }

    public string? WebsiteUrl { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
