// ================================================================
// Purpose : Application contracts for the durable device-command pipeline.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IDeviceCommunication;

/// <summary>Contains the caller-owned data required to submit a device command.</summary>
public sealed record DeviceCommandSubmission(
    long TenantId,
    long TenantDeviceId,
    string CommandName,
    string Payload,
    long? RequestedById,
    int MaxAttempts = 3);

/// <summary>Returns only AxionPro internal tracking data; it is not sent to a device.</summary>
public sealed record DeviceCommandSubmissionResult(
    long DeviceCommandId,
    Guid InternalTrackingId,
    string DeviceSerialNumber,
    DeviceCommandStatus Status);

/// <summary>Represents a command acquired by the central dispatcher.</summary>
public sealed record DeviceCommandDispatch(
    long DeviceCommandId,
    long TenantId,
    long TenantDeviceId,
    string DeviceSerialNumber,
    string CommandName,
    string Payload,
    DeviceCommandResponseMode ResponseMode,
    int AttemptCount);

/// <summary>Represents one raw inbound MQTT publication from a device.</summary>
public sealed record DeviceMqttInboundMessage(
    string Topic,
    string DeviceSerialNumber,
    string Payload,
    int QualityOfService,
    bool IsDuplicateDelivery,
    bool IsProtocolIdentityValid,
    DateTime ReceivedDateTime);

/// <summary>
/// Carries a raw device-initiated HTTPS poll. The bearer token lives only in the
/// device URL; the payload serial is still verified against its server mapping.
/// </summary>
public sealed record DeviceHttpsPollingRequest(
    string IngressToken,
    string Payload,
    DateTime ReceivedDateTime);

/// <summary>Represents the raw JSON response returned to a validated HTTPS device poll.</summary>
public sealed record DeviceHttpsPollingResponse(bool IsAccepted, string ResponsePayload)
{
    /// <summary>Returns the deliberately non-descriptive result used for rejected device traffic.</summary>
    public static DeviceHttpsPollingResponse Rejected { get; } = new(false, string.Empty);
}

/// <summary>Submits a validated command into the durable per-device queue.</summary>
public interface IDeviceCommandSubmissionService
{
    Task<DeviceCommandSubmissionResult> SubmitAsync(
        DeviceCommandSubmission submission,
        CancellationToken cancellationToken = default);
}

/// <summary>Coordinates generic transport queue acquisition, audit, retry, and protocol-aware response handling.</summary>
public interface IDeviceCommandDispatchStore
{
    /// <summary>
    /// Acquires one command only when its configured transport belongs to the
    /// supplied adapter set. This prevents one transport worker from dispatching
    /// another transport's command.
    /// </summary>
    Task<DeviceCommandDispatch?> TryAcquireNextAsync(
        IReadOnlyCollection<DeviceCommunicationProtocol> transports,
        long? tenantDeviceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Moves expired response-waiting commands to their next durable retry or final failure state.</summary>
    Task RecoverExpiredResponseDeadlinesAsync(CancellationToken cancellationToken = default);

    Task MarkPublishedAsync(
        DeviceCommandDispatch dispatch,
        string topic,
        int qualityOfService,
        DateTime publishedDateTime,
        CancellationToken cancellationToken = default);

    Task ScheduleRetryOrFailAsync(
        DeviceCommandDispatch dispatch,
        string failureReason,
        DateTime failedDateTime,
        CancellationToken cancellationToken = default);

    Task RecordInboundAsync(DeviceMqttInboundMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles the HTTPS polling transport. It never opens a connection to a
/// device IP address; the physical device always initiates the request.
/// </summary>
public interface IDeviceHttpsPollingService
{
    Task<DeviceHttpsPollingResponse> ProcessAsync(
        DeviceHttpsPollingRequest request,
        CancellationToken cancellationToken = default);
}
