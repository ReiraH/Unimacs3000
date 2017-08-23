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
        //
        private class ModbusInput
        {
            public double wheel = 0;
            public double leftHandle = 0;
            public double rightHandle = 0;
        }


        private Websocket.OnlineWebsocket websocket;
        private SerialInput serialInput = new SerialInput();
        private ModbusInput modbusInput = new ModbusInput();
        private double oldLeftEngine = 2;
        private double oldRightEngine = 2;
        private double oldRudder = 2;
        private int oldButton = -1;


        /*
         * 
        */
        public InputController(Websocket.OnlineWebsocket websocket)
        {
            this.websocket = websocket;

            Task serialReader = new Task(SerialReader);
            serialReader.Start();

            Task modbusReader = new Task(ModbusReader);
            modbusReader.Start();


            Task inputHandler = new Task(InputHandler);
            inputHandler.Start();

        }

       
        /*
         * method for letting an variable double lock between 2 chosen values
         * input(variable you want to be locked, his lowest possible value,his highest value, the lowest value you want it to be, and the highest you want it to be)
         * output is a double between your chosen b1 and b2
        */
        private double Map(double value, double a1, double a2, double b1, double b2)
        {
            return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
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

                /*
                 *  split the serialstring in parts and parse it as intergers or doubles
                 * 
                 */
                try
                {

                    message = serial.ReadLine();
                    //Console.WriteLine(message);
                    IList<String> serialsplitWithChecksum = message.Split('*').ToList<String>();

                    IList<string> serialsplit = serialsplitWithChecksum[0].Split(',').ToList<string>();
                    
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


                    //deadzone and conversion to an double between -1, and 1 
                    if (switchX == '1') { joystickX = 0; }
                    else { joystickX = Map(rawJoystickX, -900, 1040, -1, 1); }

                    if (switchY == '1') { joystickY = 0; }
                    else { joystickY = Map(rawJoystickY, -800, 875, -1, 1); }

                    //cable is loose so I set it to 2 so it will always be false
                    if (switchZ == '2') { joystickZ = 0; }
                    else { joystickZ = Map(rawJoystickZ, -820, 920, -1, 1); }

                    //create new input object
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
                catch(Exception e)
                {

                }
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

                //create new dictionary and let it update when the value is different
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
        // handle all the input
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

                //control the boat send it the serial data parts and current modbusinput
                ControlBoat(currentSerialInput, currentModbusInput);

                //do other input functions
                ChangeControls(currentSerialInput);
                ChangeBoat(currentSerialInput);
                DeselectBoat(currentSerialInput);

                //sleep
                oldButton = currentSerialInput.buttons;
                Thread.Sleep(2);
            }
        }
        /*
         * boatcontrol expect an steer mode from the serialdata and controls boat with that mode 
         */
        private void ControlBoat(SerialInput serial, ModbusInput modbus)
        {
            double leftEngine = 0;
            double rightEngine = 0;
            double rudder = 0;

            string mode = Properties.Settings.Default.ControlMode;

            if (mode == "joystick")
            {
                Joystick();
            }
            else if ( mode == "joystickWheel")
            {
                JoystickWheel();
            }
            else if ( mode == "leverWheel")
            {
                LeverWheel();
            }
            else
            {
                Console.WriteLine("Control Mode not recognised!");
            }

            if(leftEngine != oldLeftEngine | rightEngine != oldRightEngine | rudder != oldRudder)
            {
                websocket.ControlBoat(leftEngine, rightEngine, rudder);
                oldLeftEngine = leftEngine;
                oldRightEngine = rightEngine;
                oldRudder = rudder;
            }

            //controller joystick
            void Joystick()
            {
                if (serial.joystickZ > 0)
                {
                    leftEngine = serial.joystickY;
                    rightEngine = -1 * Map(serial.joystickZ, 0, 1, -1 * serial.joystickY, 1 * serial.joystickY);
                }

                else if (serial.joystickZ < 0)
                {
                    rightEngine = serial.joystickY;
                    leftEngine = Map(serial.joystickZ, -1, 0, -1 * serial.joystickY, 1 * serial.joystickY);
                }
                else
                {
                    leftEngine = rightEngine = serial.joystickY;
                }

                rudder = serial.joystickX;
            }
            //cotroller for joystick and wheel
            void JoystickWheel()
            {
                if (serial.joystickX > 0)
                {
                    leftEngine = serial.joystickY;
                    rightEngine = -1 * Map(serial.joystickX, 0, 1, -1 * serial.joystickY, 1 * serial.joystickY);
                }

                else if (serial.joystickX < 0)
                {
                    rightEngine = serial.joystickY;
                    leftEngine = Map(serial.joystickX, -1, 0, -1 * serial.joystickY, 1 * serial.joystickY);
                }
                else
                {
                    leftEngine = rightEngine = serial.joystickY;
                }

                rudder = modbus.wheel;
            }
            //control of lever and wheel
            void LeverWheel()
            {
                leftEngine = modbus.leftHandle;
                rightEngine = modbus.rightHandle;
                rudder = modbus.wheel;
            }
        }
        //change of controlls
        private void ChangeControls(SerialInput serial)
        {
            int joystick = Properties.Settings.Default.SwitchToJoystick;
            int joystickWheel = Properties.Settings.Default.SwitchToJoystickWheel;
            int leverWheel = Properties.Settings.Default.SwitchToLeverWheel;
            if (serial.buttons == joystick)
                Properties.Settings.Default.ControlMode = "joystick";
            else if (serial.buttons == joystickWheel)
                Properties.Settings.Default.ControlMode = "joystickWheel";
            else if (serial.buttons == leverWheel)
                Properties.Settings.Default.ControlMode = "leverWheel";

        }

        //change to other boat
        private void ChangeBoat(SerialInput serial)
        {
            int changeBoatButton = Properties.Settings.Default.ChangeBoat;
            if(serial.buttons == changeBoatButton && serial.buttons!= oldButton)
            {
                websocket.SelectNextBoat();
            }
        }
        //deselect boat
        private void DeselectBoat(SerialInput serial)
        {
            int deselectBoatButton = Properties.Settings.Default.DeselectBoat;
            if(serial.buttons == deselectBoatButton)
            {
                websocket.DeselectBoat();
            }
        }
        //change navigationmode
        private void ChangeNavigationMode(SerialInput serial)
        {
            int changeToDeskMode = Properties.Settings.Default.ChangeToDeskMode;
            int changeToQuayMode = Properties.Settings.Default.ChangeToQuayMode;
            int changeToGPSMode = Properties.Settings.Default.ChangeToGPSMode;
            if(changeToDeskMode == serial.buttons)
            {
                websocket.ChangeToDeskMode();
            }
            else if(changeToGPSMode == serial.buttons)
            {
                websocket.ChangeToGpsMode();
            }
            else if(changeToQuayMode == serial.buttons)
            {
                websocket.ChangeToQuayMode();
            }
        }
    }
}




