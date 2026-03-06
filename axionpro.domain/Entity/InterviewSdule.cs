using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InterviewSdule
{
    public long Id { get; set; }

    public DateTime ScheduledDateTime { get; set; }

    public long InterviewerId { get; set; }

    public string InterviewMode { get; set; } = null!;

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public virtual ICollection<InterviewFeedback> InterviewFeedbacks { get; set; } = new List<InterviewFeedback>();
}
