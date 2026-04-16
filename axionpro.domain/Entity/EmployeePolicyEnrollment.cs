using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeePolicyEnrollment
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }

    public int PolicyTypeId { get; set; }

    public int InsurancePolicyId { get; set; }

    public bool HasDependent { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<EmployeePolicyDependentMapping> EmployeePolicyDependentMapping { get; set; } = new List<EmployeePolicyDependentMapping>();

    public virtual InsurancePolicy InsurancePolicy { get; set; } = null!;

    public virtual PolicyType PolicyType { get; set; } = null!;
}
