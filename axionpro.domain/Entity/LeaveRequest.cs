using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LeaveRequest
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public bool? IsHalfDay { get; set; }

    public DateOnly? HalfDayDate { get; set; }

    public bool? IsFirstHalf { get; set; }

    public decimal TotalLeaveDays { get; set; }

    public string? Reason { get; set; }

    public int Status { get; set; }

    public long? ApprovedById { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public long LeavePolicyId { get; set; }

    public bool? IsSandwich { get; set; }

    public long CreatedById { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? CancellationDate { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public string? Remark { get; set; }

    public bool? IsDocumentAttached { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual PolicyLeaveTypeMapping LeavePolicy { get; set; } = null!;

    public virtual LeaveType LeaveType { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
