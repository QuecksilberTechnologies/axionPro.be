using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class License
{
    public int Id { get; set; }

    public DateTime? LicenseStartDate { get; set; }

    public DateTime? LicenseEndDate { get; set; }

    public bool? IsActive { get; set; }
}
