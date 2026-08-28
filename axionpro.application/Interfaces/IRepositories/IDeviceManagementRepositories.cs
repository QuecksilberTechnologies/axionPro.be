// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines Host-managed DeviceMaster and TenantDevice persistence contracts.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>Defines persistence and dependency queries for the global DeviceMaster catalog.</summary>
public interface IDeviceMasterRepository
{
    /// <summary>Gets a non-soft-deleted device model by identifier.</summary>
    Task<DeviceMaster?> GetByIdAsync(long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked non-soft-deleted device model for an administrative update.</summary>
    Task<DeviceMaster?> GetForUpdateAsync(long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged device model list.</summary>
    Task<PagedResponseDTO<DeviceMaster>> GetPagedAsync(GetDeviceMasterListRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Determines whether either live database business key is already used.</summary>
    Task<bool> DuplicateExistsAsync(string deviceCode, string companyName, string modelNo, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether a live physical device blocks catalog deactivation.</summary>
    Task<bool> HasActiveTenantDevicesAsync(long deviceMasterId, CancellationToken cancellationToken);
    /// <summary>Determines whether any non-soft-deleted physical device blocks catalog deletion.</summary>
    Task<bool> HasTenantDevicesAsync(long deviceMasterId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared Host-owned device model.</summary>
    Task AddAsync(DeviceMaster entity, CancellationToken cancellationToken);
}

/// <summary>Defines Host-managed physical TenantDevice persistence and validation queries.</summary>
public interface ITenantDeviceRepository
{
    /// <summary>Gets a non-soft-deleted physical device with display context scoped to its Tenant.</summary>
    Task<TenantDevice?> GetByIdAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a tracked non-soft-deleted physical device for a Tenant-scoped update.</summary>
    Task<TenantDevice?> GetForUpdateAsync(long tenantId, long id, CancellationToken cancellationToken);
    /// <summary>Gets a database-paged physical-device list scoped to its Tenant.</summary>
    Task<PagedResponseDTO<TenantDevice>> GetPagedAsync(long tenantId, GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken);
    /// <summary>Resolves an active or inactive non-soft-deleted physical device by manufacturer serial number.</summary>
    Task<TenantDevice?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
    /// <summary>Determines whether a Tenant is active and non-soft-deleted.</summary>
    Task<bool> IsEligibleTenantAsync(long tenantId, CancellationToken cancellationToken);
    /// <summary>Determines whether the active location belongs to the selected Tenant.</summary>
    Task<bool> IsEligibleTenantLocationAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken);
    /// <summary>Determines whether a location is active and non-soft-deleted regardless of Tenant ownership.</summary>
    Task<bool> IsActiveTenantLocationAsync(long tenantLocationId, CancellationToken cancellationToken);
    /// <summary>Determines whether a location belongs to the selected Tenant.</summary>
    Task<bool> TenantLocationBelongsToTenantAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken);
    /// <summary>Determines whether a device master is active and non-soft-deleted.</summary>
    Task<bool> IsEligibleDeviceMasterAsync(long deviceMasterId, CancellationToken cancellationToken);
    /// <summary>Determines whether the global serial number is used by another live physical device.</summary>
    Task<bool> SerialNumberExistsAsync(string serialNumber, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the device code is used by another live device of the selected Tenant.</summary>
    Task<bool> DeviceCodeExistsAsync(long tenantId, string deviceCode, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether the supplied asset tag is used by another live device of the selected Tenant.</summary>
    Task<bool> AssetTagExistsAsync(long tenantId, string? assetTag, long? excludeId, CancellationToken cancellationToken);
    /// <summary>Determines whether active employee enrollment blocks deactivation.</summary>
    Task<bool> HasActiveEnrollmentsAsync(long tenantDeviceId, CancellationToken cancellationToken);
    /// <summary>Determines whether any live employee enrollment blocks soft deletion.</summary>
    Task<bool> HasEnrollmentsAsync(long tenantDeviceId, CancellationToken cancellationToken);
    /// <summary>Adds a prepared Host-owned physical device.</summary>
    Task AddAsync(TenantDevice entity, CancellationToken cancellationToken);
}
