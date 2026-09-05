// ================================================================
// Purpose : Owns one central server-side MQTT/MQTTS client for every Tenant
//           device. Broker secrets are read from application configuration.
// ================================================================

using System.Buffers;
using System.Text;
using MQTTnet;
using MQTTnet.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace axionpro.infrastructure.DeviceCommunication.Mqtt;

/// <summary>Provides the single central broker connection and QoS 1 publication path.</summary>
public sealed class AxionProMqttClient : IAxionProMqttPublisher, IAsyncDisposable
{
    private readonly AxionProMqttOptions _options;
    private readonly DeviceMqttMessageRouter _messageRouter;
    private readonly ILogger<AxionProMqttClient> _logger;
    private readonly IMqttClient _client;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public AxionProMqttClient(
        IOptions<AxionProMqttOptions> options,
        DeviceMqttMessageRouter messageRouter,
        ILogger<AxionProMqttClient> logger)
    {
        _options = options.Value;
        _messageRouter = messageRouter;
        _logger = logger;
        _client = new MqttClientFactory().CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
        _client.ConnectedAsync += SubscribeToDeviceResponsesAsync;
        _client.DisconnectedAsync += arguments =>
        {
            _logger.LogWarning("Central AxionPro MQTT client disconnected. Reason: {Reason}", arguments.Reason);
            return Task.CompletedTask;
        };
    }

    /// <inheritdoc />
    public bool IsConnected => _client.IsConnected;

    /// <summary>Connects the central client once. Repeated calls are safe.</summary>
    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled || _client.IsConnected)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("DeviceMqtt:Host must be configured when DeviceMqtt:Enabled is true.");
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_client.IsConnected)
            {
                return;
            }

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(_options.ClientId)
                .WithTcpServer(_options.Host, _options.Port)
                .WithCleanSession(false);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                builder.WithCredentials(_options.UserName, _options.Password);
            }

            if (_options.UseTls)
            {
                builder.WithTlsOptions(tls => tls.UseTls());
            }

            await _client.ConnectAsync(builder.Build(), cancellationToken);
            _logger.LogInformation("Connected the central AxionPro MQTT client to {Host}:{Port} using TLS: {UseTls}.", _options.Host, _options.Port, _options.UseTls);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        if (!_client.IsConnected)
        {
            throw new InvalidOperationException("The central AxionPro MQTT client is not connected.");
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();
        await _client.PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _client.ApplicationMessageReceivedAsync -= HandleMessageAsync;
        _client.ConnectedAsync -= SubscribeToDeviceResponsesAsync;
        if (_client.IsConnected)
        {
            await _client.DisconnectAsync();
        }

        _connectionLock.Dispose();
        _client.Dispose();
    }

    private async Task SubscribeToDeviceResponsesAsync(MqttClientConnectedEventArgs arguments)
    {
        var options = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(filter => filter.WithTopic("aiface/+/sub").WithAtLeastOnceQoS())
            .Build();
        await _client.SubscribeAsync(options, CancellationToken.None);
        _logger.LogInformation("Subscribed the central AxionPro MQTT client to aiface/+/sub.");
    }

    private async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs arguments)
    {
        var payload = Encoding.UTF8.GetString(arguments.ApplicationMessage.Payload.ToArray());
        await _messageRouter.RouteAsync(
            arguments.ApplicationMessage.Topic,
            payload,
            (int)arguments.ApplicationMessage.QualityOfServiceLevel,
            arguments.ApplicationMessage.Dup,
            CancellationToken.None);
    }
}
