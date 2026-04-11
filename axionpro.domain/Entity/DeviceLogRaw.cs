using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DeviceLogRaw
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public string DeviceSn { get; set; } = null!;

    public string RawJson { get; set; } = null!;

    public string? CommandName { get; set; }

    public bool? IsProcessed { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? CreatedDate { get; set; }
}
