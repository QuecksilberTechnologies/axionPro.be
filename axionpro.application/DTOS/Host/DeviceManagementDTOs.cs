// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines Host-managed DeviceMaster and TenantDevice API contracts.
// ================================================================

using axionpro.application.DTOs.BaseDTO;
using axionpro.domain.Entity;

namespace axionpro.application.DTOS.Host;

/// <summary>Supplies editable model catalog values for a DeviceMaster record.</summary>
public class DeviceMasterRequestDTO
{
    #region Properties
    /// <summary>Gets or sets the manufacturer serial number for the device model.</summary>
    public string SNo { get; set; } = string.Empty;
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string ModelNo { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? BrandName { get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? ManufacturerWebsite { get; set; }
    public string? SupportEmail { get; set; }
    public string? SupportContactNumber { get; set; }
    public DeviceType DeviceType { get; set; }
    public bool IsAttendanceDevice { get; set; } = true;
    public bool IsAccessControlDevice { get; set; }
    public bool SupportsFace { get; set; }
    public bool SupportsFingerprint { get; set; }
    public bool SupportsCard { get; set; }
    public bool SupportsPin { get; set; }
    public bool SupportsQrCode { get; set; }
    public bool SupportsTemperature { get; set; }
    public bool SupportsHttp { get; set; }
    public bool SupportsHttps { get; set; }
    public bool SupportsWebSocket { get; set; }
    public bool SupportsTcpIp { get; set; }
    public bool SupportsCloudApi { get; set; }
    public bool SupportsPushSdk { get; set; }
    public bool SupportsMqtt { get; set; }
    public bool SupportsMqtts { get; set; }
    public bool SupportsWifi { get; set; }
    public bool SupportsEthernet { get; set; }
    public bool SupportsUsb { get; set; }
    public string? SupportedTlsVersions { get; set; }
    public string? SupportedProtocols { get; set; }
    public int? MaxUserCapacity { get; set; }
    public int? MaxFaceCapacity { get; set; }
    public int? MaxFingerprintCapacity { get; set; }
    public int? MaxCardCapacity { get; set; }
    public int? MaxAttendanceRecordCapacity { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? SoftwareVersion { get; set; }
    public string? OperatingSystem { get; set; }
    public string? ProcessorInfo { get; set; }
    public string? MemoryInfo { get; set; }
    public string? StorageInfo { get; set; }
    public string? DisplayInfo { get; set; }
    public string? CameraInfo { get; set; }
    public string? SensorInfo { get; set; }
    public string? PowerRequirement { get; set; }
    public string? OperatingTemperature { get; set; }
    public string? Dimensions { get; set; }
    public string? Weight { get; set; }
    public string? Features { get; set; }
    public string? TechnicalSpecifications { get; set; }
    public decimal? Price { get; set; }
    public string? CurrencyKey { get; set; }
    public int? WarrantyPeriodMonths { get; set; }
    public string? WarrantyDescription { get; set; }
    public bool IsIntegrationSupported { get; set; }
    public string? IntegrationType { get; set; }
    public string? ApiDocumentationUrl { get; set; }
    public string? SdkDocumentationUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? BrochureUrl { get; set; }
    public string? ManualUrl { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Remark { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion
}

/// <summary>Creates a Host-managed device model.</summary>
public sealed class CreateDeviceMasterRequestDTO : DeviceMasterRequestDTO { }

/// <summary>Updates a Host-managed device model.</summary>
public sealed class UpdateDeviceMasterRequestDTO : DeviceMasterRequestDTO
{
    /// <summary>Gets or sets the DeviceMaster identifier.</summary>
    public long Id { get; set; }
    /// <summary>Gets or sets the occupied state before the device is registered with a Tenant.</summary>
    public bool IsOccupied { get; set; }
}

/// <summary>Changes the active state of a Host-managed device model.</summary>
public sealed class UpdateDeviceMasterStatusRequestDTO
{
    /// <summary>Gets or sets the DeviceMaster identifier.</summary>
    public long Id { get; set; }
    /// <summary>Gets or sets the requested active state.</summary>
    public bool IsActive { get; set; }
}

/// <summary>Supplies server-side database paging and filtering for DeviceMaster records.</summary>
public sealed class GetDeviceMasterListRequestDTO
{
    /// <summary>Gets or sets a search term.</summary>
    public string? Search { get; set; }
    /// <summary>Gets or sets an optional functional device type filter.</summary>
    public DeviceType? DeviceType { get; set; }
    /// <summary>Gets or sets an optional active-state filter.</summary>
    public bool? IsActive { get; set; }
    /// <summary>Gets or sets an optional occupied-state filter.</summary>
    public bool? IsOccupied { get; set; }
    /// <summary>Gets or sets the requested page number.</summary>
    public int PageNumber { get; set; } = 1;
    /// <summary>Gets or sets the requested page size.</summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>Represents a Host-managed device model returned by the API.</summary>
public sealed class DeviceMasterResponseDTO : DeviceMasterRequestDTO
{
    /// <summary>Gets or sets the DeviceMaster identifier.</summary>
    public long Id { get; set; }
    /// <summary>Gets or sets whether this device is already registered with a Tenant.</summary>
    public bool IsOccupied { get; set; }
    /// <summary>Gets or sets the display name of the functional device type.</summary>
    public string DeviceTypeName { get; set; } = string.Empty;
    /// <summary>Gets or sets the creation timestamp.</summary>
    public DateTime AddedDateTime { get; set; }
    /// <summary>Gets or sets the most recent update timestamp.</summary>
    public DateTime? UpdatedDateTime { get; set; }
}

/// <summary>
/// Carries the encrypted Host-selected Tenant identifier and permission metadata
/// required by TenantDevice endpoints.
/// </summary>
public class TenantDeviceAccessRequestDTO : PermissionRequestDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier selected by a Host user.
    /// Tenant Employee requests derive their Tenant scope from the trusted token.
    /// </summary>
    public string? TenantId { get; set; }
}

/// <summary>Supplies editable installation values for a physical Tenant device.</summary>
public class TenantDeviceRequestDTO : TenantDeviceAccessRequestDTO
{
    #region Properties
    public long TenantLocationId { get; set; }
    public long DeviceMasterId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public DateTime? InstalledDateTime { get; set; }
    public long? InstalledBy { get; set; }
    public string? InstallationRemark { get; set; }
    public bool IsAttendanceDevice { get; set; } = true;
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion
}

/// <summary>Creates a Host-managed physical Tenant device.</summary>
public sealed class CreateTenantDeviceRequestDTO : TenantDeviceRequestDTO { }

/// <summary>Updates a Host-managed physical Tenant device without altering runtime telemetry.</summary>
public sealed class UpdateTenantDeviceRequestDTO : TenantDeviceRequestDTO
{
    /// <summary>Gets or sets the TenantDevice identifier.</summary>
    public long Id { get; set; }
}

/// <summary>Changes the active state of a physical Tenant device.</summary>
public sealed class UpdateTenantDeviceStatusRequestDTO : TenantDeviceAccessRequestDTO
{
    /// <summary>Gets or sets the TenantDevice identifier.</summary>
    public long Id { get; set; }
    /// <summary>Gets or sets the requested active state.</summary>
    public bool IsActive { get; set; }
}

/// <summary>Supplies database paging and filtering for physical Tenant-device management.</summary>
public sealed class GetTenantDeviceListRequestDTO : TenantDeviceAccessRequestDTO
{
    public string? Search { get; set; }
    public long? TenantLocationId { get; set; }
    public long? DeviceMasterId { get; set; }
    public bool? IsAttendanceDevice { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Represents a physical Tenant device returned by the API without exposing a raw Tenant identifier.</summary>
public sealed class TenantDeviceResponseDTO
{
    /// <summary>Gets or sets the encrypted Tenant identifier.</summary>
    public string TenantId { get; set; } = string.Empty;
    public long TenantLocationId { get; set; }
    public long DeviceMasterId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public DateTime? InstalledDateTime { get; set; }
    public long? InstalledBy { get; set; }
    public string? InstallationRemark { get; set; }
    public bool IsAttendanceDevice { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
    public long Id { get; set; }
    public string? TenantName { get; set; }
    public string? TenantLocationName { get; set; }
    public string? LocationCode { get; set; }
    public string? DeviceMasterName { get; set; }
    public string? DeviceMasterModelNo { get; set; }
    public bool HasConfiguration { get; set; }
    public DateTime AddedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}

/// <summary>Supplies editable connection settings for a Tenant device.</summary>
public class TenantDeviceConfigurationRequestDTO : TenantDeviceAccessRequestDTO
{
    public long TenantDeviceId { get; set; }
    public string? IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public int? DevicePort { get; set; }
    /// <summary>MQTT/MQTTS transport selected for this V1 device configuration.</summary>
    public DeviceCommunicationProtocol? MqttTransport { get; set; }
    public string? ServerHost { get; set; }
    public int? ServerPort { get; set; }
    public string? ServerPath { get; set; }
    public string? ServerUrl { get; set; }
    public string? PushMode { get; set; }
    public int? HeartbeatIntervalSeconds { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Configuration { get; set; }
    public bool IsEnrollmentEnabled { get; set; } = true;
    public bool IsAttendancePushEnabled { get; set; } = true;
    public bool IsAutoSyncEnabled { get; set; } = true;
}

/// <summary>Creates one connection configuration for a Tenant device.</summary>
public sealed class CreateTenantDeviceConfigurationRequestDTO : TenantDeviceConfigurationRequestDTO { }

/// <summary>Updates one connection configuration for a Tenant device.</summary>
public sealed class UpdateTenantDeviceConfigurationRequestDTO : TenantDeviceConfigurationRequestDTO
{
    public long Id { get; set; }
}

/// <summary>Supplies paging and optional filters for Tenant device configurations.</summary>
public sealed class GetTenantDeviceConfigurationListRequestDTO : TenantDeviceAccessRequestDTO
{
    public string? Search { get; set; }
    public long? TenantDeviceId { get; set; }
    public DeviceCommunicationProtocol? MqttTransport { get; set; }
    public bool? IsEnrollmentEnabled { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Represents a Tenant device connection configuration without exposing a raw Tenant identifier.</summary>
public sealed class TenantDeviceConfigurationResponseDTO
{
    public string TenantId { get; set; } = string.Empty;
    public long Id { get; set; }
    public long TenantDeviceId { get; set; }
    public string? IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public int? DevicePort { get; set; }
    public DeviceCommunicationProtocol? MqttTransport { get; set; }
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
    public string? DeviceCode { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceMasterName { get; set; }
    public string? DeviceMasterSNo { get; set; }
    public DateTime AddedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
