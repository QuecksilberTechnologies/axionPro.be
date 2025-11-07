using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderStatus
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TenderProject> TenderProjects { get; set; } = new List<TenderProject>();

    public virtual ICollection<TenderServiceProvider> TenderServiceProviders { get; set; } = new List<TenderServiceProvider>();

    public virtual ICollection<Tender> Tenders { get; set; } = new List<Tender>();
}
