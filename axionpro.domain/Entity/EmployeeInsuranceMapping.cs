using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeInsuranceMapping
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }

    public int InsurancePolicyId { get; set; }

    public DateOnly CoverageStartDate { get; set; }

    public DateOnly? CoverageEndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual InsurancePolicy InsurancePolicy { get; set; } = null!;
}
