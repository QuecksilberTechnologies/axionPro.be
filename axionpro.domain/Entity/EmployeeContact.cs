using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeContact
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public int? ContactType { get; set; }

    public string ContactNumber { get; set; } = null!;

    public string? AlternateNumber { get; set; }

    public string? Email { get; set; }

    public bool? IsPrimary { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? DistrictId { get; set; }

    public string? HouseNo { get; set; }

    public string? LandMark { get; set; }

    public string? Street { get; set; }

    public string? LocalAddress { get; set; }

    public string? PermanentAddress { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
    
    public bool? IsEditAllowed { get; set; }

    public bool? IsInfoVerified { get; set; }

    public long? InfoVerifiedById { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public string? Description { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
