using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderServiceType
{
    public int Id { get; set; }

    public int? ParentServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TenderServiceProvider> TenderServiceProviders { get; set; } = new List<TenderServiceProvider>();

    public virtual ICollection<TenderService> TenderServices { get; set; } = new List<TenderService>();
}
