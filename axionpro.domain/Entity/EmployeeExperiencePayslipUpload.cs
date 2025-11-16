using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperiencePayslipUpload
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long ExperienceDetailId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public bool HasUploadedPayslip { get; set; }

    public string? PayslipDocName { get; set; }

    public string? PayslipDocPath { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public long AddedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual EmployeeExperienceDetail ExperienceDetail { get; set; } = null!;
}
