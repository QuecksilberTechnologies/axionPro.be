using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderServiceHistory
{
    public int Id { get; set; }

    public int TenderServiceId { get; set; }

    public string Status { get; set; } = null!;

    public string? Remark { get; set; }
}
