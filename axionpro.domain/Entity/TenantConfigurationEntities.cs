// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents TenantConfiguration tables and the shared read-only TenantDevice model.
// ================================================================


namespace axionpro.domain.Entity;

/// <summary>Represents a Tenant-owned physical work location.</summary>
public partial class TenantLocation
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string LocationCode { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public short LocationType { get; set; }
    public int CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string TimeZoneId { get; set; } = null!;
    public bool IsHeadOffice { get; set; }
    public bool IsGeoFenceEnabled { get; set; }
    public bool IsAttendanceAllowed { get; set; }
    public bool IsBiometricEnabled { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Country Country { get; set; } = null!;
    public virtual City? City { get; set; }
    public virtual ICollection<TenantDevice> TenantDevice { get; set; } = new List<TenantDevice>();
    public virtual ICollection<EmployeeLocationAssignment> EmployeeLocationAssignment { get; set; } = new List<EmployeeLocationAssignment>();
    public virtual ICollection<EmployeeWorkArrangement> EmployeeWorkArrangement { get; set; } = new List<EmployeeWorkArrangement>();
    public virtual ICollection<EmployeeWorkPattern> EmployeeWorkPattern { get; set; } = new List<EmployeeWorkPattern>();
    public virtual ICollection<EmployeeWorkModeOverrideRequest> EmployeeWorkModeOverrideRequest { get; set; } = new List<EmployeeWorkModeOverrideRequest>();
}

/// <summary>Represents executable attendance configuration for a Tenant policy type.</summary>
public partial class AttendancePolicy
{
    public int Id { get; set; }
    public long TenantId { get; set; }
    public int PolicyTypeId { get; set; }
    public string PolicyName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public short AttendanceLocationScope { get; set; }
    public bool AllowBiometric { get; set; }
    public bool AllowMobile { get; set; }
    public bool AllowWeb { get; set; }
    public bool AllowManualAttendance { get; set; }
    public bool AllowWorkFromHome { get; set; }
    public bool RequireGeoFenceForOffice { get; set; }
    public bool RequireGpsForRemote { get; set; }
    public bool AllowOutsideLocationWithApproval { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual PolicyType PolicyType { get; set; } = null!;
    public virtual ICollection<EmployeeWorkArrangement> EmployeeWorkArrangement { get; set; } = new List<EmployeeWorkArrangement>();
}

/// <summary>Represents an employee's eligible Tenant work location.</summary>
public partial class EmployeeLocationAssignment
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long TenantLocationId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsAttendanceAllowed { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
    public virtual TenantLocation TenantLocation { get; set; } = null!;
}

/// <summary>Represents an employee's enrollment identifier on a Host-managed Tenant device.</summary>
public partial class EmployeeDeviceEnrollment
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long TenantDeviceId { get; set; }
    public string EnrollId { get; set; } = null!;
    public string? CardNumber { get; set; }
    public bool IsFaceEnrolled { get; set; }
    public bool IsFingerprintEnrolled { get; set; }
    public bool IsCardEnrolled { get; set; }
    public DateTime? LastSyncedDateTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
}

/// <summary>Represents an employee's active or historical work arrangement.</summary>
public partial class EmployeeWorkArrangement
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public int AttendancePolicyId { get; set; }
    public long? PrimaryTenantLocationId { get; set; }
    public short WorkMode { get; set; }
    public short? HybridType { get; set; }
    public short? MinimumOfficeDaysPerWeek { get; set; }
    public short? MinimumOfficeDaysPerMonth { get; set; }
    public short? MaximumWFHDaysPerMonth { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
    public virtual AttendancePolicy AttendancePolicy { get; set; } = null!;
    public virtual TenantLocation? PrimaryTenantLocation { get; set; }
    public virtual ICollection<EmployeeWorkPattern> EmployeeWorkPattern { get; set; } = new List<EmployeeWorkPattern>();
    public virtual ICollection<EmployeeWorkModeOverrideRequest> EmployeeWorkModeOverrideRequest { get; set; } = new List<EmployeeWorkModeOverrideRequest>();
}

/// <summary>Represents one day in an employee work-arrangement pattern.</summary>
public partial class EmployeeWorkPattern
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeWorkArrangementId { get; set; }
    public short DayOfWeek { get; set; }
    public short WorkMode { get; set; }
    public long? TenantLocationId { get; set; }
    public bool IsWorkingDay { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual EmployeeWorkArrangement EmployeeWorkArrangement { get; set; } = null!;
    public virtual TenantLocation? TenantLocation { get; set; }
}

/// <summary>Represents a temporary work-mode deviation pending a future approval workflow.</summary>
public partial class EmployeeWorkModeOverrideRequest
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long? EmployeeWorkArrangementId { get; set; }
    public short RequestedWorkMode { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public long? TenantLocationId { get; set; }
    public string Reason { get; set; } = null!;
    public short ApprovalStatus { get; set; }
    public long? ApprovedById { get; set; }
    public DateTime? ApprovedDateTime { get; set; }
    public string? ApprovalRemark { get; set; }
    public long? RejectedById { get; set; }
    public DateTime? RejectedDateTime { get; set; }
    public string? RejectionRemark { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
    public virtual EmployeeWorkArrangement? EmployeeWorkArrangement { get; set; }
    public virtual TenantLocation? TenantLocation { get; set; }
}

/// <summary>Represents a Host-managed physical device installed for a Tenant.</summary>
public partial class TenantDevice
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long TenantLocationId { get; set; }
    public long DeviceMasterId { get; set; }
    public string DeviceCode { get; set; } = null!;
    public string? DeviceName { get; set; }
    public DateTime? InstalledDateTime { get; set; }
    public long? InstalledBy { get; set; }
    public string? InstallationRemark { get; set; }
    public bool IsAttendanceDevice { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public long? SoftDeletedById { get; set; }
    public DateTime? SoftDeletedDateTime { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual TenantLocation TenantLocation { get; set; } = null!;
    public virtual DeviceMaster DeviceMaster { get; set; } = null!;
    public virtual TenantDeviceConfiguration? TenantDeviceConfiguration { get; set; }
}

/// <summary>Represents one connection and runtime configuration for a Tenant device.</summary>
public partial class TenantDeviceConfiguration
{
    public long Id { get; set; }
    public long TenantDeviceId { get; set; }
    public string? IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public int? DevicePort { get; set; }
    /// <summary>
    /// Legacy connection transport retained only so existing HTTP device configuration
    /// values are not reinterpreted by the MQTT/MQTTS production stack.
    /// </summary>
    public short? CommunicationType { get; set; }
    /// <summary>MQTT/MQTTS transport selected for the new device-command infrastructure.</summary>
    public short? MqttTransport { get; set; }
    public string? ServerHost { get; set; }
    public int? ServerPort { get; set; }
    public string? ServerPath { get; set; }
    public string? ServerUrl { get; set; }
    public string? PushMode { get; set; }
    public int? HeartbeatIntervalSeconds { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Configuration { get; set; }
    public bool IsEnrollmentEnabled { get; set; }
    public bool IsAttendancePushEnabled { get; set; }
    public bool IsAutoSyncEnabled { get; set; }
    public DateTime? LastHeartbeatDateTime { get; set; }
    public DateTime? LastSyncDateTime { get; set; }
    public DateTime? LastAttendanceReceivedDateTime { get; set; }
    public DateTime? LastSuccessfulConnectionDateTime { get; set; }
    public DateTime? LastFailedConnectionDateTime { get; set; }
    public string? LastConnectionError { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public virtual TenantDevice TenantDevice { get; set; } = null!;
}
