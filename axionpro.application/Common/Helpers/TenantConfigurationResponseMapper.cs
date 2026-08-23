// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Maps TenantConfiguration entities to safe response DTOs with enum display names.
// ================================================================

using axionpro.application.DTOS.TenantConfiguration;
using axionpro.domain.Entity;

namespace axionpro.application.Common.Helpers;

/// <summary>Provides non-persistent response projections for TenantConfiguration entities.</summary>
public static class TenantConfigurationResponseMapper
{
    /// <summary>Maps a Tenant location and its geographic navigation properties.</summary>
    public static TenantLocationResponseDTO ToResponse(TenantLocation entity) => new()
    {
        Id = entity.Id, LocationCode = entity.LocationCode, LocationName = entity.LocationName, LocationType = entity.LocationType,
        LocationTypeName = entity.LocationType.ToString(), CountryId = entity.CountryId, CountryName = entity.Country?.CountryName ?? string.Empty,
        StateId = entity.StateId, StateName = entity.State?.StateName, CityId = entity.CityId, CityName = entity.City?.CityName,
        Address = entity.Address, Landmark = entity.Landmark, PostalCode = entity.PostalCode, Latitude = entity.Latitude, Longitude = entity.Longitude,
        GeoFenceRadiusMeters = entity.GeoFenceRadiusMeters, TimeZoneId = entity.TimeZoneId, IsHeadOffice = entity.IsHeadOffice,
        IsGeoFenceEnabled = entity.IsGeoFenceEnabled, IsAttendanceAllowed = entity.IsAttendanceAllowed, IsBiometricEnabled = entity.IsBiometricEnabled, IsActive = entity.IsActive
    };

    /// <summary>Maps an attendance policy and its PolicyType context.</summary>
    public static AttendancePolicyResponseDTO ToResponse(AttendancePolicy entity) => new()
    {
        Id = entity.Id, PolicyTypeId = entity.PolicyTypeId, PolicyTypeName = entity.PolicyType?.PolicyName ?? string.Empty, PolicyName = entity.PolicyName,
        Description = entity.Description, Remark = entity.Remark, AttendanceLocationScope = entity.AttendanceLocationScope,
        AttendanceLocationScopeName = entity.AttendanceLocationScope.ToString(), AllowBiometric = entity.AllowBiometric, AllowMobile = entity.AllowMobile,
        AllowWeb = entity.AllowWeb, AllowManualAttendance = entity.AllowManualAttendance, AllowWorkFromHome = entity.AllowWorkFromHome,
        RequireGeoFenceForOffice = entity.RequireGeoFenceForOffice, RequireGpsForRemote = entity.RequireGpsForRemote,
        AllowOutsideLocationWithApproval = entity.AllowOutsideLocationWithApproval, IsActive = entity.IsActive
    };

    /// <summary>Maps an employee-location assignment and display context.</summary>
    public static EmployeeLocationAssignmentResponseDTO ToResponse(EmployeeLocationAssignment entity) => new()
    {
        Id = entity.Id, EmployeeId = entity.EmployeeId, EmployeeName = EmployeeName(entity.Employee), EmployeeCode = entity.Employee?.EmployementCode,
        TenantLocationId = entity.TenantLocationId, TenantLocationName = entity.TenantLocation?.LocationName ?? string.Empty,
        LocationCode = entity.TenantLocation?.LocationCode ?? string.Empty, IsPrimary = entity.IsPrimary, IsAttendanceAllowed = entity.IsAttendanceAllowed,
        EffectiveFrom = entity.EffectiveFrom, EffectiveTo = entity.EffectiveTo, IsActive = entity.IsActive
    };

