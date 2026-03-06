using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ApprovalWorkflow
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long ProjectChildModuleDetailId { get; set; }

    public string ActionName { get; set; } = null!;

    public string WorkflowName { get; set; } = null!;

    public bool? IsSoftDeleted { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Remark { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
}
