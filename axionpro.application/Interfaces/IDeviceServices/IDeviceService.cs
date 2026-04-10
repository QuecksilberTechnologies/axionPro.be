using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace axionpro.application.Interfaces.IDeviceServices
{
    public interface IDeviceService
    {
        Task SendCommandAsync(string deviceSn, object command);
        Task RegisterDeviceAsync(string deviceSn, WebSocket socket);
        Task RemoveDeviceAsync(string deviceSn);
    }
}
