using System;
using System.Collections.Generic;



public partial class Employeedailyattendance
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public DateTime Attendancedate { get; set; }

    public int Attendancedevicetypeid { get; set; }

    public int Workstationtypeid { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool? Islate { get; set; }

    public byte[]? Clickedimage { get; set; }

    public bool Isactive { get; set; }

    public bool Ismarked { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
