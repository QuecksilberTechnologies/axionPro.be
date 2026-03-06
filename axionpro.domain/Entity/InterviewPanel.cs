using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InterviewPanel
{
    public int Id { get; set; }

    public string PanelName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsApproved { get; set; }

    public string? Remarks { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<InterviewPanelMember> InterviewPanelMembers { get; set; } = new List<InterviewPanelMember>();

    public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
}
