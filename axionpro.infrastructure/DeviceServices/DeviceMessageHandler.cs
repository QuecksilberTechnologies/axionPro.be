using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace axionpro.infrastructure.DeviceServices
{
    public class DeviceMessageHandler
    {
        public async Task Handle(string message)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);

            string cmd = data.cmd;

            switch (cmd)
            {
                case "reg":
                    // device register
                    break;

                case "sendlog":
                    // attendance save
                    break;

                case "senduser":
                    // optional sync
                    break;
            }
        }
        public async Task<string> Handle(string message, WebSocket socket)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);

            string cmd = data.cmd;

            switch (cmd)
            {
                case "reg":
                    string sn = data.sn;
                    return sn;

                case "sendlog":
                    // save attendance
                    return null;
            }

            return null;
        }
    }
}
