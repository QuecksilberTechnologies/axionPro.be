using axionpro.domain.Common;
using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AssetAssignment :BaseEntity
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public long AssetId { get; set; }

    public DateTime? AssignedDate { get; set; }

    public DateTime? ExpectedReturnDate { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    public int AssignmentStatusId { get; set; }

    public string AssetConditionAtAssign { get; set; } = null!;

    public string? AssetConditionAtReturn { get; set; }

    public string IdentificationMethod { get; set; } = null!;

    public string IdentificationValue { get; set; } = null!;   

    public virtual Asset Asset { get; set; } = null!;

    public virtual AssignmentStatus AssignmentStatus { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
