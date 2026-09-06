// ================================================================
// Purpose : Contract for secure Host bootstrap and Tenant-admin runtime device
//           configuration. Credentials are accepted only for immediate use and
//           are never returned by this API.
// ================================================================

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Host;

/// <summary>Host-only request to create a short-lived initial HTTPS device route.</summary>
public sealed class IssueInitialDeviceBootstrapRequestDTO : PermissionRequestDTO
{
    public long DeviceMasterId { get; set; }

    /// <summary>Validity period for the one-device bootstrap URL. Defaults to two hours.</summary>
    public int LifetimeMinutes { get; set; } = 120;
}

/// <summary>Returned exactly once so the Host operator can enter it on the physical device.</summary>
public sealed class InitialDeviceBootstrapResponseDTO
{
    public string DeviceSerialNumber { get; set; } = string.Empty;
    public string InitialGatewayUrl { get; set; } = string.Empty;
    public int HeartbeatIntervalSeconds { get; set; }
    public DateTime ExpiresDateTime { get; set; }
}

/// <summary>
/// Tenant-admin-only desired runtime settings. The current local WebServer password
/// is used only to create an encrypted command payload; it is not saved or returned.
/// </summary>
public sealed class ApplyTenantDeviceRuntimeConfigurationRequestDTO : TenantDeviceAccessRequestDTO
{
    public long TenantDeviceId { get; set; }
    public string CurrentWebServerPassword { get; set; } = string.Empty;
    public int HeartbeatIntervalSeconds { get; set; } = 20;
    public int? Volume { get; set; }
    public bool DisableLocalWebServer { get; set; } = true;
    public string? NewWebServerPassword { get; set; }
    public bool RebootAfterApply { get; set; } = true;
}

/// <summary>Tenant-admin-only request to queue a device reboot through the configured HTTPS gateway.</summary>
public sealed class RebootTenantDeviceRequestDTO : TenantDeviceAccessRequestDTO
{
    public long TenantDeviceId { get; set; }
}

/// <summary>Contains only server-side command tracking data, never a device secret or URL token.</summary>
public sealed class TenantDeviceRuntimeConfigurationResponseDTO
{
    public long ConfigurationCommandId { get; set; }
    public Guid ConfigurationTrackingId { get; set; }
    public long? RebootCommandId { get; set; }
    public Guid? RebootTrackingId { get; set; }
    public string Status { get; set; } = string.Empty;
}
