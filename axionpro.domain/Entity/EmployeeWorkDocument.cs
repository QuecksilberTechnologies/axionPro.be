using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeWorkDocument
{
    public long Id { get; set; }

    public long EmployeeWorkHistoryId { get; set; }

    public int WorkDocumentTypeId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public bool IsVerified { get; set; }

    public long? VerifiedById { get; set; }

    public DateTime? VerifiedDateTime { get; set; }

    public bool IsActive { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public virtual EmployeeWorkHistory EmployeeWorkHistory { get; set; } = null!;

    public virtual WorkDocumentType WorkDocumentType { get; set; } = null!;
}
