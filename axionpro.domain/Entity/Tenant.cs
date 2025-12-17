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
    public int? GenderId { get; set; }
    public string TenantEmail { get; set; } = null!;

    public string? ContactPersonName { get; set; }

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

    public virtual ICollection<AssetCategory> AssetCategories { get; set; } = new List<AssetCategory>();

    public virtual ICollection<DayCombination> DayCombinations { get; set; } = new List<DayCombination>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Designation> Designations { get; set; } = new List<Designation>();

    public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

    public virtual ICollection<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMappings { get; set; } = new List<EmployeeLeavePolicyMapping>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappings { get; set; } = new List<EmployeeManagerMapping>();
    public virtual ICollection<TenantEnabledModule> TenantEnabledModules { get; set; } = new List<TenantEnabledModule>();

    public virtual ICollection<TenantEnabledOperation> TenantEnabledOperations { get; set; } = new List<TenantEnabledOperation>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
  

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRule> LeaveRules { get; set; } = new List<LeaveRule>();

    public virtual ICollection<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();

    public virtual ICollection<OrganizationHolidayCalendar> OrganizationHolidayCalendars { get; set; } = new List<OrganizationHolidayCalendar>();

    public virtual ICollection<PolicyType> PolicyTypes { get; set; } = new List<PolicyType>();

    public virtual ICollection<RequestType> RequestTypes { get; set; } = new List<RequestType>();

  //  public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<TenantEmailConfig> TenantEmailConfigs { get; set; } = new List<TenantEmailConfig>();

   
    public virtual TenantIndustry TenantIndustry { get; set; } = null!;

    public virtual Gender Gender { get; set; } = null!;

    public virtual ICollection<TenantProfile> TenantProfiles { get; set; } = new List<TenantProfile>();

    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();

    public virtual ICollection<TicketClassification> TicketClassifications { get; set; } = new List<TicketClassification>();

    public virtual ICollection<TicketHeader> TicketHeaders { get; set; } = new List<TicketHeader>();

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
}

