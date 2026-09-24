using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AttendanceDeviceType
{
    public int Id { get; set; }

    public string? DeviceType { get; set; }

    public string DeviceTypeCode { get; set; } = string.Empty;

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeviceRegister { get; set; }

    public long AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<EmployeeDailyAttendance> EmployeeDailyAttendance { get; set; } = new List<EmployeeDailyAttendance>();

    public virtual ICollection<UserAttendanceSetting> UserAttendanceSetting { get; set; } = new List<UserAttendanceSetting>();

    public virtual ICollection<EmployeeAttendancePunch> EmployeeAttendancePunches { get; set; } = new List<EmployeeAttendancePunch>();
}
