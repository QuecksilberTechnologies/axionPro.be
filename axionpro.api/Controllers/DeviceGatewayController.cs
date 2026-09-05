// ================================================================
// Purpose : Receives outbound HTTPS polling from provisioned devices. This API
//           does not open or proxy a connection to a device LAN IP address.
// ================================================================

using System.Buffers;
using System.Text;
using axionpro.application.Features.DeviceCommandCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace axionpro.api.Controllers;

/// <summary>Private bearer-route endpoint for device-initiated HTTPS polling.</summary>
[AllowAnonymous]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("device-gateway")]
[EnableRateLimiting("device-gateway")]
public sealed class DeviceGatewayController(IMediator mediator) : ControllerBase
{
    private const int MaximumPayloadBytes = 131_072;

    /// <summary>Receives an authenticated-by-route device poll and returns raw vendor JSON.</summary>
    [HttpPost("{ingressToken}")]
    [RequestSizeLimit(MaximumPayloadBytes)]
    public async Task<IActionResult> Poll(string ingressToken, CancellationToken cancellationToken)
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
            new ProcessDeviceHttpsPolling(ingressToken, payload, DateTime.UtcNow),
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
