using System;
using System.Collections.Generic;



public partial class Attendancehistory
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public DateOnly Attendancedate { get; set; }

    public DateTime? Intime { get; set; }

    public DateTime? Outtime { get; set; }

    public decimal? Totalworkhours { get; set; }

    public decimal? Totalbreakhours { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
