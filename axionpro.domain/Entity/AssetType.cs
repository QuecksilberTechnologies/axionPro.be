using axionpro.domain.Common;
using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AssetType :BaseEntity
{
    public int Id { get; set; }    

    public long? AssetCategoryId { get; set; }
    public long? TenantId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public virtual AssetCategory? AssetCategory { get; set; }

    public virtual ICollection<AssetTicketTypeDetail> AssetTicketTypeDetails { get; set; } = new List<AssetTicketTypeDetail>();


}
