using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TaxRule
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public int CountryId { get; set; }

    public string RuleType { get; set; } = null!;

    public string RuleCode { get; set; } = null!;

    public string RuleJson { get; set; } = null!;

    public string FinancialYear { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual Tenant? Tenant { get; set; }
}
