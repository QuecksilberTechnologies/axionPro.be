namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Publishes through the one central AxionPro MQTT/MQTTS client.</summary>
public interface IAxionProMqttPublisher
{
    bool IsConnected { get; }

    Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default);
}
