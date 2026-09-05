// ================================================================
// Purpose : Central dispatcher. It never creates a tenant-specific worker and
//           uses the durable database queue to preserve device ordering.
// ================================================================

using axionpro.application.Interfaces.IDeviceCommunication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Publishes the next eligible command per physical device through the central MQTT client.</summary>
public sealed class DeviceCommandDispatcherWorker(
    IServiceScopeFactory scopeFactory,
    IAxionProMqttPublisher mqttPublisher,
    ILogger<DeviceCommandDispatcherWorker> logger)
    : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var queueStore = scope.ServiceProvider.GetRequiredService<IDeviceCommandDispatchStore>();
                await queueStore.RecoverExpiredResponseDeadlinesAsync(stoppingToken);
                if (!mqttPublisher.IsConnected)
                {
                    continue;
                }

                var dispatch = await queueStore.TryAcquireNextAsync(stoppingToken);
                if (dispatch is null)
                {
                    continue;
                }

                var topic = $"aiface/{dispatch.DeviceSerialNumber}/pub";
                try
                {
                    await mqttPublisher.PublishAsync(topic, dispatch.Payload, stoppingToken);
                    await queueStore.MarkPublishedAsync(dispatch, topic, qualityOfService: 1, DateTime.UtcNow, stoppingToken);
                }
                catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning(
                        exception,
                        "MQTT publish failed for DeviceCommand {DeviceCommandId} on serial {DeviceSerialNumber}.",
                        dispatch.DeviceCommandId,
                        dispatch.DeviceSerialNumber);
                    await queueStore.ScheduleRetryOrFailAsync(dispatch, exception.Message, DateTime.UtcNow, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "The central device command dispatcher iteration failed.");
            }
        }
    }
}
