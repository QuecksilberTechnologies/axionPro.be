using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeStatusHistory
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public int OldEmployeeTypeId { get; set; }

    public int NewEmployeeTypeId { get; set; }

    public DateTime? ChangeDateTime { get; set; }

    public long ChangedById { get; set; }

    public string? Remark { get; set; }

    public bool IsActive { get; set; }

    public long AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual EmployeeType NewEmployeeType { get; set; } = null!;

    public virtual EmployeeType OldEmployeeType { get; set; } = null!;
}
