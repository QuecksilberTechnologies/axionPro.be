using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Employee
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public int? EmployeeDocumentId { get; set; }

    public string? EmployementCode { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public int? GenderId { get; set; }

    public string? FirstName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime? DateOfOnBoarding { get; set; }

    public DateTime? DateOfExit { get; set; }

    public int? DesignationId { get; set; }

    public int? EmployeeTypeId { get; set; }

    public int? DepartmentId { get; set; }

    public string? OfficialEmail { get; set; }

    public bool HasPermanent { get; set; }

    public bool IsActive { get; set; }

    public int? FunctionalId { get; set; }

    public int? ReferalId { get; set; }

    public string? Remark { get; set; }

    public bool IsEditAllowed { get; set; } = false;

    public bool IsInfoVerified { get; set; } = false;

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? InfoVerifiedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public bool IsSoftDeleted { get; set; } = false;

    public DateTime? InfoVerifiedDateTime { get; set; }

    public string? Description { get; set; }

    public int CountryId { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public int? Relation { get; set; }

    public string? BloodGroup { get; set; }

    public string? MobileNumber { get; set; }

    public bool IsMarried { get; set; } = false;

    public string? EmergencyContactPerson { get; set; }

    public virtual ICollection<AssetHistory> AssetHistoryEmployee { get; set; } = new List<AssetHistory>();
    public virtual ICollection<Ticket> TicketApprovedByUser { get; set; } = new List<Ticket>();
    public virtual ICollection<AssetHistory> AssetHistoryScrapApprovedByNavigation { get; set; } = new List<AssetHistory>();

    public virtual ICollection<AttendanceHistory> AttendanceHistory { get; set; } = new List<AttendanceHistory>();

    public virtual Country Country { get; set; } = null!;

    public virtual Designation? Designation { get; set; }

    public virtual ICollection<EmployeeBankDetail> EmployeeBankDetail { get; set; } = new List<EmployeeBankDetail>();

    public virtual ICollection<EmployeeCategorySkill> EmployeeCategorySkill { get; set; } = new List<EmployeeCategorySkill>();

    public virtual ICollection<EmployeeContact> EmployeeContact { get; set; } = new List<EmployeeContact>();

    public virtual ICollection<EmployeeDailyAttendance> EmployeeDailyAttendance { get; set; } = new List<EmployeeDailyAttendance>();

    public virtual ICollection<EmployeeDependent> EmployeeDependent { get; set; } = new List<EmployeeDependent>();

    public virtual ICollection<EmployeeExperience> EmployeeExperience { get; set; } = new List<EmployeeExperience>();

    public virtual ICollection<EmployeeIdentity> EmployeeIdentity { get; set; } = new List<EmployeeIdentity>();

    public virtual ICollection<EmployeeImage> EmployeeImage { get; set; } = new List<EmployeeImage>();

    public virtual ICollection<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMapping { get; set; } = new List<EmployeeLeavePolicyMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappingEmployee { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappingManager { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetail { get; set; } = new List<EmployeePersonalDetail>();

    public virtual ICollection<EmployeePolicyEnrollment> EmployeePolicyEnrollment { get; set; } = new List<EmployeePolicyEnrollment>();

    public virtual EmployeeType? EmployeeType { get; set; }

    public virtual ICollection<EmployeesChangedTypeHistory> EmployeesChangedTypeHistory { get; set; } = new List<EmployeesChangedTypeHistory>();

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequest { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LoginCredential> LoginCredential { get; set; } = new List<LoginCredential>();

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<ThreadMessage> ThreadMessage { get; set; } = new List<ThreadMessage>();

    public virtual ICollection<Ticket> TicketAssignedToUser { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketAttachment> TicketAttachment { get; set; } = new List<TicketAttachment>();

    public virtual ICollection<Ticket> TicketRecommendedByUser { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketRequestedByUser { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketRequestedForUser { get; set; } = new List<Ticket>();

    public virtual ICollection<UserAttendanceSetting> UserAttendanceSetting { get; set; } = new List<UserAttendanceSetting>();

    public virtual ICollection<UserRole> UserRole { get; set; } = new List<UserRole>();
}
