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

    public virtual ICollection<TenderService> TenderService { get; set; } = new List<TenderService>();

    public virtual ICollection<TenderServiceProvider> TenderServiceProvider { get; set; } = new List<TenderServiceProvider>();
}
