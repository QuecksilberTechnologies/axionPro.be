using System;
using System.Collections.Generic;



public partial class Interviewschedule
{
    public int Id { get; set; }

    public long Candidateid { get; set; }

    public int Panelid { get; set; }

    public DateTime Scheduleddate { get; set; }

    public bool Isactive { get; set; }

    public string? Remarks { get; set; }
}
