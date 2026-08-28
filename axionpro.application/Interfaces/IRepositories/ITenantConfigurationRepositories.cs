// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines tenant-isolated persistence contracts for TenantConfiguration resources.
// ================================================================

using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>Defines Tenant-location persistence and dependency checks.</summary>
public interface ITenantLocationRepository
{
    /// <summary>Gets a non-soft-deleted Tenant location by its trusted Tenant and identifier.</summary>
    Task<TenantLocation?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked non-soft-deleted Tenant location for a state change.</summary>
    Task<TenantLocation?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged Tenant-location result.</summary>
    Task<PagedResponseDTO<TenantLocation>> GetPagedAsync(long tenantId, TenantLocationFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged Host-visible Tenant-location result.</summary>
    Task<PagedResponseDTO<TenantLocation>> GetHostPagedAsync(TenantLocationFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the LocationCode is already live for the Tenant.</summary>
    Task<bool> LocationCodeExistsAsync(long tenantId, string locationCode, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Validates the active country, state, and city geographic hierarchy.</summary>
    Task<bool> IsValidGeographyAsync(int countryId, int? stateId, int? cityId, CancellationToken cancellationToken);
    /// <summary>Determines whether live active dependent configuration blocks deactivation.</summary>
    Task<bool> HasLiveActiveDependenciesAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Determines whether any live dependent configuration blocks soft deletion.</summary>
    Task<bool> HasAnyDependenciesAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared Tenant location.</summary>
    Task AddAsync(TenantLocation entity, CancellationToken cancellationToken);
}

/// <summary>Defines AttendancePolicy persistence and dependency checks.</summary>
public interface IAttendancePolicyRepository
{
    /// <summary>Gets a non-soft-deleted attendance policy scoped to a Tenant.</summary>
    Task<AttendancePolicy?> GetByIdAsync(long tenantId, int id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked attendance policy scoped to a Tenant.</summary>
    Task<AttendancePolicy?> GetForUpdateAsync(long tenantId, int id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged attendance-policy result.</summary>
    Task<PagedResponseDTO<AttendancePolicy>> GetPagedAsync(long tenantId, AttendancePolicyFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the policy name is already live for the Tenant.</summary>
    Task<bool> PolicyNameExistsAsync(long tenantId, string policyName, int? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the supplied PolicyType is active and owned by the Tenant.</summary>
    Task<bool> IsEligiblePolicyTypeAsync(long tenantId, int policyTypeId, CancellationToken cancellationToken);
    /// <summary>Determines whether an active work arrangement blocks deactivation.</summary>
    Task<bool> HasActiveWorkArrangementsAsync(long tenantId, int policyId, CancellationToken cancellationToken);
    /// <summary>Determines whether any live work arrangement blocks soft deletion.</summary>
    Task<bool> HasAnyWorkArrangementsAsync(long tenantId, int policyId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared attendance policy.</summary>
    Task AddAsync(AttendancePolicy entity, CancellationToken cancellationToken);
}

/// <summary>Defines employee-location-assignment persistence and validation checks.</summary>
public interface IEmployeeLocationAssignmentRepository
{
    /// <summary>Gets a non-soft-deleted employee-location assignment scoped to a Tenant.</summary>
    Task<EmployeeLocationAssignment?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked employee-location assignment scoped to a Tenant.</summary>
    Task<EmployeeLocationAssignment?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged employee-location-assignment result.</summary>
    Task<PagedResponseDTO<EmployeeLocationAssignment>> GetPagedAsync(long tenantId, EmployeeLocationAssignmentFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the Employee is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the location is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Determines whether a live assignment already uses the Employee/location pair.</summary>
    Task<bool> AssignmentExistsAsync(long tenantId, long employeeId, long locationId, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether another live primary assignment exists for the Employee.</summary>
    Task<bool> PrimaryAssignmentExistsAsync(long tenantId, long employeeId, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared employee-location assignment.</summary>
    Task AddAsync(EmployeeLocationAssignment entity, CancellationToken cancellationToken);
}

/// <summary>Defines employee-device-enrollment persistence and read-only TenantDevice validation.</summary>
public interface IEmployeeDeviceEnrollmentRepository
{
    /// <summary>Gets a non-soft-deleted employee-device enrollment scoped to a Tenant.</summary>
    Task<EmployeeDeviceEnrollment?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked employee-device enrollment scoped to a Tenant.</summary>
    Task<EmployeeDeviceEnrollment?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged employee-device-enrollment result.</summary>
    Task<PagedResponseDTO<EmployeeDeviceEnrollment>> GetPagedAsync(long tenantId, EmployeeDeviceEnrollmentFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the Employee is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken);
    /// <summary>Determines whether a Host-managed TenantDevice is eligible for enrollment.</summary>
    Task<bool> IsEligibleTenantDeviceAsync(long tenantId, long tenantDeviceId, CancellationToken cancellationToken);
    /// <summary>Determines whether the physical device already has the live enroll identifier.</summary>
    Task<bool> EnrollIdExistsAsync(long tenantId, long tenantDeviceId, string enrollId, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared employee-device enrollment.</summary>
    Task AddAsync(EmployeeDeviceEnrollment entity, CancellationToken cancellationToken);
}

/// <summary>Defines employee-work-arrangement persistence, validation, and lifecycle checks.</summary>
public interface IEmployeeWorkArrangementRepository
{
    /// <summary>Gets a non-soft-deleted work arrangement scoped to a Tenant.</summary>
    Task<EmployeeWorkArrangement?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked work arrangement scoped to a Tenant.</summary>
    Task<EmployeeWorkArrangement?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged employee-work-arrangement result.</summary>
    Task<PagedResponseDTO<EmployeeWorkArrangement>> GetPagedAsync(long tenantId, EmployeeWorkArrangementFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the Employee is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the AttendancePolicy is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleAttendancePolicyAsync(long tenantId, int policyId, CancellationToken cancellationToken);
    /// <summary>Determines whether the location is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Determines whether another live current arrangement exists for the Employee.</summary>
    Task<bool> CurrentArrangementExistsAsync(long tenantId, long employeeId, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether active children block deactivation.</summary>
    Task<bool> HasLiveActiveDependenciesAsync(long tenantId, long arrangementId, CancellationToken cancellationToken);
    /// <summary>Determines whether any live children block soft deletion.</summary>
    Task<bool> HasAnyDependenciesAsync(long tenantId, long arrangementId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared employee work arrangement.</summary>
    Task AddAsync(EmployeeWorkArrangement entity, CancellationToken cancellationToken);
}

/// <summary>Defines employee-work-pattern persistence and validation checks.</summary>
public interface IEmployeeWorkPatternRepository
{
    /// <summary>Gets a non-soft-deleted work-pattern day scoped to a Tenant.</summary>
    Task<EmployeeWorkPattern?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked work-pattern day scoped to a Tenant.</summary>
    Task<EmployeeWorkPattern?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged employee-work-pattern result.</summary>
    Task<PagedResponseDTO<EmployeeWorkPattern>> GetPagedAsync(long tenantId, EmployeeWorkPatternFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the work arrangement is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleArrangementAsync(long tenantId, long arrangementId, CancellationToken cancellationToken);
    /// <summary>Determines whether the location is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Determines whether a live work-pattern day already exists for the arrangement.</summary>
    Task<bool> PatternDayExistsAsync(long tenantId, long arrangementId, short dayOfWeek, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared work-pattern day.</summary>
    Task AddAsync(EmployeeWorkPattern entity, CancellationToken cancellationToken);
}

/// <summary>Defines employee work-mode-override persistence and validation checks.</summary>
public interface IEmployeeWorkModeOverrideRequestRepository
{
    /// <summary>Gets a non-soft-deleted override request scoped to a Tenant.</summary>
    Task<EmployeeWorkModeOverrideRequest?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked override request scoped to a Tenant.</summary>
    Task<EmployeeWorkModeOverrideRequest?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged work-mode-override result.</summary>
    Task<PagedResponseDTO<EmployeeWorkModeOverrideRequest>> GetPagedAsync(long tenantId, EmployeeWorkModeOverrideFilterRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether the Employee is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the optional work arrangement is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleArrangementAsync(long tenantId, long arrangementId, CancellationToken cancellationToken);
    /// <summary>Determines whether the optional location is active and owned by the Tenant.</summary>
    Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared work-mode override request.</summary>
    Task AddAsync(EmployeeWorkModeOverrideRequest entity, CancellationToken cancellationToken);
}
