using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serialread
{
    public class InputController
    {
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>
        {
            {"setAngle", 0.0},
            {"setSpeed", 0.0},
            {"buttons", -1},
            {"joystickY", 0.0},
            {"joystickX", 0.0},
            {"joystickZ", 0.0}

        };

        Websocket.Websocket websocket;

        public InputController(Websocket.Websocket websocket)
        {
            this.websocket = websocket;
            Task readThread = new Task(Read);
            readThread.Start();
            Task inputHandler = new Task(InputHandler);
            inputHandler.Start();

        }

        public void Read()
        {
            
            SerialPort serial = new SerialPort(Properties.Settings.Default.ComPoort, 9600);
            Thread.Sleep(50);
            Boolean _continue = true;
            String message;
            String joystickControlZAndSum;

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
                    dictionary["joystickSwitch"] = Convert.ToInt32(serialsplit[10]);
                    double joystickX = Convert.ToDouble(serialsplit[11]);
                    double joystickY = Convert.ToDouble(serialsplit[12]);
                    joystickControlZAndSum = serialsplit[13];
                    IList<String> joystickZsplitter = joystickControlZAndSum.Split('*').ToList<String>();
                    double joystickZ = Convert.ToDouble(joystickZsplitter[0]);

                    if (dictionary["joystickSwitch"] = 1000000) { dictionary["joystickX"] = 0; }
                    else{dictionary["joystickX"] = Map(joystickX, -950, 1100, -1, 1);}

                    if (dictionary["joystickSwitch"] = 0100000) { dictionary["joystickY"] = 0; }
                    else { dictionary["joystickY"] = Map(joystickY, -900, 900, -1, 1); }

                    if (dictionary["joystickSwitch"] = 0010000) { dictionary["joystickZ"] = 0; }
                    else { dictionary["joystickZ"] = Map(joystickZ, -850, 950, -1, 1); }
                    
                    //Console.WriteLine("setAngle: "+ dictionary["setAngle"]);
                    Console.WriteLine("buttons: " + dictionary["buttons"]);

                    //websocket.ControlBoat(dictionary["joystickX"], dictionary[" joystickX"], dictionary[" joystickY"]);

                }

                catch (Exception) { }
            }
        }
        double Map(double value, double a1, double a2, double b1, double b2)
        {
            return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
        }

        public void InputHandler() {
            while (true)
            {
               
                Thread.Sleep(2);
            }
        }
    }
}
/*
               //control boat
               double motorPower = dictionary[Properties.Settings.Default.MotorPower];
               double motorSteer = dictionary[Properties.Settings.Default.MotorSteer];
               double rudder = dictionary[Properties.Settings.Default.Rudder];
               Console.WriteLine("Inputhandler: "+ motorPower +  " - " + motorSteer);
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
               */






