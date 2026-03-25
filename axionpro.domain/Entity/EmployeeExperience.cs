using System.Timers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.domain.Entity;

public class EmployeeExperience
{
    public long Id { get; set; }

    // 🔹 FK
    public long EmployeeId { get; set; }

    // 🔹 Business
    public decimal? Ctc { get; set; }
    public string? Comment { get; set; }

    public bool HasEPFAccount { get; set; } = false;
    public bool IsFresher { get; set; } = false;

    // 🔹 Status
    public bool IsActive { get; set; } = true;
    public bool IsSoftDeleted { get; set; } = false;
    public bool IsInfoVerified { get; set; } = false;
    public bool IsEditAllowed { get; set; } = true;

    // 🔹 Audit
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public DateTime? InfoVerifiedAt { get; set; }
    public DateTime? DeletedDateTime { get; set; }

    public long? UpdatedById { get; set; }
    public long? InfoVerifiedById { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }


    // 🔹 Navigation Properties
    public virtual Employee? Employee { get; set; }

    public virtual ICollection<EmployeeExperienceDetail> EmployeeExperienceDetails { get; set; } = new List<EmployeeExperienceDetail>();
}
 