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

        int setAngle;
        int setSpeed;
        int buttons;
        int joystickSwitch;
        double joystickControlX;
        double joystickControlY;
        double joystickControlZ;
        Websocket.Websocket websocket;

        public InputController(Websocket.Websocket websocket)
        {
            //StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Task readThread = new Task(Read);
            readThread.Start();
            this.websocket = websocket;

        }

        public void Read()
        {
            SerialPort serial = new SerialPort("COM4", 9600);
            Boolean _continue = true;
            String message;
            String joystickControlZAndSum;
           
            serial.Open();
            while (_continue)
            {
                try
                {   
                    message = serial.ReadLine();
                    IList<string> serialsplit = message.Split(',').Reverse().ToList<string>();
                    // testen of de goede gesplit is
                  //  setAngle = Convert.ToInt32(serialsplit[3]);
                   // setSpeed = Convert.ToInt32(serialsplit[5]);
                    //buttons = Convert.ToInt32(serialsplit[6]);
                    //joystickSwitch = Convert.ToInt32(serialsplit[10]);
                    joystickControlY = Convert.ToDouble(serialsplit[1]);
                    joystickControlX = Convert.ToDouble(serialsplit[2]);
                  //  joystickControlZAndSum = serialsplit[13];
                   // IList<String> joystickZsplitter = joystickControlZAndSum.Split('*').ToList<String>();
                   // joystickControlZ = Convert.ToDouble(joystickZsplitter[0]);
                    //Boatcontrol boatcontrol;
                    joystickControlX = map(joystickControlX, -950, 1100, -1, 1);
                    joystickControlY = map(joystickControlY, -850, 900, -1, 1);
                    websocket.ControlBoat(joystickControlY, joystickControlY, joystickControlX);
                    //boatcontrol = new Boatcontrol((joystickControlX + 800) / 1680, (joystickControlY + 880) / 1930);



                }

                catch (TimeoutException) { }
            }
        }
        double map(double s, double a1, double a2, double b1, double b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public void Dictionaries()
        {
            Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();

            dictionary.Add("setAngle",setAngle);
            dictionary.Add("setSpeed",setSpeed);
            dictionary.Add("Buttons", buttons);
            dictionary.Add("JoystickSwitch", joystickSwitch);
            dictionary.Add("JoystickControlY",joystickControlY );
            dictionary.Add("JoystickControlX", joystickControlX );
            dictionary.Add("joystickControlZ", joystickControlZ );
            
        }
        


        /*
        public void Knoppen()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("dim",-2 ) ;
            dictionary.Add("geenKnop",-1 );
            dictionary.Add("ready", 0);
            dictionary.Add("alarm", 1);
            dictionary.Add("ecdis", 2);
            dictionary.Add("deskControl", 3);
            dictionary.Add("remoteControl", 4);
            dictionary.Add("surgeJoystick", 5);
            dictionary.Add("surgeAuto", 6);
            dictionary.Add("position", 7);
            dictionary.Add("swayJoystick", 8);
            dictionary.Add("swayAuto", 9);
            dictionary.Add("headingJoystick", 10);
            dictionary.Add("", 11);
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );
            dictionary.Add("", );

        }
        */


    }

    }


