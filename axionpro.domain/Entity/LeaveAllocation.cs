using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LeaveAllocation
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public int Year { get; set; }

    public decimal? AllocatedLeave { get; set; }

    public decimal? RemainingLeave { get; set; }

    public bool? CanApply { get; set; }

    public bool? IsActive { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual LeaveType LeaveType { get; set; } = null!;
}
