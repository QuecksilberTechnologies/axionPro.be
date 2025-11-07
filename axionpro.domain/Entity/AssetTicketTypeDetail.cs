using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AssetTicketTypeDetail
{
    public long Id { get; set; }

    public long TicketTypeId { get; set; }

    public int AssetTypeId { get; set; }

    public int ResponsibleRoleId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedTime { get; set; }

    public virtual AssetType AssetType { get; set; } = null!;

    public virtual Role ResponsibleRole { get; set; } = null!;

    public virtual TicketType TicketType { get; set; } = null!;
}
