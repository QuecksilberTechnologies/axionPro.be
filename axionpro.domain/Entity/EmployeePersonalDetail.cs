using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeePersonalDetail
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public string? AadhaarNumber { get; set; }

    public string? PanNumber { get; set; }

    public string? PassportNumber { get; set; }

    public string? DrivingLicenseNumber { get; set; }

    public string? VoterId { get; set; }

    public string? BloodGroup { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Nationality { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public bool IsActive { get; set; }

    public bool? IsEditAllowed { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public long? InfoVerifiedById { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public bool? IsInfoVerified { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
