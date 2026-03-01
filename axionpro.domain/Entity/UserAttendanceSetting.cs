using System;
using System.Collections.Generic;



public partial class Userattendancesetting
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public int Attendancedevicetypeid { get; set; }

    public int Workstationtypeid { get; set; }

    public bool Isallowed { get; set; }

    public string? Remark { get; set; }

    public decimal? Geofencelatitude { get; set; }

    public decimal? Geofencelongitude { get; set; }

    public bool Isgeofenceenabled { get; set; }

    public bool Isactive { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Reportingtime { get; set; }
}
