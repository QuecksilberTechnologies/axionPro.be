using System;
using System.Collections.Generic;



public partial class Attendance
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public DateOnly Attendancedate { get; set; }

    public int Deviceid { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Attendanceimagepath { get; set; }

    public string? Attendanceimageurl { get; set; }
}
