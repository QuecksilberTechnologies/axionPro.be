using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class WorkstationType
{
    public int Id { get; set; }

    public string Workstation { get; set; } = null!;

    public bool? IsActive { get; set; }

    public long AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<EmployeeDailyAttendance> EmployeeDailyAttendances { get; set; } = new List<EmployeeDailyAttendance>();

    public virtual ICollection<UserAttendanceSetting> UserAttendanceSettings { get; set; } = new List<UserAttendanceSetting>();
}
