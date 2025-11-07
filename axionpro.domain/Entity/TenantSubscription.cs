using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenantSubscription
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public int SubscriptionPlanId { get; set; }

    public DateTime SubscriptionStartDate { get; set; }

    public DateTime SubscriptionEndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsTrial { get; set; }

    public string? PaymentTxnId { get; set; }

    public string? PaymentMode { get; set; }

    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