    /// <summary>Maps an employee device enrollment and read-only device context.</summary>
    public static EmployeeDeviceEnrollmentResponseDTO ToResponse(EmployeeDeviceEnrollment entity) => new()
    {
        Id = entity.Id, EmployeeId = entity.EmployeeId, EmployeeName = EmployeeName(entity.Employee), EmployeeCode = entity.Employee?.EmployementCode,
        TenantDeviceId = entity.TenantDeviceId, DeviceCode = entity.TenantDevice?.DeviceCode ?? string.Empty, DeviceName = entity.TenantDevice?.DeviceName,
        SerialNumber = entity.TenantDevice?.SerialNumber ?? string.Empty, TenantLocationId = entity.TenantDevice?.TenantLocationId ?? 0,
        TenantLocationName = entity.TenantDevice?.TenantLocation?.LocationName ?? string.Empty, EnrollId = entity.EnrollId, CardNumber = entity.CardNumber,
        IsFaceEnrolled = entity.IsFaceEnrolled, IsFingerprintEnrolled = entity.IsFingerprintEnrolled, IsCardEnrolled = entity.IsCardEnrolled,
        LastSyncedDateTime = entity.LastSyncedDateTime, IsActive = entity.IsActive
    };

    /// <summary>Maps an employee work arrangement and its display context.</summary>
    public static EmployeeWorkArrangementResponseDTO ToResponse(EmployeeWorkArrangement entity) => new()
    {
        Id = entity.Id, EmployeeId = entity.EmployeeId, EmployeeName = EmployeeName(entity.Employee), AttendancePolicyId = entity.AttendancePolicyId,
        AttendancePolicyName = entity.AttendancePolicy?.PolicyName ?? string.Empty, PrimaryTenantLocationId = entity.PrimaryTenantLocationId,
        PrimaryTenantLocationName = entity.PrimaryTenantLocation?.LocationName, WorkMode = entity.WorkMode, WorkModeName = entity.WorkMode.ToString(),
        HybridType = entity.HybridType, HybridTypeName = entity.HybridType?.ToString(), MinimumOfficeDaysPerWeek = entity.MinimumOfficeDaysPerWeek,
        MinimumOfficeDaysPerMonth = entity.MinimumOfficeDaysPerMonth, MaximumWFHDaysPerMonth = entity.MaximumWFHDaysPerMonth,
        EffectiveFrom = entity.EffectiveFrom, EffectiveTo = entity.EffectiveTo, IsActive = entity.IsActive
    };

    /// <summary>Maps an employee work-pattern day and optional location context.</summary>
    public static EmployeeWorkPatternResponseDTO ToResponse(EmployeeWorkPattern entity) => new()
    {
        Id = entity.Id, EmployeeWorkArrangementId = entity.EmployeeWorkArrangementId, DayOfWeek = entity.DayOfWeek, DayOfWeekName = entity.DayOfWeek.ToString(),
        WorkMode = entity.WorkMode, WorkModeName = entity.WorkMode.ToString(), TenantLocationId = entity.TenantLocationId,
        TenantLocationName = entity.TenantLocation?.LocationName, IsWorkingDay = entity.IsWorkingDay, IsActive = entity.IsActive
    };

    /// <summary>Maps a temporary work-mode override while leaving approval data read-only.</summary>
    public static EmployeeWorkModeOverrideResponseDTO ToResponse(EmployeeWorkModeOverrideRequest entity) => new()
    {
        Id = entity.Id, EmployeeId = entity.EmployeeId, EmployeeName = EmployeeName(entity.Employee), EmployeeWorkArrangementId = entity.EmployeeWorkArrangementId,
        RequestedWorkMode = entity.RequestedWorkMode, RequestedWorkModeName = entity.RequestedWorkMode.ToString(), FromDate = entity.FromDate, ToDate = entity.ToDate,
        TenantLocationId = entity.TenantLocationId, TenantLocationName = entity.TenantLocation?.LocationName, Reason = entity.Reason,
        ApprovalStatus = entity.ApprovalStatus, ApprovalStatusName = entity.ApprovalStatus.ToString(), ApprovalRemark = entity.ApprovalRemark,
        RejectionRemark = entity.RejectionRemark, IsActive = entity.IsActive
    };

    private static string EmployeeName(Employee? employee) =>
        string.Join(' ', new[] { employee?.FirstName, employee?.MiddleName, employee?.LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
