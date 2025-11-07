using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeEducation
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public string? Degree { get; set; }

    public string? InstituteName { get; set; }

    public string? Remark { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? GradeOrPercentage { get; set; }

    public string? EducationDocPath { get; set; }
    public int? DocType { get; set; }
    public string? DocName { get; set; }
    public string? GpaorPercentage { get; set; }

    public bool? EducationGap { get; set; }

    public string? ReasonOfEducationGap { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? InfoVerifiedById { get; set; }

    public bool? IsInfoVerified { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public bool? IsEditAllowed { get; set; }

    public bool? IsActive { get; set; }

}
