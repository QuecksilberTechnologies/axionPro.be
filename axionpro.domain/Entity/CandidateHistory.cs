using System;
using System.Collections.Generic;



public partial class Candidatehistory
{
    public long Id { get; set; }

    public long Candidateid { get; set; }

    public string? Status { get; set; }

    public string Reason { get; set; } = null!;

    public long Addedbyid { get; set; }

    public DateTime? Reapplyallowedafter { get; set; }

    public DateTime Createddatetime { get; set; }
}
