using axionpro.application.Interfaces.IDeviceServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace axionpro.infrastructure.DeviceServices
{
    public class DeviceService : IDeviceService
    {
        private readonly DeviceConnectionManager _manager;

        public Task RegisterDeviceAsync(string deviceSn, WebSocket socket)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDeviceAsync(string deviceSn)
        {
            throw new NotImplementedException();
        }

        public async Task SendCommandAsync(string deviceSn, object command)
        {
            var socket = _manager.Get(deviceSn);

            if (socket == null)
                throw new Exception("Device not connected");

            var json = JsonConvert.SerializeObject(command);
            var buffer = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
