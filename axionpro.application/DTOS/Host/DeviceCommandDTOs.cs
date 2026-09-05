// ================================================================
// Purpose : API contracts for MQTT device command submission. The payload is
//           vendor JSON and must not contain AxionPro tracking identifiers.
// ================================================================

namespace axionpro.application.DTOS.Host;

/// <summary>Submits one supported vendor command to a Tenant device.</summary>
public sealed class SubmitDeviceCommandRequestDTO : TenantDeviceAccessRequestDTO
{
    public long TenantDeviceId { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}

/// <summary>Returns server-side tracking information for a newly durable command.</summary>
public sealed class DeviceCommandSubmissionResponseDTO
{
    public long DeviceCommandId { get; set; }
    public Guid InternalTrackingId { get; set; }
    public string DeviceSerialNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
