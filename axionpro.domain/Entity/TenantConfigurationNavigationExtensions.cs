// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Adds TenantConfiguration navigation collections to existing domain entities.
// ================================================================

namespace axionpro.domain.Entity;

/// <summary>Adds TenantConfiguration relationships to a Tenant.</summary>
public partial class Tenant
{
    public virtual ICollection<TenantLocation> TenantLocations { get; set; } = new List<TenantLocation>();
    public virtual ICollection<AttendancePolicy> AttendancePolicies { get; set; } = new List<AttendancePolicy>();
    public virtual ICollection<EmployeeLocationAssignment> EmployeeLocationAssignments { get; set; } = new List<EmployeeLocationAssignment>();
    public virtual ICollection<EmployeeDeviceEnrollment> EmployeeDeviceEnrollments { get; set; } = new List<EmployeeDeviceEnrollment>();
    public virtual ICollection<EmployeeWorkArrangement> EmployeeWorkArrangements { get; set; } = new List<EmployeeWorkArrangement>();
    public virtual ICollection<EmployeeWorkPattern> EmployeeWorkPatterns { get; set; } = new List<EmployeeWorkPattern>();
    public virtual ICollection<EmployeeWorkModeOverrideRequest> EmployeeWorkModeOverrideRequests { get; set; } = new List<EmployeeWorkModeOverrideRequest>();
    public virtual ICollection<TenantDevice> TenantDevices { get; set; } = new List<TenantDevice>();
}

/// <summary>Adds TenantConfiguration relationships to an Employee.</summary>
public partial class Employee
{
    public virtual ICollection<EmployeeLocationAssignment> EmployeeLocationAssignments { get; set; } = new List<EmployeeLocationAssignment>();
    public virtual ICollection<EmployeeDeviceEnrollment> EmployeeDeviceEnrollments { get; set; } = new List<EmployeeDeviceEnrollment>();
    public virtual ICollection<EmployeeWorkArrangement> EmployeeWorkArrangements { get; set; } = new List<EmployeeWorkArrangement>();
    public virtual ICollection<EmployeeWorkModeOverrideRequest> EmployeeWorkModeOverrideRequests { get; set; } = new List<EmployeeWorkModeOverrideRequest>();
}

/// <summary>Adds TenantConfiguration relationships to a PolicyType.</summary>
public partial class PolicyType
{
    public virtual ICollection<AttendancePolicy> AttendancePolicies { get; set; } = new List<AttendancePolicy>();
}

/// <summary>Adds TenantLocation relationships to a Country.</summary>
public partial class Country
{
    public virtual ICollection<TenantLocation> TenantLocations { get; set; } = new List<TenantLocation>();
}

/// <summary>Adds TenantLocation relationships to a City.</summary>
public partial class City
{
    public virtual ICollection<TenantLocation> TenantLocations { get; set; } = new List<TenantLocation>();
}
