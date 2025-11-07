using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderProject
{
    public int Id { get; set; }

    public int TenderServiceProviderId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public long UserRoleId { get; set; }

    public int StatusId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? EstimatedBudget { get; set; }

    public bool? IsActive { get; set; }

    public int? ExpectedTeamSize { get; set; }

    public virtual TenderStatus Status { get; set; } = null!;

    public virtual TenderServiceProvider TenderServiceProvider { get; set; } = null!;

    public virtual UserRole UserRole { get; set; } = null!;
}
