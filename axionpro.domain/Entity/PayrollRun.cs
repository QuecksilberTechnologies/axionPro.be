using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PayrollRun
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public int PayrollMonth { get; set; }

    public int PayrollYear { get; set; }

    public int Status { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public bool? IsLocked { get; set; }

    public long? LockedBy { get; set; }

    public DateTime? LockedOn { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual ICollection<PayrollEmployee> PayrollEmployee { get; set; } = new List<PayrollEmployee>();

    public virtual Tenant Tenant { get; set; } = null!;
}
