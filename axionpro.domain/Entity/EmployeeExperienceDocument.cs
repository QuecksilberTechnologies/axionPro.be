using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperienceDocument
{
    // 🔹 Primary Key
    public long Id { get; set; }

    // 🔹 Correct FK
    public long EmployeeExperienceId { get; set; }

    // 🔹 Navigation
    public virtual EmployeeExperience EmployeeExperience { get; set; } = null!;

    // 🔹 Document Info
    public int DocumentType { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }

    public bool IsUploaded { get; set; } = true;

    public string? Remark { get; set; }

    // 🔹 Audit
    public bool IsActive { get; set; } = true;
    public bool IsSoftDeleted { get; set; } = false;

    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}