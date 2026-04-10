using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace axionpro.infrastructure.DeviceServices
{
    public class DeviceConnectionManager
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _devices = new();

        public void Add(string sn, WebSocket socket)
            => _devices[sn] = socket;

        public WebSocket Get(string sn)
            => _devices.TryGetValue(sn, out var socket) ? socket : null;

        public void Remove(string sn)
            => _devices.TryRemove(sn, out _);
    }
}
