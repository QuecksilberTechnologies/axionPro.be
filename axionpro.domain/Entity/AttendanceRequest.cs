using System;
using System.Collections.Generic;



public partial class Attendancerequest
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public DateTime Attendancedate { get; set; }

    public int Attendancedevicetypeid { get; set; }

    public int Workstationtypeid { get; set; }

    public double? Latitude { get; set; }

    public bool? Isactive { get; set; }

    public string? Remark { get; set; }

    public double? Longitude { get; set; }

    public string? Attendanceimagepath { get; set; }

    public string? Attendanceimageurl { get; set; }
}
