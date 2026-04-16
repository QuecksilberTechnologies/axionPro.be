using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeWorkProfile
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public bool IsFresher { get; set; }

    public string? Comment { get; set; }

    public bool IsActive { get; set; }

    public bool IsEditAllowed { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<EmployeeWorkHistory> EmployeeWorkHistory { get; set; } = new List<EmployeeWorkHistory>();
}
