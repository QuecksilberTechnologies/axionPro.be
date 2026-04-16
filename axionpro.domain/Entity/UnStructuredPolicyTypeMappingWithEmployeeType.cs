using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class UnStructuredPolicyTypeMappingWithEmployeeType
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public int EmployeeTypeId { get; set; }

    public int PolicyTypeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime StartDate { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public virtual EmployeeType EmployeeType { get; set; } = null!;

    public virtual PolicyType PolicyType { get; set; } = null!;
}
