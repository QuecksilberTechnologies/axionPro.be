using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperience
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public string? CompanyName { get; set; }

    public decimal? Ctc { get; set; }

    public string? Designation { get; set; }

    public string? EmployeeIdOfCompany { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Experience { get; set; }

    public bool IsWfh { get; set; } =false;

    public int? WorkingCountryId { get; set; }

    public int? WorkingStateId { get; set; }

    public int? WorkingDistrictId { get; set; }

    public bool IsForeignExperience { get; set; } =false;

    public string? ReasonForLeaving { get; set; }

    public string? Remark { get; set; }

    public string? ColleagueName { get; set; }

    public string? ColleagueDesignation { get; set; }

    public string? ColleagueContactNumber { get; set; }

    public string? ReportingManagerName { get; set; }

    public string? ReportingManagerNumber { get; set; }

    public string? VerificationEmail { get; set; }

    public bool IsAnyGap { get; set; } = false;

    public string? ReasonOfGap { get; set; }

    public DateTime? GapYearFrom { get; set; }

    public DateTime? GapYearTo { get; set; }

    public bool? IsExperienceVerified { get; set; }

    public bool? IsExperienceVerifiedByMail { get; set; }

    public bool? IsExperienceVerifiedByCall { get; set; }

    public long? InfoVerifiedById { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public bool? IsInfoVerified { get; set; }

    public bool? IsEditAllowed { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public long AddedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<EmployeeExperienceDocument> EmployeeExperienceDocument { get; set; } = new List<EmployeeExperienceDocument>();
}
