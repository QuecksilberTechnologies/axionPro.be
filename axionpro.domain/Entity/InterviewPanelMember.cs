using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InterviewPanelMember
{
    public int Id { get; set; }

    public int PanelId { get; set; }

    public long UserRoleId { get; set; }

    public bool IsActive { get; set; }

    public bool IsApproved { get; set; }

    public string? Description { get; set; }

    public string? Remarks { get; set; }

    public virtual InterviewPanel Panel { get; set; } = null!;

    public virtual UserRole UserRole { get; set; } = null!;
}
