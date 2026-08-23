// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines request, filter, status, and response contracts for TenantConfiguration management.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.DTOS.TenantConfiguration;

/// <summary>Supplies client-editable values for a Tenant location.</summary>
public class CreateTenantLocationRequestDTO
{
    #region Properties
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public TenantLocationType LocationType { get; set; }
    public int CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public bool IsHeadOffice { get; set; }
    public bool IsGeoFenceEnabled { get; set; }
    public bool IsAttendanceAllowed { get; set; } = true;
    public bool IsBiometricEnabled { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion
}

/// <summary>Supplies client-editable values for an existing Tenant location.</summary>
public sealed class UpdateTenantLocationRequestDTO : CreateTenantLocationRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies a Tenant-location active-state change.</summary>
public sealed class UpdateTenantLocationStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for Tenant locations.</summary>
public sealed class TenantLocationFilterRequestDTO
{
    public string? Search { get; set; }
    public int? CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }
    public TenantLocationType? LocationType { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes a Tenant location with geographic display context.</summary>
public sealed class TenantLocationResponseDTO
{
    public long Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public TenantLocationType LocationType { get; set; }
    public string LocationTypeName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
    public string? Address { get; set; }
    public string? Landmark { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? GeoFenceRadiusMeters { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public bool IsHeadOffice { get; set; }
    public bool IsGeoFenceEnabled { get; set; }
    public bool IsAttendanceAllowed { get; set; }
    public bool IsBiometricEnabled { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an attendance policy.</summary>
public class CreateAttendancePolicyRequestDTO
{
    #region Properties
    public int PolicyTypeId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public AttendanceLocationScope AttendanceLocationScope { get; set; }
    public bool AllowBiometric { get; set; } = true;
    public bool AllowMobile { get; set; }
    public bool AllowWeb { get; set; }
    public bool AllowManualAttendance { get; set; }
    public bool AllowWorkFromHome { get; set; }
    public bool RequireGeoFenceForOffice { get; set; } = true;
    public bool RequireGpsForRemote { get; set; } = true;
    public bool AllowOutsideLocationWithApproval { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion
}

/// <summary>Supplies client-editable values for an existing attendance policy.</summary>
public sealed class UpdateAttendancePolicyRequestDTO : CreateAttendancePolicyRequestDTO
{
    public int Id { get; set; }
}

/// <summary>Supplies an attendance-policy active-state change.</summary>
public sealed class UpdateAttendancePolicyStatusRequestDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for attendance policies.</summary>
public sealed class AttendancePolicyFilterRequestDTO
{
    public string? Search { get; set; }
    public int? PolicyTypeId { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes an attendance policy and its configured location scope.</summary>
public sealed class AttendancePolicyResponseDTO
{
    public int Id { get; set; }
    public int PolicyTypeId { get; set; }
    public string PolicyTypeName { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public AttendanceLocationScope AttendanceLocationScope { get; set; }
    public string AttendanceLocationScopeName { get; set; } = string.Empty;
    public bool AllowBiometric { get; set; }
    public bool AllowMobile { get; set; }
    public bool AllowWeb { get; set; }
    public bool AllowManualAttendance { get; set; }
    public bool AllowWorkFromHome { get; set; }
    public bool RequireGeoFenceForOffice { get; set; }
    public bool RequireGpsForRemote { get; set; }
    public bool AllowOutsideLocationWithApproval { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an employee-location assignment.</summary>
public class CreateEmployeeLocationAssignmentRequestDTO
{
    public long EmployeeId { get; set; }
    public long TenantLocationId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsAttendanceAllowed { get; set; } = true;
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing employee-location assignment.</summary>
public sealed class UpdateEmployeeLocationAssignmentRequestDTO : CreateEmployeeLocationAssignmentRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies an employee-location-assignment active-state change.</summary>
public sealed class UpdateEmployeeLocationAssignmentStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee-location assignments.</summary>
public sealed class EmployeeLocationAssignmentFilterRequestDTO
{
    public long? EmployeeId { get; set; }
    public long? TenantLocationId { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes one employee-to-location configuration record.</summary>
public sealed class EmployeeLocationAssignmentResponseDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public long TenantLocationId { get; set; }
    public string TenantLocationName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsAttendanceAllowed { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an employee device enrollment.</summary>
public class CreateEmployeeDeviceEnrollmentRequestDTO
{
    public long EmployeeId { get; set; }
    public long TenantDeviceId { get; set; }
    public string EnrollId { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public bool IsFaceEnrolled { get; set; }
    public bool IsFingerprintEnrolled { get; set; }
    public bool IsCardEnrolled { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing employee device enrollment.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentRequestDTO : CreateEmployeeDeviceEnrollmentRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies an employee-device-enrollment active-state change.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee device enrollments.</summary>
public sealed class EmployeeDeviceEnrollmentFilterRequestDTO
{
    public string? Search { get; set; }
    public long? EmployeeId { get; set; }
    public long? TenantDeviceId { get; set; }
    public long? TenantLocationId { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes an employee enrollment on a Host-managed physical device.</summary>
public sealed class EmployeeDeviceEnrollmentResponseDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public long TenantDeviceId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public long TenantLocationId { get; set; }
    public string TenantLocationName { get; set; } = string.Empty;
    public string EnrollId { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public bool IsFaceEnrolled { get; set; }
    public bool IsFingerprintEnrolled { get; set; }
    public bool IsCardEnrolled { get; set; }
    public DateTime? LastSyncedDateTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an employee work arrangement.</summary>
public class CreateEmployeeWorkArrangementRequestDTO
{
    public long EmployeeId { get; set; }
    public int AttendancePolicyId { get; set; }
    public long? PrimaryTenantLocationId { get; set; }
    public WorkMode WorkMode { get; set; }
    public HybridType? HybridType { get; set; }
    public short? MinimumOfficeDaysPerWeek { get; set; }
    public short? MinimumOfficeDaysPerMonth { get; set; }
    public short? MaximumWFHDaysPerMonth { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing employee work arrangement.</summary>
public sealed class UpdateEmployeeWorkArrangementRequestDTO : CreateEmployeeWorkArrangementRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies an employee-work-arrangement active-state change.</summary>
public sealed class UpdateEmployeeWorkArrangementStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work arrangements.</summary>
public sealed class EmployeeWorkArrangementFilterRequestDTO
{
    public long? EmployeeId { get; set; }
    public int? AttendancePolicyId { get; set; }
    public long? PrimaryTenantLocationId { get; set; }
    public WorkMode? WorkMode { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes an employee work arrangement with policy and location context.</summary>
public sealed class EmployeeWorkArrangementResponseDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int AttendancePolicyId { get; set; }
    public string AttendancePolicyName { get; set; } = string.Empty;
    public long? PrimaryTenantLocationId { get; set; }
    public string? PrimaryTenantLocationName { get; set; }
    public WorkMode WorkMode { get; set; }
    public string WorkModeName { get; set; } = string.Empty;
    public HybridType? HybridType { get; set; }
    public string? HybridTypeName { get; set; }
    public short? MinimumOfficeDaysPerWeek { get; set; }
    public short? MinimumOfficeDaysPerMonth { get; set; }
    public short? MaximumWFHDaysPerMonth { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an employee work-pattern day.</summary>
public class CreateEmployeeWorkPatternRequestDTO
{
    public long EmployeeWorkArrangementId { get; set; }
    public WorkPatternDay DayOfWeek { get; set; }
    public WorkMode WorkMode { get; set; }
    public long? TenantLocationId { get; set; }
    public bool IsWorkingDay { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing employee work-pattern day.</summary>
public sealed class UpdateEmployeeWorkPatternRequestDTO : CreateEmployeeWorkPatternRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies an employee-work-pattern active-state change.</summary>
public sealed class UpdateEmployeeWorkPatternStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work-pattern days.</summary>
public sealed class EmployeeWorkPatternFilterRequestDTO
{
    public long? EmployeeWorkArrangementId { get; set; }
    public WorkPatternDay? DayOfWeek { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes one employee work-pattern day.</summary>
public sealed class EmployeeWorkPatternResponseDTO
{
    public long Id { get; set; }
    public long EmployeeWorkArrangementId { get; set; }
    public WorkPatternDay DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public WorkMode WorkMode { get; set; }
    public string WorkModeName { get; set; } = string.Empty;
    public long? TenantLocationId { get; set; }
    public string? TenantLocationName { get; set; }
    public bool IsWorkingDay { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Supplies client-editable values for an employee temporary work-mode override.</summary>
public class CreateEmployeeWorkModeOverrideRequestDTO
{
    public long EmployeeId { get; set; }
    public long? EmployeeWorkArrangementId { get; set; }
    public WorkMode RequestedWorkMode { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public long? TenantLocationId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing temporary work-mode override.</summary>
public sealed class UpdateEmployeeWorkModeOverrideRequestDTO : CreateEmployeeWorkModeOverrideRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies an override record active-state change without exposing approval fields.</summary>
public sealed class UpdateEmployeeWorkModeOverrideStatusRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work-mode overrides.</summary>
public sealed class EmployeeWorkModeOverrideFilterRequestDTO
{
    public string? Search { get; set; }
    public long? EmployeeId { get; set; }
    public WorkMode? RequestedWorkMode { get; set; }
    public WorkModeOverrideApprovalStatus? ApprovalStatus { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes a temporary work-mode override, including read-only approval state.</summary>
public sealed class EmployeeWorkModeOverrideResponseDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public long? EmployeeWorkArrangementId { get; set; }
    public WorkMode RequestedWorkMode { get; set; }
    public string RequestedWorkModeName { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public long? TenantLocationId { get; set; }
    public string? TenantLocationName { get; set; }
    public string Reason { get; set; } = string.Empty;
    public WorkModeOverrideApprovalStatus ApprovalStatus { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public string? ApprovalRemark { get; set; }
    public string? RejectionRemark { get; set; }
    public bool IsActive { get; set; }
}
