// ================================================================
// Purpose : Maintains the single central connection. It never creates a
//           per-tenant client, subscription, or hosted service.
// ================================================================

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Keeps the central MQTT/MQTTS client connected with bounded reconnect attempts.</summary>
public sealed class AxionProMqttHostedService(
    AxionProMqttClient mqttClient,
    IOptions<AxionProMqttOptions> options,
    ILogger<AxionProMqttHostedService> logger)
    : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogWarning("Central MQTT device communication is disabled. No device command will be published until DeviceMqtt:Enabled is configured.");
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await mqttClient.ConnectAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Central MQTT connection attempt failed; retrying without creating tenant-specific clients.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}
