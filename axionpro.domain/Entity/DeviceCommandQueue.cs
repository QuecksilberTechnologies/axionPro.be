using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DeviceCommandQueue
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long DeviceId { get; set; }

    public string DeviceSn { get; set; } = null!;

    public string? CommandName { get; set; }

    public string CommandJson { get; set; } = null!;

    public int? Status { get; set; }

    public int? RetryCount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual DeviceMaster Device { get; set; } = null!;
}
