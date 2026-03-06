using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderServiceProvider
{
    public int Id { get; set; }

    public int TenderServiceSpecificationId { get; set; }

    public int ServiceProviderId { get; set; }

    public bool? IsInHouse { get; set; }

    public decimal? ContractAmount { get; set; }

    public DateOnly? ContractStartDate { get; set; }

    public DateOnly? ContractEndDate { get; set; }

    public bool? IsPrimaryProvider { get; set; }

    public int StatusId { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual TenderStatus Status { get; set; } = null!;

    public virtual ICollection<TenderProject> TenderProjects { get; set; } = new List<TenderProject>();

    public virtual TenderServiceSpecification TenderServiceSpecification { get; set; } = null!;

    public virtual TenderServiceType TenderServiceSpecificationNavigation { get; set; } = null!;
}
