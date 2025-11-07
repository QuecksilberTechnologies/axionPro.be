using axionpro.domain.Common;
using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AssetStatus :BaseEntity
{
    public int Id { get; set; }

    public long? TenantId { get; set; }

    public string StatusName { get; set; } = null!;
    public string? ColorKey { get; set; } = null!;

    public string? Description { get; set; } 


    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
