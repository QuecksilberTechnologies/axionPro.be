using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Tenant
{
    public long Id { get; set; }

    public int TenantIndustryId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? TenantCode { get; set; }

    public string CompanyEmailDomain { get; set; } = null!;

    public string TenantEmail { get; set; } = null!;

    public string? ContactPersonName { get; set; }

    public int? GenderId { get; set; }

    public string? ContactNumber { get; set; }

    public int CountryId { get; set; }

    public bool IsVerified { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<AssetCategory> AssetCategory { get; set; } = new List<AssetCategory>();

    public virtual ICollection<DayCombination> DayCombination { get; set; } = new List<DayCombination>();

    public virtual ICollection<Department> Department { get; set; } = new List<Department>();

    public virtual ICollection<Designation> Designation { get; set; } = new List<Designation>();

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();

    public virtual ICollection<EmployeeCodePattern> EmployeeCodePattern { get; set; } = new List<EmployeeCodePattern>();

    public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalance { get; set; } = new List<EmployeeLeaveBalance>();

    public virtual ICollection<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMapping { get; set; } = new List<EmployeeLeavePolicyMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMapping { get; set; } = new List<EmployeeManagerMapping>();

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequest { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRule> LeaveRule { get; set; } = new List<LeaveRule>();

    public virtual ICollection<LeaveType> LeaveType { get; set; } = new List<LeaveType>();

    public virtual ICollection<OrganizationHolidayCalendar> OrganizationHolidayCalendar { get; set; } = new List<OrganizationHolidayCalendar>();

    public virtual ICollection<PolicyType> PolicyType { get; set; } = new List<PolicyType>();

    public virtual ICollection<RequestType> RequestType { get; set; } = new List<RequestType>();

    public virtual ICollection<TenantEmailConfig> TenantEmailConfig { get; set; } = new List<TenantEmailConfig>();

    public virtual ICollection<TenantEnabledModule> TenantEnabledModule { get; set; } = new List<TenantEnabledModule>();

    public virtual ICollection<TenantEnabledOperation> TenantEnabledOperation { get; set; } = new List<TenantEnabledOperation>();

    public virtual TenantIndustry TenantIndustry { get; set; } = null!;

    public virtual ICollection<TenantProfile> TenantProfile { get; set; } = new List<TenantProfile>();

    public virtual ICollection<TenantSubscription> TenantSubscription { get; set; } = new List<TenantSubscription>();

    public virtual ICollection<TicketClassification> TicketClassification { get; set; } = new List<TicketClassification>();

    public virtual ICollection<TicketHeader> TicketHeader { get; set; } = new List<TicketHeader>();

    public virtual ICollection<TicketType> TicketType { get; set; } = new List<TicketType>();
}
