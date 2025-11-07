using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TicketHeader
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public string HeaderName { get; set; } = null!;

    public int TicketClassificationId { get; set; }

    public bool IsAssetRelated { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedTime { get; set; }

    public virtual Tenant? Tenant { get; set; }

    public virtual TicketClassification TicketClassification { get; set; } = null!;

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
}
