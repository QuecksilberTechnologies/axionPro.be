using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LeaveRule
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long PolicyLeaveTypeId { get; set; }

    public bool ApplySandwichRule { get; set; }

    public bool? IsLinkedSandwichRule { get; set; }

    public bool IsHalfDayAllowed { get; set; }

    public int? HalfDayNoticeHours { get; set; }

    public int? NoticePeriodDays { get; set; }

    public int? MaxContinuousLeaves { get; set; }

    public int? MinGapBetweenLeaves { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public string? Remark { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public DateTime? SoftDeleteDateTime { get; set; }

    public long? SoftDeleteById { get; set; }

    public virtual ICollection<LeaveSandwichRuleMapping> LeaveSandwichRuleMappings { get; set; } = new List<LeaveSandwichRuleMapping>();

    public virtual PolicyLeaveTypeMapping PolicyLeaveType { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
