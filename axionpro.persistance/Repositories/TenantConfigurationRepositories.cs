// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides Tenant-isolated EF Core persistence for TenantConfiguration records.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

/// <summary>Provides shared database paging behavior for TenantConfiguration repositories.</summary>
public abstract class TenantConfigurationRepositoryBase
{
    protected TenantConfigurationRepositoryBase(WorkforceDbContext context) => Context = context;
    protected WorkforceDbContext Context { get; }

    protected static (int PageNumber, int PageSize) NormalizePage(int pageNumber, int pageSize) =>
        (pageNumber > 0 ? pageNumber : 1, pageSize > 0 ? pageSize : 10);

    protected static PagedResponseDTO<T> CreatePage<T>(List<T> data, int count, int pageNumber, int pageSize) =>
        new(data, count, pageNumber, pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
}

/// <summary>Provides Tenant-location persistence and dependency queries.</summary>
public sealed class TenantLocationRepository : TenantConfigurationRepositoryBase, ITenantLocationRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public TenantLocationRepository(WorkforceDbContext context) : base(context) { }

    #region Tenant Location Queries

    /// <inheritdoc />
    public Task<TenantLocation?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.TenantLocations.AsNoTracking().Include(x => x.Country).Include(x => x.City)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<TenantLocation?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.TenantLocations.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<TenantLocation>> GetPagedAsync(long tenantId, TenantLocationFilterRequestDTO filter, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.TenantLocations.AsNoTracking().Include(x => x.Country).Include(x => x.City)
            .Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = $"%{filter.Search.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.LocationCode, term) || EF.Functions.ILike(x.LocationName, term) || (x.Address != null && EF.Functions.ILike(x.Address, term))); }
        if (filter.CountryId.HasValue) query = query.Where(x => x.CountryId == filter.CountryId.Value);
        if (filter.StateId.HasValue) query = query.Where(x => x.StateId == filter.StateId.Value);
        if (filter.CityId.HasValue) query = query.Where(x => x.CityId == filter.CityId.Value);
        if (filter.LocationType.HasValue) query = query.Where(x => x.LocationType == (short)filter.LocationType.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.LocationName).ThenBy(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResponseDTO<TenantLocation>> GetHostPagedAsync(TenantLocationFilterRequestDTO filter, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.TenantLocations.AsNoTracking().Include(x => x.Country).Include(x => x.City)
            .Where(x => !x.IsSoftDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = $"%{filter.Search.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.LocationCode, term) || EF.Functions.ILike(x.LocationName, term) || (x.Address != null && EF.Functions.ILike(x.Address, term))); }
        if (filter.CountryId.HasValue) query = query.Where(x => x.CountryId == filter.CountryId.Value);
        if (filter.StateId.HasValue) query = query.Where(x => x.StateId == filter.StateId.Value);
        if (filter.CityId.HasValue) query = query.Where(x => x.CityId == filter.CityId.Value);
        if (filter.LocationType.HasValue) query = query.Where(x => x.LocationType == (short)filter.LocationType.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.LocationName).ThenBy(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> LocationCodeExistsAsync(long tenantId, string locationCode, long? excludeId, CancellationToken cancellationToken) =>
        Context.TenantLocations.AnyAsync(x => x.TenantId == tenantId && !x.IsSoftDeleted && x.LocationCode.ToLower() == locationCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsValidGeographyAsync(int countryId, int? stateId, int? cityId, CancellationToken cancellationToken)
    {
        var countryExists = await Context.Countries.AnyAsync(x => x.Id == countryId && x.IsActive == true, cancellationToken);
        if (!countryExists) return false;
        if (stateId.HasValue && !await Context.States.AnyAsync(x => x.Id == stateId && x.CountryId == countryId && x.IsActive == true, cancellationToken)) return false;
        return !cityId.HasValue || await Context.Cities.AnyAsync(x => x.Id == cityId && x.IsActive == true && (!stateId.HasValue || x.StateId == stateId.Value), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasLiveActiveDependenciesAsync(long tenantId, long locationId, CancellationToken cancellationToken) =>
        await Context.TenantDevices.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeLocationAssignments.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkArrangements.AnyAsync(x => x.TenantId == tenantId && x.PrimaryTenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkPatterns.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkModeOverrideRequests.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> HasAnyDependenciesAsync(long tenantId, long locationId, CancellationToken cancellationToken) =>
        await Context.TenantDevices.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeLocationAssignments.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkArrangements.AnyAsync(x => x.TenantId == tenantId && x.PrimaryTenantLocationId == locationId && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkPatterns.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkModeOverrideRequests.AnyAsync(x => x.TenantId == tenantId && x.TenantLocationId == locationId && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Tenant Location Commands

    /// <inheritdoc />
    public Task AddAsync(TenantLocation entity, CancellationToken cancellationToken) => Context.TenantLocations.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides AttendancePolicy persistence and lifecycle queries.</summary>
public sealed class AttendancePolicyRepository : TenantConfigurationRepositoryBase, IAttendancePolicyRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public AttendancePolicyRepository(WorkforceDbContext context) : base(context) { }

    #region Attendance Policy Queries

    /// <inheritdoc />
    public Task<AttendancePolicy?> GetByIdAsync(long tenantId, int id, CancellationToken cancellationToken) =>
        Context.AttendancePolicies.AsNoTracking().Include(x => x.PolicyType).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<AttendancePolicy?> GetForUpdateAsync(long tenantId, int id, CancellationToken cancellationToken) =>
        Context.AttendancePolicies.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<AttendancePolicy>> GetPagedAsync(long tenantId, AttendancePolicyFilterRequestDTO filter, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.AttendancePolicies.AsNoTracking().Include(x => x.PolicyType).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = $"%{filter.Search.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.PolicyName, term)); }
        if (filter.PolicyTypeId.HasValue) query = query.Where(x => x.PolicyTypeId == filter.PolicyTypeId.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.PolicyName).ThenBy(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> PolicyNameExistsAsync(long tenantId, string policyName, int? excludeId, CancellationToken cancellationToken) =>
        Context.AttendancePolicies.AnyAsync(x => x.TenantId == tenantId && !x.IsSoftDeleted && x.PolicyName.ToLower() == policyName.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    /// <inheritdoc />
    public Task<bool> IsEligiblePolicyTypeAsync(long tenantId, int policyTypeId, CancellationToken cancellationToken) =>
        Context.PolicyTypes.AnyAsync(x => x.Id == policyTypeId && x.TenantId == tenantId && x.IsActive == true && x.IsSoftDelete != true, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasActiveWorkArrangementsAsync(long tenantId, int policyId, CancellationToken cancellationToken) =>
        Context.EmployeeWorkArrangements.AnyAsync(x => x.TenantId == tenantId && x.AttendancePolicyId == policyId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasAnyWorkArrangementsAsync(long tenantId, int policyId, CancellationToken cancellationToken) =>
        Context.EmployeeWorkArrangements.AnyAsync(x => x.TenantId == tenantId && x.AttendancePolicyId == policyId && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Attendance Policy Commands

    /// <inheritdoc />
    public Task AddAsync(AttendancePolicy entity, CancellationToken cancellationToken) => Context.AttendancePolicies.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides employee-location-assignment persistence and validation queries.</summary>
public sealed class EmployeeLocationAssignmentRepository : TenantConfigurationRepositoryBase, IEmployeeLocationAssignmentRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public EmployeeLocationAssignmentRepository(WorkforceDbContext context) : base(context) { }

    #region Employee Location Assignment Queries

    /// <inheritdoc />
    public Task<EmployeeLocationAssignment?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeLocationAssignments.AsNoTracking().Include(x => x.Employee).Include(x => x.TenantLocation).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<EmployeeLocationAssignment?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeLocationAssignments.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<EmployeeLocationAssignment>> GetPagedAsync(
        long tenantId,
        EmployeeLocationAssignmentFilterRequestDTO filter,
        long requestingEmployeeId,
        int requestingRoleTypeId,
        CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.EmployeeLocationAssignments.AsNoTracking().Include(x => x.Employee).Include(x => x.TenantLocation).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);

        // A Manager receives only their own and actively mapped direct reports' locations.
        // Employee and unknown role types fail closed to their own location assignments.
        if (requestingRoleTypeId == ConstantValues.RoleTypeManager)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(assignment =>
                assignment.EmployeeId == requestingEmployeeId ||
                Context.EmployeeManagerMappings.Any(mapping =>
                    mapping.TenantId == tenantId &&
                    mapping.ManagerId == requestingEmployeeId &&
                    mapping.EmployeeId == assignment.EmployeeId &&
                    mapping.IsActive &&
                    mapping.IsSoftDeleted != true &&
                    mapping.EffectiveFrom <= today &&
                    (!mapping.EffectiveTo.HasValue || mapping.EffectiveTo.Value >= today)));
        }
        else if (requestingRoleTypeId != ConstantValues.RoleTypeAdmin)
        {
            query = query.Where(assignment => assignment.EmployeeId == requestingEmployeeId);
        }

        if (filter.ResolvedEmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.ResolvedEmployeeId.Value);
        if (filter.TenantLocationId.HasValue) query = query.Where(x => x.TenantLocationId == filter.TenantLocationId.Value);
        if (filter.IsPrimary.HasValue) query = query.Where(x => x.IsPrimary == filter.IsPrimary.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.IsPrimary).ThenBy(x => x.EmployeeId).ThenByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken) => Context.Employees.AnyAsync(x => x.Id == employeeId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken) => Context.TenantLocations.AnyAsync(x => x.Id == locationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> AssignmentExistsAsync(long tenantId, long employeeId, long locationId, long? excludeId, CancellationToken cancellationToken) => Context.EmployeeLocationAssignments.AnyAsync(x => x.TenantId == tenantId && x.EmployeeId == employeeId && x.TenantLocationId == locationId && x.IsActive && !x.IsSoftDeleted && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    /// <inheritdoc />
    public Task<bool> PrimaryAssignmentExistsAsync(long tenantId, long employeeId, long? excludeId, CancellationToken cancellationToken) => Context.EmployeeLocationAssignments.AnyAsync(x => x.TenantId == tenantId && x.EmployeeId == employeeId && x.IsPrimary && x.IsActive && !x.IsSoftDeleted && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    #endregion

    #region Employee Location Assignment Commands

    /// <inheritdoc />
    public Task AddAsync(EmployeeLocationAssignment entity, CancellationToken cancellationToken) => Context.EmployeeLocationAssignments.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides employee-device-enrollment persistence and read-only TenantDevice validation.</summary>
public sealed class EmployeeDeviceEnrollmentRepository : TenantConfigurationRepositoryBase, IEmployeeDeviceEnrollmentRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public EmployeeDeviceEnrollmentRepository(WorkforceDbContext context) : base(context) { }

    #region Employee Device Enrollment Queries

    /// <inheritdoc />
    public Task<EmployeeDeviceEnrollment?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeDeviceEnrollments.AsNoTracking().Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<EmployeeDeviceEnrollment?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeDeviceEnrollments.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<EmployeeDeviceEnrollment>> GetPagedAsync(long tenantId, EmployeeDeviceEnrollmentFilterRequestDTO filter, long requestingEmployeeId, int requestingRoleTypeId, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.EmployeeDeviceEnrollments.AsNoTracking().Include(x => x.Employee).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (requestingRoleTypeId != ConstantValues.RoleTypeAdmin) query = query.Where(x => x.EmployeeId == requestingEmployeeId);
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = $"%{filter.Search.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.EnrollId, term) || (x.CardNumber != null && EF.Functions.ILike(x.CardNumber, term)) || Context.TenantDevices.Any(d => d.Id == x.TenantDeviceId && (EF.Functions.ILike(d.DeviceMaster.SNo, term) || EF.Functions.ILike(d.DeviceCode, term)))); }
        if (filter.ResolvedEmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.ResolvedEmployeeId.Value);
        if (filter.TenantDeviceId.HasValue) query = query.Where(x => x.TenantDeviceId == filter.TenantDeviceId.Value);
        if (filter.TenantLocationId.HasValue) query = query.Where(x => Context.TenantDevices.Any(d => d.Id == x.TenantDeviceId && d.TenantLocationId == filter.TenantLocationId.Value));
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken) => Context.Employees.AnyAsync(x => x.Id == employeeId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleTenantDeviceAsync(long tenantId, long tenantDeviceId, CancellationToken cancellationToken) => Context.TenantDevices.AnyAsync(x => x.Id == tenantDeviceId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted && x.TenantDeviceConfiguration != null && x.TenantDeviceConfiguration.IsEnrollmentEnabled, cancellationToken);
    /// <inheritdoc />
    public Task<bool> EnrollIdExistsAsync(long tenantId, long tenantDeviceId, string enrollId, long? excludeId, CancellationToken cancellationToken) => Context.EmployeeDeviceEnrollments.AnyAsync(x => x.TenantId == tenantId && x.TenantDeviceId == tenantDeviceId && !x.IsSoftDeleted && x.EnrollId.ToLower() == enrollId.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    #endregion

    #region Employee Device Enrollment Commands

    /// <inheritdoc />
    public Task AddAsync(EmployeeDeviceEnrollment entity, CancellationToken cancellationToken) => Context.EmployeeDeviceEnrollments.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides employee-work-arrangement persistence and lifecycle queries.</summary>
public sealed class EmployeeWorkArrangementRepository : TenantConfigurationRepositoryBase, IEmployeeWorkArrangementRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public EmployeeWorkArrangementRepository(WorkforceDbContext context) : base(context) { }

    #region Employee Work Arrangement Queries

    /// <inheritdoc />
    public Task<EmployeeWorkArrangement?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkArrangements.AsNoTracking().Include(x => x.Employee).Include(x => x.AttendancePolicy).Include(x => x.PrimaryTenantLocation).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<EmployeeWorkArrangement?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkArrangements.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<EmployeeWorkArrangement>> GetPagedAsync(long tenantId, EmployeeWorkArrangementFilterRequestDTO filter, long requestingEmployeeId, int requestingRoleTypeId, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.EmployeeWorkArrangements.AsNoTracking().Include(x => x.Employee).Include(x => x.AttendancePolicy).Include(x => x.PrimaryTenantLocation).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (requestingRoleTypeId != ConstantValues.RoleTypeAdmin) query = query.Where(x => x.EmployeeId == requestingEmployeeId);
        if (filter.ResolvedEmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.ResolvedEmployeeId.Value);
        if (filter.AttendancePolicyId.HasValue) query = query.Where(x => x.AttendancePolicyId == filter.AttendancePolicyId.Value);
        if (filter.PrimaryTenantLocationId.HasValue) query = query.Where(x => x.PrimaryTenantLocationId == filter.PrimaryTenantLocationId.Value);
        if (filter.WorkMode.HasValue) query = query.Where(x => x.WorkMode == (short)filter.WorkMode.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.EffectiveFrom).ThenByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken) => Context.Employees.AnyAsync(x => x.Id == employeeId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleAttendancePolicyAsync(long tenantId, int policyId, CancellationToken cancellationToken) => Context.AttendancePolicies.AnyAsync(x => x.Id == policyId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken) => Context.TenantLocations.AnyAsync(x => x.Id == locationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> CurrentArrangementExistsAsync(long tenantId, long employeeId, long? excludeId, CancellationToken cancellationToken) => Context.EmployeeWorkArrangements.AnyAsync(x => x.TenantId == tenantId && x.EmployeeId == employeeId && x.IsActive && !x.IsSoftDeleted && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    /// <inheritdoc />
    public async Task<bool> HasLiveActiveDependenciesAsync(long tenantId, long arrangementId, CancellationToken cancellationToken) =>
        await Context.EmployeeWorkPatterns.AnyAsync(x => x.TenantId == tenantId && x.EmployeeWorkArrangementId == arrangementId && x.IsActive && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkModeOverrideRequests.AnyAsync(x => x.TenantId == tenantId && x.EmployeeWorkArrangementId == arrangementId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public async Task<bool> HasAnyDependenciesAsync(long tenantId, long arrangementId, CancellationToken cancellationToken) =>
        await Context.EmployeeWorkPatterns.AnyAsync(x => x.TenantId == tenantId && x.EmployeeWorkArrangementId == arrangementId && !x.IsSoftDeleted, cancellationToken)
        || await Context.EmployeeWorkModeOverrideRequests.AnyAsync(x => x.TenantId == tenantId && x.EmployeeWorkArrangementId == arrangementId && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Employee Work Arrangement Commands

    /// <inheritdoc />
    public Task AddAsync(EmployeeWorkArrangement entity, CancellationToken cancellationToken) => Context.EmployeeWorkArrangements.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides employee-work-pattern persistence and validation queries.</summary>
public sealed class EmployeeWorkPatternRepository : TenantConfigurationRepositoryBase, IEmployeeWorkPatternRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public EmployeeWorkPatternRepository(WorkforceDbContext context) : base(context) { }

    #region Employee Work Pattern Queries

    /// <inheritdoc />
    public Task<EmployeeWorkPattern?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkPatterns.AsNoTracking().Include(x => x.EmployeeWorkArrangement).Include(x => x.TenantLocation).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<EmployeeWorkPattern?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkPatterns.Include(x => x.EmployeeWorkArrangement).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<EmployeeWorkPattern>> GetPagedAsync(long tenantId, EmployeeWorkPatternFilterRequestDTO filter, long requestingEmployeeId, int requestingRoleTypeId, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.EmployeeWorkPatterns.AsNoTracking().Include(x => x.EmployeeWorkArrangement).Include(x => x.TenantLocation).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (requestingRoleTypeId != ConstantValues.RoleTypeAdmin) query = query.Where(x => x.EmployeeWorkArrangement.EmployeeId == requestingEmployeeId);
        if (filter.EmployeeWorkArrangementId.HasValue) query = query.Where(x => x.EmployeeWorkArrangementId == filter.EmployeeWorkArrangementId.Value);
        if (filter.DayOfWeek.HasValue) query = query.Where(x => x.DayOfWeek == (short)filter.DayOfWeek.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.DayOfWeek).ThenBy(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> IsEligibleArrangementAsync(long tenantId, long arrangementId, CancellationToken cancellationToken) => Context.EmployeeWorkArrangements.AnyAsync(x => x.Id == arrangementId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken) => Context.TenantLocations.AnyAsync(x => x.Id == locationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> PatternDayExistsAsync(long tenantId, long arrangementId, short dayOfWeek, long? excludeId, CancellationToken cancellationToken) => Context.EmployeeWorkPatterns.AnyAsync(x => x.TenantId == tenantId && x.EmployeeWorkArrangementId == arrangementId && x.DayOfWeek == dayOfWeek && x.IsActive && !x.IsSoftDeleted && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    #endregion

    #region Employee Work Pattern Commands

    /// <inheritdoc />
    public Task AddAsync(EmployeeWorkPattern entity, CancellationToken cancellationToken) => Context.EmployeeWorkPatterns.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides employee work-mode-override persistence and validation queries.</summary>
public sealed class EmployeeWorkModeOverrideRequestRepository : TenantConfigurationRepositoryBase, IEmployeeWorkModeOverrideRequestRepository
{
    /// <summary>Initializes a new repository over the shared workforce context.</summary>
    public EmployeeWorkModeOverrideRequestRepository(WorkforceDbContext context) : base(context) { }

    #region Employee Work Mode Override Queries

    /// <inheritdoc />
    public Task<EmployeeWorkModeOverrideRequest?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkModeOverrideRequests.AsNoTracking().Include(x => x.Employee).Include(x => x.TenantLocation).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<EmployeeWorkModeOverrideRequest?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.EmployeeWorkModeOverrideRequests.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<EmployeeWorkModeOverrideRequest>> GetPagedAsync(long tenantId, EmployeeWorkModeOverrideFilterRequestDTO filter, long requestingEmployeeId, int requestingRoleTypeId, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.EmployeeWorkModeOverrideRequests.AsNoTracking().Include(x => x.Employee).Include(x => x.TenantLocation).Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (requestingRoleTypeId != ConstantValues.RoleTypeAdmin) query = query.Where(x => x.EmployeeId == requestingEmployeeId);
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = $"%{filter.Search.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.Reason, term)); }
        if (filter.ResolvedEmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.ResolvedEmployeeId.Value);
        if (filter.RequestedWorkMode.HasValue) query = query.Where(x => x.RequestedWorkMode == (short)filter.RequestedWorkMode.Value);
        if (filter.ApprovalStatus.HasValue) query = query.Where(x => x.ApprovalStatus == (short)filter.ApprovalStatus.Value);
        if (filter.FromDate.HasValue) query = query.Where(x => x.FromDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.ToDate <= filter.ToDate.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.FromDate).ThenByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, count, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> IsEligibleEmployeeAsync(long tenantId, long employeeId, CancellationToken cancellationToken) => Context.Employees.AnyAsync(x => x.Id == employeeId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleArrangementAsync(long tenantId, long arrangementId, CancellationToken cancellationToken) => Context.EmployeeWorkArrangements.AnyAsync(x => x.Id == arrangementId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);
    /// <inheritdoc />
    public Task<bool> IsEligibleLocationAsync(long tenantId, long locationId, CancellationToken cancellationToken) => Context.TenantLocations.AnyAsync(x => x.Id == locationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Employee Work Mode Override Commands

    /// <inheritdoc />
    public Task AddAsync(EmployeeWorkModeOverrideRequest entity, CancellationToken cancellationToken) => Context.EmployeeWorkModeOverrideRequests.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}
