using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InterviewSchedule
{
    public int Id { get; set; }

    public long CandidateId { get; set; }

    public int PanelId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public bool IsActive { get; set; }

    public string? Remarks { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual InterviewPanel Panel { get; set; } = null!;
}
