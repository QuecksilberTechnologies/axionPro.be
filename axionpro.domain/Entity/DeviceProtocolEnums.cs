// ================================================================
// Purpose : Defines the MQTT/MQTTS-only device communication lifecycle.
// ================================================================

namespace axionpro.domain.Entity;

/// <summary>Identifies the supported V1 transport for a Tenant device.</summary>
public enum DeviceCommunicationProtocol : short
{
    /// <summary>MQTT over TCP.</summary>
    Mqtt = 1,

    /// <summary>MQTT over TLS.</summary>
    Mqtts = 2
}

/// <summary>Represents the durable server-side state of a device command.</summary>
public enum DeviceCommandStatus : short
{
    Queued = 1,
    Publishing = 2,
    AwaitingResponse = 3,
    Completed = 4,
    Failed = 5,
    RetryScheduled = 6,
    Cancelled = 7
}

/// <summary>Defines whether a command expects a vendor-protocol response.</summary>
public enum DeviceCommandResponseMode : short
{
    Required = 1,
    Streamed = 2,
    PublishOnly = 3
}

/// <summary>Classifies the authorization risk of a vendor command.</summary>
public enum DeviceCommandAccessLevel : short
{
    TenantPermission = 1,
    TenantAccessControlPermission = 2,
    HostOnly = 3
}

/// <summary>Identifies the direction of a raw MQTT protocol message.</summary>
public enum DeviceMessageDirection : short
{
    Inbound = 1,
    Outbound = 2
}

/// <summary>Describes why a device credential is retained.</summary>
public enum DeviceCredentialType : short
{
    Mqtt = 1,
    Mqtts = 2,
    ClientCertificate = 3
}
