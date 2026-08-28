// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides Host-managed PostgreSQL persistence for DeviceMaster and TenantDevice records.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

/// <summary>Provides shared paging behavior for Host device repositories.</summary>
public abstract class DeviceManagementRepositoryBase(WorkforceDbContext context)
{
    protected WorkforceDbContext Context { get; } = context;

    protected static (int PageNumber, int PageSize) NormalizePage(int pageNumber, int pageSize) =>
        (pageNumber > 0 ? pageNumber : 1, pageSize is > 0 and <= 100 ? pageSize : 10);

    protected static PagedResponseDTO<T> CreatePage<T>(List<T> data, int totalCount, int pageNumber, int pageSize) =>
        new(data, totalCount, pageNumber, pageSize) { TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) };
}

/// <summary>Provides DeviceMaster catalog persistence and physical-device dependency checks.</summary>
public sealed class DeviceMasterRepository(WorkforceDbContext context) : DeviceManagementRepositoryBase(context), IDeviceMasterRepository
{
    #region Device Master Queries

    /// <inheritdoc />
    public Task<DeviceMaster?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        Context.DeviceMasters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<DeviceMaster?> GetForUpdateAsync(long id, CancellationToken cancellationToken) =>
        Context.DeviceMasters.FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<DeviceMaster>> GetPagedAsync(GetDeviceMasterListRequestDTO filter, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = Context.DeviceMasters.AsNoTracking().Where(x => !x.IsSoftDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = $"%{filter.Search.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.DeviceCode, term) || EF.Functions.ILike(x.DeviceName, term) || EF.Functions.ILike(x.ModelNo, term) || EF.Functions.ILike(x.CompanyName, term) || (x.BrandName != null && EF.Functions.ILike(x.BrandName, term)) || (x.Description != null && EF.Functions.ILike(x.Description, term)));
        }
        if (filter.DeviceType.HasValue) query = query.Where(x => x.DeviceType == (short)filter.DeviceType.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<bool> DuplicateExistsAsync(string deviceCode, string companyName, string modelNo, long? excludeId, CancellationToken cancellationToken) =>
        Context.DeviceMasters.AnyAsync(x => !x.IsSoftDeleted && (!excludeId.HasValue || x.Id != excludeId.Value) && (x.DeviceCode.ToLower() == deviceCode.ToLower() || (x.CompanyName.ToLower() == companyName.ToLower() && x.ModelNo.ToLower() == modelNo.ToLower())), cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasActiveTenantDevicesAsync(long deviceMasterId, CancellationToken cancellationToken) =>
        Context.TenantDevices.AnyAsync(x => x.DeviceMasterId == deviceMasterId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasTenantDevicesAsync(long deviceMasterId, CancellationToken cancellationToken) =>
        Context.TenantDevices.AnyAsync(x => x.DeviceMasterId == deviceMasterId && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Device Master Commands

    /// <inheritdoc />
    public Task AddAsync(DeviceMaster entity, CancellationToken cancellationToken) => Context.DeviceMasters.AddAsync(entity, cancellationToken).AsTask();

    #endregion
}

/// <summary>Provides Host-managed physical TenantDevice persistence while retaining TenantConfiguration read compatibility.</summary>
public sealed class TenantDeviceRepository(WorkforceDbContext context) : DeviceManagementRepositoryBase(context), ITenantDeviceRepository
{
    #region Tenant Device Queries

    /// <inheritdoc />
    public Task<TenantDevice?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        DeviceQuery().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<TenantDevice?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken) =>
        Context.TenantDevices.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResponseDTO<TenantDevice>> GetPagedAsync(long tenantId, GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken)
    {
        var (pageNumber, pageSize) = NormalizePage(filter.PageNumber, filter.PageSize);
        var query = DeviceQuery().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = $"%{filter.Search.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.SerialNumber, term) || EF.Functions.ILike(x.DeviceCode, term) || (x.DeviceName != null && EF.Functions.ILike(x.DeviceName, term)) || (x.AssetTag != null && EF.Functions.ILike(x.AssetTag, term)) || EF.Functions.ILike(x.Tenant.CompanyName, term) || EF.Functions.ILike(x.TenantLocation.LocationName, term));
        }
        if (filter.TenantLocationId.HasValue) query = query.Where(x => x.TenantLocationId == filter.TenantLocationId.Value);
        if (filter.DeviceMasterId.HasValue) query = query.Where(x => x.DeviceMasterId == filter.DeviceMasterId.Value);
        if (filter.CommunicationType.HasValue) query = query.Where(x => x.CommunicationType == (short)filter.CommunicationType.Value);
        if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return CreatePage(data, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public Task<TenantDevice?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken) =>
        DeviceQuery().FirstOrDefaultAsync(x => !x.IsSoftDeleted && x.SerialNumber.ToLower() == serialNumber.Trim().ToLower(), cancellationToken);

    /// <inheritdoc />
    public Task<bool> IsEligibleTenantAsync(long tenantId, CancellationToken cancellationToken) =>
        Context.Tenants.AnyAsync(x => x.Id == tenantId && x.IsActive && x.IsSoftDeleted != true, cancellationToken);

    /// <inheritdoc />
    public Task<bool> IsEligibleTenantLocationAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken) =>
        Context.TenantLocations.AnyAsync(x => x.Id == tenantLocationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> IsActiveTenantLocationAsync(long tenantLocationId, CancellationToken cancellationToken) =>
        Context.TenantLocations.AnyAsync(x => x.Id == tenantLocationId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> TenantLocationBelongsToTenantAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken) =>
        Context.TenantLocations.AnyAsync(x => x.Id == tenantLocationId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> IsEligibleDeviceMasterAsync(long deviceMasterId, CancellationToken cancellationToken) =>
        Context.DeviceMasters.AnyAsync(x => x.Id == deviceMasterId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> SerialNumberExistsAsync(string serialNumber, long? excludeId, CancellationToken cancellationToken) =>
        Context.TenantDevices.AnyAsync(x => !x.IsSoftDeleted && x.SerialNumber.ToLower() == serialNumber.Trim().ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    /// <inheritdoc />
    public Task<bool> DeviceCodeExistsAsync(long tenantId, string deviceCode, long? excludeId, CancellationToken cancellationToken) =>
        Context.TenantDevices.AnyAsync(x => !x.IsSoftDeleted && x.TenantId == tenantId && x.DeviceCode.ToLower() == deviceCode.Trim().ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    /// <inheritdoc />
    public Task<bool> AssetTagExistsAsync(long tenantId, string? assetTag, long? excludeId, CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(assetTag) ? Task.FromResult(false) : Context.TenantDevices.AnyAsync(x => !x.IsSoftDeleted && x.TenantId == tenantId && x.AssetTag != null && x.AssetTag.ToLower() == assetTag.Trim().ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasActiveEnrollmentsAsync(long tenantDeviceId, CancellationToken cancellationToken) =>
        Context.EmployeeDeviceEnrollments.AnyAsync(x => x.TenantDeviceId == tenantDeviceId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasEnrollmentsAsync(long tenantDeviceId, CancellationToken cancellationToken) =>
        Context.EmployeeDeviceEnrollments.AnyAsync(x => x.TenantDeviceId == tenantDeviceId && !x.IsSoftDeleted, cancellationToken);

    #endregion

    #region Tenant Device Commands

    /// <inheritdoc />
    public Task AddAsync(TenantDevice entity, CancellationToken cancellationToken) => Context.TenantDevices.AddAsync(entity, cancellationToken).AsTask();

    #endregion

    private IQueryable<TenantDevice> DeviceQuery() => Context.TenantDevices.AsNoTracking().Include(x => x.Tenant).Include(x => x.TenantLocation).Include(x => x.DeviceMaster);
}
