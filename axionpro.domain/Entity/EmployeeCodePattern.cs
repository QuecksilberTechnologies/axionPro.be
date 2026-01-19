using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeCodePattern
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public string? Prefix { get; set; }

    public bool IncludeYear { get; set; }

    public bool IncludeMonth { get; set; }

    public bool IncludeDepartment { get; set; }

    public string Separator { get; set; } = null!;

    public int RunningNumberLength { get; set; }

    public int LastUsedNumber { get; set; }

    public bool IsActive { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
