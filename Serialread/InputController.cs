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
        private Dictionary<string, dynamic> serialInput = new Dictionary<string, dynamic>
        {
            {"setAngle", 0.0},
            {"setSpeed", 0.0},
            {"buttons", -1},
            {"joystickY", 0.0},
            {"joystickX", 0.0},
            {"joystickZ", 0.0}

        };

        private Dictionary<string, dynamic> modbusInput = new Dictionary<string, dynamic>
        {
            {"wheel", 0.0 },
            {"leftHandle", 0.0 },
            {"rightHandle", 0.0 }
        };

        Websocket.Websocket websocket;

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
            String joystickControlZAndSum;
            String joystickswitch;
            int switchX;
            int switchY;
            int switchZ;

            serial.Open();
            while (_continue)
            {
                try
                {
                    message = serial.ReadLine();
                    IList<string> serialsplit = message.Split(',').ToList<string>();
                    // testen of het splitten goed is gegaan
                    
                    dictionary["setAngle"] = Convert.ToInt32(serialsplit[3]);
                    dictionary["setSpeed"] = Convert.ToInt32(serialsplit[5]);
                    dictionary["buttons"] = Convert.ToInt32(serialsplit[6]);
                    double joystickX = Convert.ToDouble(serialsplit[11]);
                    double joystickY = Convert.ToDouble(serialsplit[12]);
                    joystickControlZAndSum = serialsplit[13];
                    IList<String> joystickZsplitter = joystickControlZAndSum.Split('*').ToList<String>();
                    double joystickZ = Convert.ToDouble(joystickZsplitter[0]);
                    joystickswitch = serialsplit[10];
                    joystickswitch.ToCharArray();
                    switchX = joystickswitch[0];
                    switchY = joystickswitch[1];
                    switchZ = joystickswitch[2];

                    if (switchX ==1) { dictionary["joystickX"] = 0; }
                    else{dictionary["joystickX"] = Map(joystickX, -950, 1100, -1, 1);}

                    if (switchY==1){ dictionary["joystickY"] = 0; }
                    else { dictionary["joystickY"] = Map(joystickY, -900, 900, -1, 1); }

                    if (switchZ == 1) { dictionary["joystickZ"] = 0; }
                    else { dictionary["joystickZ"] = Map(joystickZ, -850, 950, -1, 1); }
                    
                    //Console.WriteLine("setAngle: "+ dictionary["setAngle"]);
                    Console.WriteLine("buttons: " + serialInput["buttons"]);

                    //websocket.ControlBoat(dictionary["joystickX"], dictionary[" joystickX"], dictionary[" joystickY"]);

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
                Dictionary<string, dynamic> newModbusInput = new Dictionary<string, dynamic>
                {
                    {"wheel", wheel },
                    {"leftHandle", leftHandle },
                    {"rightHandle", rightHandle }
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

        public void InputHandler() {
            while (true)
            {
                ControlBoat();
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






