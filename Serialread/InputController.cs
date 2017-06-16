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
            {"Buttons", -1},
            {"JoystickY", 0.0},
            {"JoystickX", 0.0},
            {"joystickZ", 0.0}
            
        };

        Websocket.Websocket websocket;

        public InputController(Websocket.Websocket websocket)
        {

            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Task readThread = new Task(Read);
            readThread.Start();
            this.websocket = websocket;

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
                    // testen of het splitten goed is gegaan
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
                        dictionary["joystickX"] = map(joystickX, -950, 1100, -1, 1);
                        dictionary["joystickY"] = map(joystickY, -850, 900, -1, 1);
                        dictionary["joystickZ"] = map(joystickZ, 10, 10, -1, 1);// nog ffkies uitzoeke
                    }
                    //websocket.ControlBoat(dictionary["joystickX"], dictionary[" joystickX"], dictionary[" joystickY"]);
                    
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


