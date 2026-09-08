// ================================================================
// Purpose : Tenant-facing, human-readable contracts for AiFace runtime
//           settings. These are intentionally typed; raw vendor JSON is never
//           accepted from Angular.
// ================================================================

namespace axionpro.application.DTOS.Host;

/// <summary>Identifies the Device Configuration section that is being queued.</summary>
public enum TenantDeviceSettingsSection
{
    Time,
    Bell,
    DeviceSetup,
    Advanced,
    Lock,
    Serial,
    Ethernet,
    Wifi,
    AppNotification,
    WebAccess,
    ScreenMenuPin
}

/// <summary>Common Tenant-admin fields for every device setting command.</summary>
public abstract class TenantDeviceSettingRequestDTO : TenantDeviceAccessRequestDTO
{
    /// <summary>Opaque Tenant device identifier returned by the Tenant device APIs.</summary>
    public string TenantDeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Current device Web UI/API password. It authorizes the vendor command,
    /// is encrypted at rest in the queue, and is never returned.
    /// </summary>
    public string CurrentWebServerPassword { get; set; } = string.Empty;
}

/// <summary>Updates time display, DST, NTP, and scheduled reboot settings.</summary>
public sealed class UpdateTenantDeviceTimeSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int TimeFormat { get; set; }
    public int DateFormat { get; set; }
    public bool DaylightSavingEnabled { get; set; }
    /// <summary>Month/day in M/d form, for example 3/21.</summary>
    public string DaylightSavingStart { get; set; } = string.Empty;
    /// <summary>Month/day in M/d form, for example 9/21.</summary>
    public string DaylightSavingEnd { get; set; } = string.Empty;
    public bool NetworkTimeEnabled { get; set; }
    public int TimeZone { get; set; }
    /// <summary>24-hour time in HH:mm form; 00:00 disables the scheduled reboot.</summary>
    public string RebootTime1 { get; set; } = "00:00";
    public string RebootTime2 { get; set; } = "00:00";
    public string RebootTime3 { get; set; } = "00:00";
}

/// <summary>Sets the physical device clock to a supplied UTC timestamp or the current server UTC time.</summary>
public sealed class SyncTenantDeviceTimeRequestDTO : TenantDeviceSettingRequestDTO
{
    /// <summary>Optional UTC device time. If omitted, AxionPro's current UTC time is used.</summary>
    public DateTime? UtcDateTime { get; set; }
}

/// <summary>Updates bell count, ring pattern, and bell output state.</summary>
public sealed class UpdateTenantDeviceBellSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int BellCount { get; set; }
    public int RingStyle { get; set; }
    public int BellOutput { get; set; }
}

/// <summary>Updates day-to-day display, wake-up, voice, and face-recognition settings.</summary>
public sealed class UpdateTenantDeviceSetupSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int Language { get; set; }
    public int VoiceVolume { get; set; }
    public bool AnnouncePersonName { get; set; }
    public bool DetectMultipleFaces { get; set; }
    public int ResultDisplaySeconds { get; set; }
    public int ScreenSaverIdleSeconds { get; set; }
    public int SleepModeSeconds { get; set; }
    public int ScreenWakeUpMethod { get; set; }
    public int FaceWakeUpSeconds { get; set; }
    public int ResultDisplayStyle { get; set; }
    public int FaceRecognitionDistance { get; set; }
    public bool LivenessDetectionEnabled { get; set; }
    public bool ShowAvatar { get; set; }
}

/// <summary>Updates advanced verification, privacy, fill-light, and face options.</summary>
public sealed class UpdateTenantDeviceAdvancedSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int MaximumAdministrators { get; set; }
    public int VerificationMode { get; set; }
    public int QrCodeMode { get; set; }
    public bool HidePrivacyInformation { get; set; }
    public int FaceMatchThreshold { get; set; }
    public int LivenessThreshold { get; set; }
    public int FingerprintMatchThreshold { get; set; }
    public int FingerprintsPerUser { get; set; }
    public bool MaskDetectionEnabled { get; set; }
    public int MaskThreshold { get; set; }
    public int FillLightMode { get; set; }
    /// <summary>Time range in HH:mm~HH:mm form, for example 08:00~18:00.</summary>
    public string ConstantFillLightPeriod { get; set; } = "00:00~00:00";
    public int ExposureCompensation { get; set; }
    public int PalmVeinMatchThreshold { get; set; }
    public int PalmDetectionThreshold { get; set; }
    public bool DisableFaceRecognition { get; set; }
    public bool OnlineDebugEnabled { get; set; }
}

