using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InterviewFeedback
{
    public long Id { get; set; }

    public long InterviewScheduleId { get; set; }

    public long CandidateId { get; set; }

    public string Feedback { get; set; } = null!;

    public decimal? Rating { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ReapplyAfter { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual InterviewSdule InterviewSchedule { get; set; } = null!;
}
