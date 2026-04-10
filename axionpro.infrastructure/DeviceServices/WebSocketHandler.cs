using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace axionpro.infrastructure.DeviceServices
{
    public class WebSocketHandler
    {
        private readonly DeviceMessageHandler _messageHandler;
        private readonly DeviceConnectionManager _connectionManager;

        public WebSocketHandler(
            DeviceMessageHandler messageHandler,
            DeviceConnectionManager connectionManager)
        {
            _messageHandler = messageHandler;
            _connectionManager = connectionManager;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();

            Console.WriteLine("🔥 Device Connected");

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine($"📩 Received: {message}");
            }
        }

    }
}
