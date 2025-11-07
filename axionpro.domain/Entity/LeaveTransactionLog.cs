using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LeaveTransactionLog
{
    public long Id { get; set; }

    public long? EmployeeLeaveBalanceId { get; set; }

    public string? TransactionType { get; set; }

    public decimal? LeaveDays { get; set; }

    public DateTime? TransactionDate { get; set; }

    public long? PerformedBy { get; set; }

    public string? Remarks { get; set; }
}