/// <summary>Updates door, sensor, Wiegand, and access-control behaviour.</summary>
public sealed class UpdateTenantDeviceLockSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int DoorOpenDelaySeconds { get; set; }
    public int DoorSensorMode { get; set; }
    public int DoorSensorDelaySeconds { get; set; }
    public bool BlockStrangerAccess { get; set; }
    public int DoorPassword { get; set; }
    public int RequiredUsersForDoorOpen { get; set; }
    public int AntiPassbackMode { get; set; }
    public int WiegandOutput { get; set; }
    public int WiegandFormat { get; set; }
    public int AccessLimit { get; set; }
    public int CardDisplayFormat { get; set; }
    public bool ReverseCardPin { get; set; }
    public bool ReverseWiegandOutput { get; set; }
    public bool ExternalWiegandSnapshotEnabled { get; set; }
    public bool InterlockEnabled { get; set; }
    public bool AlarmProcessingEnabled { get; set; }
    public int FailedVerificationLimit { get; set; }
    public int TimeZonePunchLimit { get; set; }
    public bool SuppressAccessDeniedLog { get; set; }
    public bool DenyOutsideNormallyOpenTimeZone { get; set; }
}

/// <summary>Updates serial-port identity and function settings.</summary>
public sealed class UpdateTenantDeviceSerialSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public int DeviceAddress { get; set; }
    public int NetworkPort { get; set; }
    public int BaudRate { get; set; }
    public int SerialFunction { get; set; }
}

/// <summary>Updates the Ethernet interface. Static addressing is used only when DHCP is disabled.</summary>
public sealed class UpdateTenantDeviceEthernetSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public bool DhcpEnabled { get; set; }
    public string? IpAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? Gateway { get; set; }
    public string? DnsServer { get; set; }
    public bool HideIpAddress { get; set; }
}

/// <summary>Updates the Wi-Fi interface. Static addressing is used only when DHCP is disabled.</summary>
public sealed class UpdateTenantDeviceWifiSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public bool DhcpEnabled { get; set; }
    public string? IpAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? Gateway { get; set; }
}

/// <summary>Updates the device's optional third-party app notification behaviour.</summary>
public sealed class UpdateTenantDeviceAppNotificationSettingsRequestDTO : TenantDeviceSettingRequestDTO
{
    public bool AppNotificationEnabled { get; set; }
    /// <summary>Third-party app token. It is protected in the command queue and never returned.</summary>
    public string? AppToken { get; set; }
    public int NotificationType { get; set; }
}

/// <summary>Enables/disables the device local Web UI/API and optionally rotates its password.</summary>
public sealed class UpdateTenantDeviceWebAccessRequestDTO : TenantDeviceSettingRequestDTO
{
    public bool LocalWebServerEnabled { get; set; }
    /// <summary>Optional new local Web UI/API password. Never returned after submission.</summary>
    public string? NewWebServerPassword { get; set; }
}

/// <summary>Sets the PIN required by the device's physical System/Local Manager menu.</summary>
public sealed class UpdateTenantDeviceScreenMenuPinRequestDTO : TenantDeviceSettingRequestDTO
{
    /// <summary>New numeric PIN for the physical device System/Local Manager menu.</summary>
    public string ScreenMenuPin { get; set; } = string.Empty;
}

/// <summary>Moves an already assigned Tenant device to another location without replacing its DeviceMaster.</summary>
public sealed class UpdateTenantDeviceLocationRequestDTO : TenantDeviceAccessRequestDTO
{
    public string TenantDeviceId { get; set; } = string.Empty;
    public long TenantLocationId { get; set; }
}

/// <summary>Requests immediate broker delivery of the next already-queued MQTTS command.</summary>
public sealed class DispatchTenantDeviceMqttsNowRequestDTO : TenantDeviceAccessRequestDTO
{
    /// <summary>Opaque Tenant device identifier returned by the Tenant device APIs.</summary>
    public string TenantDeviceId { get; set; } = string.Empty;
}

/// <summary>Non-sensitive outcome of a manual MQTTS command dispatch.</summary>
public sealed class ManualTenantDeviceCommandDispatchResponseDTO
{
    public bool WasDispatched { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>Returns only non-secret gateway address metadata for a Tenant device.</summary>
public sealed class TenantDeviceGatewayAddressResponseDTO
{
    public string TenantDeviceId { get; set; } = string.Empty;
    public string ServerUrl { get; set; } = string.Empty;
    public string ServerPath { get; set; } = string.Empty;
    public int ServerPort { get; set; }
    public bool HasActiveGatewayUrl { get; set; }
    public bool IsReplacementPending { get; set; }
    public DateTime? ReplacementExpiresDateTime { get; set; }
}

/// <summary>Queues a seamless replacement of the opaque HTTPS device URL without changing heartbeat.</summary>
public sealed class ReplaceTenantDeviceHttpsGatewayUrlRequestDTO : TenantDeviceSettingRequestDTO
{
    /// <summary>Lifetime for the pending replacement address, in minutes.</summary>
    public int ReplacementLifetimeMinutes { get; set; } = 30;
}
