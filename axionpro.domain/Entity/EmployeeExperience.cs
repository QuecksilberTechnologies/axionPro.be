using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperience
{
    // 🔹 Primary Key
    public long Id { get; set; }

    public decimal? Ctc { get; set; }

    // 🔹 Foreign Key
    public long EmployeeId { get; set; }

    // 🔹 Navigation (IMPORTANT)
    public virtual Employee Employee { get; set; } = null!;

    // 🔹 Job Info
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public string? EmployeeIdOfCompany { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? Experience { get; set; }
    public bool IsWFH { get; set; } = false;

    // 🔹 Location
    public int? WorkingCountryId { get; set; }
    public int? WorkingStateId { get; set; }
    public int? WorkingDistrictId { get; set; }

    public bool IsForeignExperience { get; set; } = false;

    // 🔹 Exit Info
    public string? ReasonForLeaving { get; set; }
    public string? Remark { get; set; }

    // 🔹 Reporting Info
    public string? ColleagueName { get; set; }
    public string? ColleagueDesignation { get; set; }
    public string? ColleagueContactNumber { get; set; }

    public string? ReportingManagerName { get; set; }
    public string? ReportingManagerNumber { get; set; }

    public string? VerificationEmail { get; set; }

    // 🔹 Gap Info
    public bool IsAnyGap { get; set; } = false;
    public string? ReasonOfGap { get; set; }

    public DateTime? GapYearFrom { get; set; }
    public DateTime? GapYearTo { get; set; }

    // 🔹 Verification
    public bool? IsExperienceVerified { get; set; }
    public bool? IsExperienceVerifiedByMail { get; set; }
    public bool? IsExperienceVerifiedByCall { get; set; }

    public long? InfoVerifiedById { get; set; }
    public DateTime? InfoVerifiedDateTime { get; set; }

    public bool? IsInfoVerified { get; set; }
    public bool? IsEditAllowed { get; set; }

    // 🔹 Audit
    public bool IsActive { get; set; } = true;
    public bool IsSoftDeleted { get; set; } = false;
    public long? SoftDeletedById { get; set; }

    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public DateTime? DeletedDateTime { get; set; }

    // 🔹 Child Navigation
    public virtual ICollection<EmployeeExperienceDocument> EmployeeExperienceDocuments { get; set; }
        = new List<EmployeeExperienceDocument>();
}

