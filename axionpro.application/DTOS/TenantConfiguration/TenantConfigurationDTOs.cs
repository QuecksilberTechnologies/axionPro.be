// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines request, filter, status, and response contracts for TenantConfiguration management.
// ================================================================

using axionpro.application.DTOs.BaseDTO;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace axionpro.application.DTOS.TenantConfiguration;

/// <summary>
/// Carries the encrypted Host-selected Tenant identifier and the module-operation
/// permission context required by TenantLocation endpoints.
/// </summary>
public class TenantLocationAccessRequestDTO : PermissionRequestDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier selected by a Host user.
    /// Tenant Employee requests are always scoped from their trusted token and
    /// therefore do not use this value.
    /// </summary>
    public string? TenantId { get; set; }
}

/// <summary>Supplies client-editable values for a Tenant location.</summary>
public class CreateTenantLocationRequestDTO : TenantLocationAccessRequestDTO
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
public sealed class UpdateTenantLocationStatusRequestDTO : TenantLocationAccessRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for Tenant locations.</summary>
public sealed class TenantLocationFilterRequestDTO : TenantLocationAccessRequestDTO
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
public class CreateAttendancePolicyRequestDTO : PermissionRequestDTO
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
public sealed class UpdateAttendancePolicyStatusRequestDTO : PermissionRequestDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for attendance policies.</summary>
public sealed class AttendancePolicyFilterRequestDTO : PermissionRequestDTO
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
public class CreateEmployeeLocationAssignmentRequestDTO : PermissionRequestDTO
{
    public string EmployeeId { get; set; } = string.Empty;
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
public sealed class UpdateEmployeeLocationAssignmentStatusRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee-location assignments.</summary>
public sealed class EmployeeLocationAssignmentFilterRequestDTO : PermissionRequestDTO
{
    public string? EmployeeId { get; set; }
    [JsonIgnore] public long? ResolvedEmployeeId { get; set; }
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
public class CreateEmployeeDeviceEnrollmentRequestDTO : PermissionRequestDTO
{
    public string EmployeeId { get; set; } = string.Empty;
    /// <summary>Encrypted Tenant device identifier. The server resolves the physical device and location.</summary>
    public string TenantDeviceId { get; set; } = string.Empty;
    /// <summary>Optional encrypted Host-issued card identifier. A typed card number is never accepted here.</summary>
    public string? TenantCardId { get; set; }
    /// <summary>Optional inclusive validity period for all configured weekly access windows.</summary>
    public DateTime? AccessEffectiveFromDateTime { get; set; }
    public DateTime? AccessEffectiveToDateTime { get; set; }
    /// <summary>Weekly employee/device access windows; the API owns their persistence and lifecycle.</summary>
    public List<EmployeeDeviceAccessWindowRequestDTO> AccessWindows { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

/// <summary>One local-time weekly access window submitted with an employee-device enrollment.</summary>
public sealed class EmployeeDeviceAccessWindowRequestDTO
{
    public WorkPatternDay DayOfWeek { get; set; }
    public TimeOnly StartLocalTime { get; set; }
    public TimeOnly EndLocalTime { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Supplies client-editable values for an existing employee device enrollment.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentRequestDTO : CreateEmployeeDeviceEnrollmentRequestDTO
{
    public string Id { get; set; } = string.Empty;
}

/// <summary>Supplies an employee-device-enrollment active-state change.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentStatusRequestDTO : PermissionRequestDTO
{
    public string Id { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee device enrollments.</summary>
public sealed class EmployeeDeviceEnrollmentFilterRequestDTO : PermissionRequestDTO
{
    public string? Search { get; set; }
    public string? EmployeeId { get; set; }
    [JsonIgnore] public long? ResolvedEmployeeId { get; set; }
    public string? TenantDeviceId { get; set; }
    [JsonIgnore] public long? ResolvedTenantDeviceId { get; set; }
    public long? TenantLocationId { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Describes an employee enrollment on a Host-managed physical device.</summary>
public sealed class EmployeeDeviceEnrollmentResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public string TenantDeviceId { get; set; } = string.Empty;
    public string DeviceCode { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string TenantLocationId { get; set; } = string.Empty;
    public string TenantLocationName { get; set; } = string.Empty;
    /// <summary>Device-only global employee identifier. It is intentionally not returned to Angular.</summary>
    public string? TenantCardId { get; set; }
    public string? MaskedCardNumber { get; set; }
    public bool IsFaceEnrolled { get; set; }
    public bool IsFingerprintEnrolled { get; set; }
    public bool IsCardEnrolled { get; set; }
    public DeviceCredentialDeploymentStatus FaceDeploymentStatus { get; set; }
    public DeviceCredentialDeploymentStatus CardDeploymentStatus { get; set; }
    public DeviceCredentialDeploymentStatus PinDeploymentStatus { get; set; }
    public DeviceCommandStatus? FaceCommandStatus { get; set; }
    public DeviceCommandStatus? CardCommandStatus { get; set; }
    public DeviceCommandStatus? PinCommandStatus { get; set; }
    /// <summary>Live confirmation state for the most recent enable/disable command.</summary>
    public DeviceCommandStatus? UserActivationCommandStatus { get; set; }
    public DateTime? AccessEffectiveFromDateTime { get; set; }
    public DateTime? AccessEffectiveToDateTime { get; set; }
    public List<EmployeeDeviceAccessWindowResponseDTO> AccessWindows { get; set; } = new();
    public DateTime? LastSyncedDateTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>One read-only weekly access window returned as part of its enrollment.</summary>
public sealed class EmployeeDeviceAccessWindowResponseDTO
{
    public WorkPatternDay DayOfWeek { get; set; }
    public TimeOnly StartLocalTime { get; set; }
    public TimeOnly EndLocalTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Queues an uploaded face photo for one existing employee-device enrollment.</summary>
public sealed class UpsertEmployeeDeviceFaceRequestDTO : PermissionRequestDTO
{
    public string EnrollmentId { get; set; } = string.Empty;
    public IFormFile? FaceImage { get; set; }
}

/// <summary>Queues a write-only employee device PIN create or replacement.</summary>
public sealed class UpsertEmployeeDevicePinRequestDTO : PermissionRequestDTO
{
    public string EnrollmentId { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}

/// <summary>Queues card association change for one existing employee-device enrollment.</summary>
public sealed class BindEmployeeDeviceCardRequestDTO : PermissionRequestDTO
{
    public string EnrollmentId { get; set; } = string.Empty;
    public string TenantCardId { get; set; } = string.Empty;
}

/// <summary>Queues removal of one employee credential from a physical device.</summary>
public sealed class RemoveEmployeeDeviceCredentialRequestDTO : PermissionRequestDTO
{
    public string EnrollmentId { get; set; } = string.Empty;
    public EmployeeDeviceCredentialType CredentialType { get; set; }
}

/// <summary>Names only the employee credentials which can be remotely removed.</summary>
public enum EmployeeDeviceCredentialType : short
{
    Face = 1,
    Card = 2,
    Pin = 3
}

/// <summary>Supplies client-editable values for an employee work arrangement.</summary>
public class CreateEmployeeWorkArrangementRequestDTO : PermissionRequestDTO
{
    public string EmployeeId { get; set; } = string.Empty;
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
public sealed class UpdateEmployeeWorkArrangementStatusRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work arrangements.</summary>
public sealed class EmployeeWorkArrangementFilterRequestDTO : PermissionRequestDTO
{
    public string? EmployeeId { get; set; }
    [JsonIgnore] public long? ResolvedEmployeeId { get; set; }
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
public class CreateEmployeeWorkPatternRequestDTO : PermissionRequestDTO
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
public sealed class UpdateEmployeeWorkPatternStatusRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work-pattern days.</summary>
public sealed class EmployeeWorkPatternFilterRequestDTO : PermissionRequestDTO
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
public class CreateEmployeeWorkModeOverrideRequestDTO : PermissionRequestDTO
{
    public string EmployeeId { get; set; } = string.Empty;
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
public sealed class UpdateEmployeeWorkModeOverrideStatusRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Defines database-side filters for employee work-mode overrides.</summary>
public sealed class EmployeeWorkModeOverrideFilterRequestDTO : PermissionRequestDTO
{
    public string? Search { get; set; }
    public string? EmployeeId { get; set; }
    [JsonIgnore] public long? ResolvedEmployeeId { get; set; }
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
