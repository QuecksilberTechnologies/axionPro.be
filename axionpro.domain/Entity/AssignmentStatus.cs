using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AssignmentStatus
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();

    public virtual ICollection<AssetHistory> AssetHistories { get; set; } = new List<AssetHistory>();
}
