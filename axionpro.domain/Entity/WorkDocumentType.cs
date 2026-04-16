using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class WorkDocumentType
{
    public int Id { get; set; }

    public string DocumentTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<EmployeeWorkDocument> EmployeeWorkDocument { get; set; } = new List<EmployeeWorkDocument>();
}
