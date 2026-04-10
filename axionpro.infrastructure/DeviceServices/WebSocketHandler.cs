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
            var buffer = new byte[4096];

            string deviceSn = null;

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // 🔥 Process message
                deviceSn = await _messageHandler.Handle(message, socket);

                if (!string.IsNullOrEmpty(deviceSn))
                {
                    _connectionManager.Add(deviceSn, socket);
                }
            }

            if (!string.IsNullOrEmpty(deviceSn))
                _connectionManager.Remove(deviceSn);
        }
     
    }
}
