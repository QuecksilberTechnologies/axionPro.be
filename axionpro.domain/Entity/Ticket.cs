using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Ticket
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public string? TicketNumber { get; set; }

    public int? TicketClassificationId { get; set; }

    public long? TicketHeaderId { get; set; }

    public long? TicketTypeId { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public int? Status { get; set; }

    public long? AssignedToUserId { get; set; }

    public long? RequestedForUserId { get; set; }

    public long? RequestedByUserId { get; set; }

    public long? RecommendedByUserId { get; set; }

    public DateTime? DueDateTime { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public long? UpdatedByUserId { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public virtual Employee? AssignedToUser { get; set; }

    public virtual Employee? RecommendedByUser { get; set; }

    public virtual Employee? RequestedByUser { get; set; }

    public virtual Employee? RequestedForUser { get; set; }

    public virtual ICollection<TicketAttachment> TicketAttachment { get; set; } = new List<TicketAttachment>();

    public virtual TicketClassification? TicketClassification { get; set; }

    public virtual TicketHeader? TicketHeader { get; set; }

    public virtual TicketType? TicketType { get; set; }
}
