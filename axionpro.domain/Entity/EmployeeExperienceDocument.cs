using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperienceDocument
{
    public long Id { get; set; }

    public long EmployeeExperienceId { get; set; }

    public int DocumentType { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public bool HasExperienceDocUploaded { get; set; } = false;

    public string? Remark { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long AddedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public virtual EmployeeExperience EmployeeExperience { get; set; } = null!;
}
