using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeInsurancePolicy
{
    public int Id { get; set; }

    public long? EmployeeId { get; set; }

    public int? InsurancePolicyId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public string? Description { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public long? ApprovedById { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
}
