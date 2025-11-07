using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TicketType
{
    public long Id { get; set; }

    public string TicketTypeName { get; set; } = null!;

    public long? TicketHeaderId { get; set; }

    public long? TenantId { get; set; }

    public int? ResponsibleRoleId { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedTime { get; set; }

    public virtual ICollection<AssetTicketTypeDetail> AssetTicketTypeDetails { get; set; } = new List<AssetTicketTypeDetail>();

    public virtual Role? ResponsibleRole { get; set; }

    public virtual Tenant? Tenant { get; set; }

    public virtual TicketHeader? TicketHeader { get; set; }
}
