using System;
using System.Collections.Generic;



public partial class Interviewfeedback
{
    public long Id { get; set; }

    public long Interviewscheduleid { get; set; }

    public long Candidateid { get; set; }

    public string Feedback { get; set; } = null!;

    public decimal? Rating { get; set; }

    public string? Status { get; set; }

    public DateTime? Reapplyafter { get; set; }

    public DateTime Createddatetime { get; set; }
}
