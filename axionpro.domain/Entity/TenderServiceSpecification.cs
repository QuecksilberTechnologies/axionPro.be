using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderServiceSpecification
{
    public int Id { get; set; }

    public int TenderServiceId { get; set; }

    public string SpecificationType { get; set; } = null!;

    public string SpecificationName { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ProductPlatform { get; set; }

    public string? ProductSpecification { get; set; }

    public int? ExperienceRequired { get; set; }

    public int? NoticePeriodConsidered { get; set; }

    public decimal? EstimatedBudget { get; set; }

    public bool? IsActive { get; set; }

    public virtual TenderService TenderService { get; set; } = null!;

    public virtual ICollection<TenderServiceProvider> TenderServiceProviders { get; set; } = new List<TenderServiceProvider>();
}
