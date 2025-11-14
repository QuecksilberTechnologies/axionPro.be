using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeDependent
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public string? DependentName { get; set; }

    public string? Relation { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? IsCoveredInPolicy { get; set; }

    public bool? IsMarried { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
    public bool HasProofUploaded { get; set; }
    public string? ProofDocName { get; set; }
    public string? ProofDocPath { get; set; }
    public int  DocType { get; set; }
    public bool? IsSoftDeleted { get; set; }
    public bool? IsEditAllowed { get; set; }
    public bool? IsActive { get; set; }

    public long? InfoVerifiedById { get; set; }

    public bool? IsInfoVerified { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public virtual Employee? Employee { get; set; }

}
