using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Gender
{
    public int Id { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

    public virtual ICollection<PolicyLeaveTypeMapping> PolicyLeaveTypeMappings { get; set; } = new List<PolicyLeaveTypeMapping>();

}
