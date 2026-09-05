// ================================================================
// Purpose : Processes device-initiated HTTPS polling without treating the
//           anonymous device endpoint as an authenticated user API.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Interfaces.IDeviceCommunication;
using MediatR;

namespace axionpro.application.Features.DeviceCommandCmd;

/// <summary>Represents one raw inbound HTTPS device request after controller size checks.</summary>
public sealed record ProcessDeviceHttpsPolling(
    string IngressToken,
    string Payload,
    DateTime ReceivedDateTime) : IRequest<DeviceHttpsPollingResponse>;

/// <summary>Delegates a validated device poll to the durable command pipeline.</summary>
public sealed class ProcessDeviceHttpsPollingHandler(IDeviceHttpsPollingService pollingService)
    : IRequestHandler<ProcessDeviceHttpsPolling, DeviceHttpsPollingResponse>
{
    /// <inheritdoc />
    public Task<DeviceHttpsPollingResponse> Handle(
        ProcessDeviceHttpsPolling request,
        CancellationToken cancellationToken) =>
        pollingService.ProcessAsync(
            new DeviceHttpsPollingRequest(request.IngressToken, request.Payload, request.ReceivedDateTime),
            cancellationToken);
}

/// <summary>
/// Fails closed before persistence when a caller does not resemble a provisioned
/// device gateway request. The controller deliberately turns this into a 404 so
/// a caller cannot enumerate valid device URLs.
/// </summary>
public sealed class DeviceHttpsGatewaySecurityBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ProcessDeviceHttpsPolling poll)
        {
            return next(cancellationToken);
        }

        if (!DeviceHttpsGatewaySecurity.IsValidIngressToken(poll.IngressToken) ||
            string.IsNullOrWhiteSpace(poll.Payload) ||
            poll.Payload.Length > 131_072)
        {
            return Task.FromResult((TResponse)(object)DeviceHttpsPollingResponse.Rejected);
        }

        return next(cancellationToken);
    }
}
