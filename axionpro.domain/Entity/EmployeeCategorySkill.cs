using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeCategorySkill
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
