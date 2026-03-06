using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class SubscriptionPlan
{
    public int Id { get; set; }

    public string PlanName { get; set; } = null!;

    public int MaxUsers { get; set; }

    public decimal? PerDayPrice { get; set; }

    public decimal MonthlyPrice { get; set; }

    public decimal? YearlyPrice { get; set; }

    public bool? IsFree { get; set; }

    public bool IsActive { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? AddedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public string? CurrencyKey { get; set; }

    public bool? IsMostPopular { get; set; }

    public bool? IsCustom { get; set; }

    public virtual ICollection<PlanModuleMapping> PlanModuleMappings { get; set; } = new List<PlanModuleMapping>();

    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
}
