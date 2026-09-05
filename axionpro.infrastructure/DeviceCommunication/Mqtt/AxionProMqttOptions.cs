// ================================================================
// Purpose : Central broker configuration. Production credentials are supplied
//           by secure application/environment configuration, never a tenant row.
// ================================================================

namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Configuration for the single AxionPro server-side MQTT/MQTTS connection.</summary>
public sealed class AxionProMqttOptions
{
    public const string SectionName = "DeviceMqtt";

    /// <summary>Enables the central client after production secret configuration is supplied.</summary>
    public bool Enabled { get; init; }

    /// <summary>Central broker host name.</summary>
    public string Host { get; init; } = string.Empty;

    /// <summary>Central broker TCP/TLS port.</summary>
    public int Port { get; init; } = 8883;

    /// <summary>Uses TLS for MQTTS.</summary>
    public bool UseTls { get; init; } = true;

    /// <summary>Server-side client ID. It must be stable across reconnects.</summary>
    public string ClientId { get; init; } = "axionpro-device-gateway";

    /// <summary>Server-side broker user name from secure configuration.</summary>
    public string? UserName { get; init; }

    /// <summary>Server-side broker secret from secure configuration.</summary>
    public string? Password { get; init; }
}
