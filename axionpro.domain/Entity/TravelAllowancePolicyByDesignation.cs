using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TravelAllowancePolicyByDesignation
{
    public int Id { get; set; }

    public int PolicyTypeId { get; set; }

    public int EmployeeTypeId { get; set; }

    public int DesignationId { get; set; }

    public int? TravelModeId { get; set; }

    public string? TravelClass { get; set; }

    public int? MinTravelDistance { get; set; }

    public int? MaxTravelDistance { get; set; }

    public decimal? ReimbursementPerKm { get; set; }

    public bool? IsMetro { get; set; }

    public decimal? MetroBonus { get; set; }

    public string? RequiredDocuments { get; set; }

    public bool? AdvanceAllowed { get; set; }

    public decimal? MaxAdvanceAmount { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDelete { get; set; }

    public DateTime? SoftDeleteDateTime { get; set; }

    public int AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual Designation Designation { get; set; } = null!;

    public virtual EmployeeType EmployeeType { get; set; } = null!;

    public virtual PolicyType PolicyType { get; set; } = null!;
}
