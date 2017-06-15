using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Websocket;

namespace ModBusReader
{

    public class ModBusReader
    {
        TcpClient client;
        ModbusIpMaster master;
        Websocket.Websocket websocket;
        public ModBusReader(Websocket.Websocket websocket)
        {
            Task readThread = new Task(modBusRead);
            client = new TcpClient("10.0.0.21", 502);
            master = ModbusIpMaster.CreateIp(client);
            this.websocket = websocket;
        }
        public void modBusRead()
        {
            Boolean _continue = true;
            ushort wheelValue;
            ushort leftHandleValue;
            while(_continue)
            {
                try
                {
                    wheelValue = master.ReadInputRegisters(7, 1)[0];
                    leftHandleValue = master.ReadInputRegisters(4, 1)[0];
                }
                catch (TimeoutException) { }
            }
        }
    }
}
