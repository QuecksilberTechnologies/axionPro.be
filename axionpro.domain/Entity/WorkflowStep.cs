using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class WorkflowStep
{
    public long Id { get; set; }

    public long ApprovalWorkflowId { get; set; }

    public int ApprovalLevel { get; set; }

    public int ApproverType { get; set; }

    public long ApproverReferenceId { get; set; }

    public bool? IsMandatory { get; set; }

    public int? EscalateAfterDays { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
}
