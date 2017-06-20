using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modbus.Device;
using System.Net.Sockets;

namespace serialread
{
    public class InputController
    {
        private class SerialInput
        {
            public int setAngle = 0;
            public int setSpeed = 0;
            public int buttons = -1;
            public double joystickX = 0;
            public double joystickY = 0;
            public double joystickZ = 0;
        }

        private class ModbusInput
        {
            public double wheel = 0;
            public double leftHandle = 0;
            public double rightHandle = 0;
        }


        private Websocket.Websocket websocket;
        private SerialInput serialInput = new SerialInput();
        private ModbusInput modbusInput = new ModbusInput();



        public InputController(Websocket.Websocket websocket)
        {
            this.websocket = websocket;

            Task serialReader = new Task(SerialReader);
            serialReader.Start();

            Task modbusReader = new Task(ModbusReader);
            modbusReader.Start();


            Task inputHandler = new Task(InputHandler);
            inputHandler.Start();

        }

        private void SerialReader()
        {

            SerialPort serial = new SerialPort(Properties.Settings.Default.ComPoort, 9600);
            Thread.Sleep(50);
            Boolean _continue = true;
            String message;
            double joystickX;
            double joystickY;
            double joystickZ;


            serial.Open();
            while (_continue)
            {
                try
                {
                    message = serial.ReadLine();
                    IList<String> serialsplitWithChecksum = message.Split('*').ToList<String>();

                    IList<string> serialsplit = serialsplitWithChecksum[0].Split(',').ToList<string>();
                    // TODO testen of het splitten goed is gegaan

                    int setAngle = Convert.ToInt32(serialsplit[3]);
                    int setSpeed = Convert.ToInt32(serialsplit[5]);
                    int buttons = Convert.ToInt32(serialsplit[6]);

                    double rawJoystickX = Convert.ToDouble(serialsplit[11]);
                    double rawJoystickY = Convert.ToDouble(serialsplit[12]);
                    double rawJoystickZ = Convert.ToDouble(serialsplit[13]);
                    char[] joystickswitch = serialsplit[10].ToCharArray();
                    int switchX = joystickswitch[0];
                    int switchY = joystickswitch[1];
                    int switchZ = joystickswitch[2];


                    //deadzone en conversion naar een double tussen -1 en 1
                    if (switchX == 1) { joystickX = 0; }
                    else { joystickX = Map(rawJoystickX, -950, 1100, -1, 1); }

                    if (switchY == 1) { joystickY = 0; }
                    else { joystickY = Map(rawJoystickY, -900, 900, -1, 1); }

                    if (switchZ == 1) { joystickZ = 0; }
                    else { joystickZ = Map(rawJoystickZ, -850, 950, -1, 1); }

                    //create new dictionary
                    
                    SerialInput newSerialInput = new SerialInput()
                    {
                        setAngle = setAngle,
                        setSpeed = setSpeed,
                        buttons = buttons,
                        joystickX = joystickX,
                        joystickY = joystickY,
                        joystickZ = joystickZ,

                    };
                    

                   
                    //update 
                    lock (serialInput)
                    {
                        serialInput = newSerialInput;
                    }

                }

                catch (Exception) { }
            }
        }


        private void ModbusReader()
        {
            TcpClient client = new TcpClient("10.0.0.21", 502);
            ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
            Boolean _continue = true;

            while (_continue)
            {
                //get raw values
                ushort rawWheelValue = master.ReadInputRegisters(7, 1)[0];
                ushort rawLeftHandleValue = master.ReadInputRegisters(4, 1)[0];
                ushort rawRightHandleValue = master.ReadInputRegisters(5, 1)[0];

                //convert to double between -1 and 1
                Double wheel = Map(rawWheelValue, 5720, 17500, -1, 1);
                Double leftHandle = Map(rawLeftHandleValue, 5728, 18824, -1, 1);
                Double rightHandle = Map(rawRightHandleValue, 5728, 18888, -1, 1);

                //deadzones
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

                //create new dictionary
                ModbusInput newModbusInput = new ModbusInput()
                {
                    wheel = wheel,
                    leftHandle = leftHandle,
                    rightHandle = rightHandle,
                };

                //update 
                lock (modbusInput)
                {
                    modbusInput = newModbusInput;
                }

                //sleep
                Thread.Sleep(1);



            }

        }


        private double Map(double value, double a1, double a2, double b1, double b2)
        {
            return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
        }

        public void InputHandler()
        {
            SerialInput currentSerialInput;
            ModbusInput currentModbusInput;
            while (true)
            {
                //get new input
                lock (serialInput)
                {
                    currentSerialInput = serialInput;
                }

                lock (modbusInput)
                {
                    currentModbusInput = modbusInput;
                }

                //control the boat
                ControlBoat();

                //do other input functions

                //sleep
                Thread.Sleep(2);
            }
        }
        private void ControlBoat()
        {
            double motorPower = serialInput[Properties.Settings.Default.MotorPower];
            double motorSteer = serialInput[Properties.Settings.Default.MotorSteer];
            double rudder = serialInput[Properties.Settings.Default.Rudder];
            Console.WriteLine("Inputhandler: " + motorPower + " - " + motorSteer);
            double leftEngine;
            double rightEngine;

            if (motorSteer > 0)
            {
                leftEngine = motorPower;
                rightEngine = -1 * Map(motorSteer, 0, 1, -1 * motorPower, 1 * motorPower);
            }

            else if (motorSteer < 0)
            {
                rightEngine = motorPower;
                leftEngine = Map(motorSteer, -1, 0, -1 * motorPower, 1 * motorPower);
            }
            else
            {
                leftEngine = rightEngine = motorPower;
            }
            websocket.ControlBoat(leftEngine, rightEngine, rudder);
        }
    }
}






