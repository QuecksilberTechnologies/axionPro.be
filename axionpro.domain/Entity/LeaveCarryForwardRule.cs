using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LeaveCarryForwardRule
{
    public int Id { get; set; }

    public int LeaveTypeId { get; set; }

    public int? MaxCarryForwardLeave { get; set; }

    public bool? IsActive { get; set; }

    public int? ExpiryAfterYears { get; set; }

    public bool? IsSoftDelete { get; set; }

    public DateTime? SoftDeleteDateTime { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
