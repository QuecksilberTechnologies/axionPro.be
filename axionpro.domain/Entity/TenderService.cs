using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderService
{
    public int Id { get; set; }

    public int TenderId { get; set; }

    public int TenderServiceTypeId { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public virtual Tender Tender { get; set; } = null!;

    public virtual ICollection<TenderServiceSpecification> TenderServiceSpecifications { get; set; } = new List<TenderServiceSpecification>();

    public virtual TenderServiceType TenderServiceType { get; set; } = null!;
}
