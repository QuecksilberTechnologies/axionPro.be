using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AttendanceHistory
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? InTime { get; set; }

    public DateTime? OutTime { get; set; }

    public decimal? TotalWorkHours { get; set; }

    public decimal? TotalBreakHours { get; set; }

    public string Status { get; set; } = null!;

    public string? Remarks { get; set; }

    public long AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
