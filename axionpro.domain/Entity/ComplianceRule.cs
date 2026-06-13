using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ComplianceRule
{
    public long Id { get; set; }

    public int ComplianceTypeId { get; set; }

    public int CountryId { get; set; }

    public int? StateId { get; set; }

    public string RuleJson { get; set; } = null!;

    public int? Priority { get; set; }

    public long? TenantId { get; set; }

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

    public virtual ComplianceTypeMaster ComplianceType { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual State? State { get; set; }

    public virtual Tenant? Tenant { get; set; }
}
