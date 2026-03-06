using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeLeavePolicyMapping
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public bool? IsLeaveBalanceAssigned { get; set; }

    public long EmployeeId { get; set; }

    public long PolicyLeaveTypeMappingId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

    public virtual PolicyLeaveTypeMapping PolicyLeaveTypeMapping { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
