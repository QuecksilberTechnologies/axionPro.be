using System;
using System.Collections.Generic;



public partial class Interviewsdule
{
    public long Id { get; set; }

    public DateTime Scheduleddatetime { get; set; }

    public long Interviewerid { get; set; }

    public string? Interviewmode { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public string? Description { get; set; }

    public DateTime Createddatetime { get; set; }
}
