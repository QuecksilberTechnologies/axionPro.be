// ================================================================
// Purpose : Abstraction for the Host-only initial device bootstrap identity.
// ================================================================

namespace axionpro.application.Interfaces.IDeviceCommunication;

/// <summary>Returned once when a Host issues a physical-device bootstrap URL.</summary>
public sealed record DeviceInitialProvisioningIssue(
    string DeviceSerialNumber,
    string InitialGatewayUrl,
    int HeartbeatIntervalSeconds,
    DateTime ExpiresDateTime);

/// <summary>Represents a validated initial device caller without exposing its route token.</summary>
public sealed record DeviceInitialProvisioningValidation(
    long ProvisioningId,
    long DeviceMasterId,
    string DeviceSerialNumber,
    int HeartbeatIntervalSeconds);

/// <summary>Creates, validates, records, and revokes secure initial device bootstrap routes.</summary>
public interface IDeviceInitialProvisioningService
{
    Task<DeviceInitialProvisioningIssue> IssueAsync(
        long deviceMasterId,
        int lifetimeMinutes,
        long issuedById,
        CancellationToken cancellationToken = default);

    Task<DeviceInitialProvisioningValidation?> ValidateAsync(
        string deviceSerialNumber,
        string ingressToken,
        CancellationToken cancellationToken = default);

    Task RecordConnectionAsync(
        long provisioningId,
        DateTime connectedDateTime,
        CancellationToken cancellationToken = default);

    Task RevokeForDeviceMasterAsync(
        long deviceMasterId,
        DateTime revokedDateTime,
        CancellationToken cancellationToken = default);
}
