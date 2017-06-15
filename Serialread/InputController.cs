﻿using System;
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
            {"Buttons", -1},
            {"JoystickY", 0.0},
            {"JoystickX", 0.0},
            {"joystickZ", 0.0}

        };

        Websocket.Websocket websocket;

        public InputController(Websocket.Websocket websocket)
        {
            this.websocket = websocket;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Task readThread = new Task(Read);
            readThread.Start();
            Task inputHandler = new Task(InputHandler);
            inputHandler.Start();

        }

        public void Read()
        {
            SerialPort serial = new SerialPort("COM5", 9600);
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

                    dictionary["setAngle"] = Convert.ToInt32(serialsplit[3]);
                    dictionary["setSpeed"] = Convert.ToInt32(serialsplit[5]);
                    dictionary["buttons"] = Convert.ToInt32(serialsplit[6]);
                    dictionary["joystickSwitch"] = Convert.ToInt32(serialsplit[10]);
                    double joystickY = Convert.ToDouble(serialsplit[11]);
                    double joystickX = Convert.ToDouble(serialsplit[12]);
                    joystickControlZAndSum = serialsplit[13];
                    IList<String> joystickZsplitter = joystickControlZAndSum.Split('*').ToList<String>();
                    double joystickZ = Convert.ToDouble(joystickZsplitter[0]);

                    if (joystickZ == 1)
                    {
                        dictionary["joystickX"] = Map(joystickX, -800, 1680, -1, 1);
                        dictionary["joystickY"] = Map(joystickY, 880, 1930, -1, 1);
                        dictionary["joystickZ"] = map(joystickZ,)
                        dictionary["joystickControlZ"] = Convert.ToDouble(joystickZsplitter[0]);
                    }
                    //websocket.ControlBoat(dictionary["joystickX"], dictionary[" joystickX"], dictionary[" joystickY"]);

                }

                catch (TimeoutException) { }
            }
        }
        double Map(double value, double a1, double a2, double b1, double b2)
        {
            return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
        }

        public void InputHandler() {
            //control boat
            double motorPower = dictionary[Properties.Settings.Default.MotorPower];
            double motorSteer = dictionary[Properties.Settings.Default.MotorSteer];
            double rudder = dictionary[Properties.Settings.Default.Rudder];
            double leftEngine;
            double rightEngine;

            if (motorSteer > 0)
            {
                leftEngine = motorPower;
                rightEngine = -1 * Map(motorSteer, 0, 1, -1 * motorPower, 1 * motorPower);
            }

            else if (motorSteer < 0) { 
                rightEngine = motorPower;
                leftEngine = Map(motorSteer, -1, 0, -1 * motorPower, 1 * motorPower);
            }
            else
            {
                leftEngine = rightEngine = motorPower;
            }
            websocket.ControlBoat(leftEngine, rightEngine, rudder);



            //other functions to be added later here



            Thread.Sleep(2);
        }
    }
}






