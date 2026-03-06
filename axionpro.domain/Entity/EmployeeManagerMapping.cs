using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeManagerMapping
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }

    public long ManagerId { get; set; }

    public int? DepartmentId { get; set; }

    public int? DesignationId { get; set; }

    public int ReportingTypeId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Designation? Designation { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee Manager { get; set; } = null!;

    public virtual ReportingType ReportingType { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
