using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DayCombination
{
    public int Id { get; set; }

    public long? TenantId { get; set; }

    public string CombinationName { get; set; } = null!;

    public int StartDay { get; set; }

    public int EndDay { get; set; }

    public string? Remark { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public virtual ICollection<LeaveSandwichRuleMapping> LeaveSandwichRuleMappings { get; set; } = new List<LeaveSandwichRuleMapping>();

    public virtual Tenant? Tenant { get; set; }
}
