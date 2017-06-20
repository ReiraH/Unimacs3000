using Modbus.Device;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            readThread.Start();
            this.websocket = websocket;
        }
        public void modBusRead()
        {
            Boolean _continue = true;
            ushort wheelValue;
            ushort leftHandleValue;
            ushort rightHandleValue;
            Double oldRLeftHandle = 0;
            Double oldRightHandle = 0;
            Double oldWheel = 0;
            while(_continue)
            {
                try
                {
                    wheelValue = master.ReadInputRegisters(7, 1)[0];
                    leftHandleValue = master.ReadInputRegisters(4, 1)[0];
                    rightHandleValue = master.ReadInputRegisters(5, 1)[0];
                    Double wheel = map(wheelValue, 5720, 17500, -1, 1);
                    Double leftHandle = map(leftHandleValue, 5728, 18824, -1, 1);
                    Double rightHandle = map(rightHandleValue, 5728, 18888, -1, 1);
                    if (rightHandle < 0.02 && rightHandle > -0.02)
                    {
                        rightHandle = 0;
                   }
                    if (leftHandle < 0.02 && leftHandle > -0.02)
                    {
                        leftHandle = 0;
                   }
                    if (wheel < 0.175 && wheel > -0.175)
                    {
                        wheel = 0;
                   }
                    if(oldRLeftHandle != leftHandle | oldRightHandle != rightHandle | oldWheel != wheel)
                    {
                        websocket.ControlBoat(leftHandle, rightHandle, wheel);
                    }
                    oldRightHandle = rightHandle;
                    oldRLeftHandle = leftHandle;
                    oldWheel = wheel;
                    Console.WriteLine(leftHandle + " - " + rightHandle + " - " + wheel);
                    System.Threading.Thread.Sleep(100);

                }
                catch (TimeoutException) { }
            }
        }
        double map(double s, double a1, double a2, double b1, double b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

    }
}
