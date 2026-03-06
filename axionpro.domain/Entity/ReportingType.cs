using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ReportingType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappings { get; set; } = new List<EmployeeManagerMapping>();
}
