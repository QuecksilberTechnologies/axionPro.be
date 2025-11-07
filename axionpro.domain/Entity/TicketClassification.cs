using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TicketClassification
{
    public int Id { get; set; }

    public long? TenantId { get; set; }

    public string ClassificationName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedTime { get; set; }

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<TicketHeader> TicketHeaders { get; set; } = new List<TicketHeader>();
}
