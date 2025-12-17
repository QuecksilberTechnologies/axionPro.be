using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Employee
{
   

    public long Id { get; set; }

    public long TenantId { get; set; }

    public int? EmployeeDocumentId { get; set; }

    public string? EmployementCode { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public int GenderId { get; set; }

    public string FirstName { get; set; }=string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public DateTime? DateOfOnBoarding { get; set; }

    public DateTime? DateOfExit { get; set; }

    public int DesignationId { get; set; }

    public int EmployeeTypeId { get; set; }

    public int DepartmentId { get; set; }

    public string OfficialEmail { get; set; }= string.Empty;

    public bool HasPermanent { get; set; }

    public bool IsActive { get; set; }

    public int? FunctionalId { get; set; }

    public int? ReferalId { get; set; }

    public string? Remark { get; set; }

    public bool? IsEditAllowed { get; set; }

    public bool? IsInfoVerified { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? InfoVerifiedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();

    public virtual ICollection<AssetHistory> AssetHistoryEmployees { get; set; } = new List<AssetHistory>();

    public virtual ICollection<AssetHistory> AssetHistoryScrapApprovedByNavigations { get; set; } = new List<AssetHistory>();

    public virtual ICollection<AttendanceHistory> AttendanceHistories { get; set; } = new List<AttendanceHistory>();

    public virtual Designation? Designation { get; set; }

    public virtual ICollection<EmployeeBankDetail> EmployeeBankDetails { get; set; } = new List<EmployeeBankDetail>();
    public virtual ICollection<EmployeeContact> EmployeeContacts { get; set; } = new List<EmployeeContact>();
    public virtual ICollection<EmployeeEducation> EmployeeEducations { get; set; } = new List<EmployeeEducation>();

    public virtual ICollection<EmployeeCategorySkill> EmployeeCategorySkills { get; set; } = new List<EmployeeCategorySkill>();

    public virtual ICollection<EmployeeDailyAttendance> EmployeeDailyAttendances { get; set; } = new List<EmployeeDailyAttendance>();

    public virtual ICollection<EmployeeDependent> EmployeeDependents { get; set; } = new List<EmployeeDependent>();

    public virtual ICollection<EmployeeImage> EmployeeImages { get; set; } = new List<EmployeeImage>();
    public virtual ICollection<LoginCredential> LoginCredentials { get; set; } = new List<LoginCredential>();

    public virtual ICollection<EmployeeExperienceDetail> EmployeeExperienceDetails { get; set; } = new List<EmployeeExperienceDetail>();

    public virtual ICollection<EmployeeExperiencePayslipUpload> EmployeeExperiencePayslipUploads { get; set; } = new List<EmployeeExperiencePayslipUpload>();


    public virtual ICollection<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMappings { get; set; } = new List<EmployeeLeavePolicyMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappingEmployees { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappingManagers { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();

    public virtual EmployeeType? EmployeeType { get; set; }

    public virtual ICollection<EmployeesChangedTypeHistory> EmployeesChangedTypeHistories { get; set; } = new List<EmployeesChangedTypeHistory>();

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<UserAttendanceSetting> UserAttendanceSettings { get; set; } = new List<UserAttendanceSetting>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
