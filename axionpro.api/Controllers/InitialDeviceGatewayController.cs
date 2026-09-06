// ================================================================
// Purpose : Receives short-lived, opaque-token authenticated initial device
//           polls. It never accepts serial-only traffic.
// ================================================================

using System.Buffers;
using System.Text;
using axionpro.application.Features.DeviceCommandCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace axionpro.api.Controllers;

/// <summary>Hidden device-only bootstrap route; this is not a user-facing public API.</summary>
[AllowAnonymous]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/initial")]
[EnableRateLimiting("device-gateway")]
public sealed class InitialDeviceGatewayController(IMediator mediator) : ControllerBase
{
    private const int MaximumPayloadBytes = 131_072;

    /// <summary>Accepts a device POST only when both serial and opaque route token match.</summary>
    [HttpPost("{deviceSerialNumber}/{ingressToken}")]
    [RequestSizeLimit(MaximumPayloadBytes)]
    public async Task<IActionResult> Poll(
        string deviceSerialNumber,
        string ingressToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Request.ContentType) ||
            !Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound();
        }

        var payload = await ReadBoundedPayloadAsync(Request, cancellationToken);
        if (payload is null)
        {
            return NotFound();
        }

        var response = await mediator.Send(
            new ProcessInitialDeviceHttpsPolling(deviceSerialNumber, ingressToken, payload, DateTime.UtcNow),
            cancellationToken);
        return !response.IsAccepted
            ? NotFound()
            : Content(response.ResponsePayload, "application/json", Encoding.UTF8);
    }

    private static async Task<string?> ReadBoundedPayloadAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        if (request.ContentLength is > MaximumPayloadBytes)
        {
            return null;
        }

        var rentedBuffer = ArrayPool<byte>.Shared.Rent(8_192);
        try
        {
            await using var content = new MemoryStream();
            while (true)
            {
                var bytesRead = await request.Body.ReadAsync(rentedBuffer.AsMemory(0, rentedBuffer.Length), cancellationToken);
                if (bytesRead == 0)
                {
                    break;
                }

                if (content.Length + bytesRead > MaximumPayloadBytes)
                {
                    return null;
                }

                await content.WriteAsync(rentedBuffer.AsMemory(0, bytesRead), cancellationToken);
            }

            return Encoding.UTF8.GetString(content.GetBuffer(), 0, checked((int)content.Length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }
}
