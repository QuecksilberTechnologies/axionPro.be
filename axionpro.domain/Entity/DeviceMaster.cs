using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DeviceMaster
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public string DeviceSn { get; set; } = null!;

    public string? DeviceName { get; set; }

    public string? Location { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<DeviceCommandQueue> DeviceCommandQueue { get; set; } = new List<DeviceCommandQueue>();
}
