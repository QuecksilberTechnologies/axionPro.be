// ================================================================
// Purpose : Validates vendor MQTT topic/payload identity then delegates tenant
//           resolution and idempotent response matching to persistence.
// ================================================================

using System.Text.Json;
using axionpro.application.Interfaces.IDeviceCommunication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Routes inbound messages received on <c>aiface/{SN}/sub</c>.</summary>
public sealed class DeviceMqttMessageRouter(
    IServiceScopeFactory scopeFactory,
    ILogger<DeviceMqttMessageRouter> logger)
{
    /// <summary>Audits and processes an inbound device publication without trusting client tenant data.</summary>
    public async Task RouteAsync(
        string topic,
        string payload,
        int qualityOfService,
        bool isDuplicateDelivery,
        CancellationToken cancellationToken)
    {
        if (!TryGetDeviceSerialNumber(topic, out var serialNumber))
        {
            logger.LogWarning("Rejected MQTT topic outside the documented device response route: {Topic}", topic);
            return;
        }

        var identityValid = IsPayloadSerialCompatible(payload, serialNumber);
        using var scope = scopeFactory.CreateScope();
        var queueStore = scope.ServiceProvider.GetRequiredService<IDeviceCommandDispatchStore>();
        await queueStore.RecordInboundAsync(
            new DeviceMqttInboundMessage(
                topic,
                serialNumber,
                payload,
                qualityOfService,
                isDuplicateDelivery,
                identityValid,
                DateTime.UtcNow),
            cancellationToken);
    }

    private static bool TryGetDeviceSerialNumber(string topic, out string serialNumber)
    {
        serialNumber = string.Empty;
        var segments = topic.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length != 3 ||
            !string.Equals(segments[0], "aiface", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(segments[2], "sub", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(segments[1]))
        {
            return false;
        }

        serialNumber = segments[1];
        return true;
    }

    private static bool IsPayloadSerialCompatible(string payload, string topicSerialNumber)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty("sn", out var serialProperty))
            {
                // Older vendor payload shapes do not always include sn; the topic remains authoritative.
                return true;
            }

            return serialProperty.ValueKind == JsonValueKind.String &&
                   string.Equals(serialProperty.GetString()?.Trim(), topicSerialNumber, StringComparison.Ordinal);
        }
        catch (JsonException)
        {
            // Keep and audit malformed raw payloads, but never route them to a command completion.
            return false;
        }
    }
}
