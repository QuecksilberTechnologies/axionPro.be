using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class AttendanceLog
{
    public long Id { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public DateTime PunchTime { get; set; }

    public string? DeviceSn { get; set; }

    public int DeviceType { get; set; }

    public int? PunchMode { get; set; }

    public int? InOut { get; set; }

    public int? EventType { get; set; }

    public double? Temperature { get; set; }

    public string? ImageBase64 { get; set; }

    public DateTime? CreatedDate { get; set; }
}
