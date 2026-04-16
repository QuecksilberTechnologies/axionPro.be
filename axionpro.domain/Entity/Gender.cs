using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Gender
{
    public int Id { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();

    public virtual ICollection<PolicyLeaveTypeMapping> PolicyLeaveTypeMapping { get; set; } = new List<PolicyLeaveTypeMapping>();

    public virtual ICollection<Tenant> Tenant { get; set; } = new List<Tenant>();
}
