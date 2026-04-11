using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DeviceCommandMaster
{
    public long Id { get; set; }

    public string CommandName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
}
