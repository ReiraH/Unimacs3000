using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Serialread
{
    class Program
    {

        static void Main(string[] args)
        {
            
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);
            readThread.Start();
        }

        public static void Read()
        {
            SerialPort serial = new SerialPort("COM5", 9600);
            Boolean _continue = true;
            string serialdata;


            serial.Open();
            while (_continue)
            {
                try
                {
                    serialdata = serial.ReadLine();
                    joyStickControl(serialdata);
                }
                catch (TimeoutException) { }
            }
        }
        
        
        public static void joyStickControl(String s)// joystick heeft orgineel waarde x-880 tot 1000,-770 tot 900
        {
            String _xMovement;
            String _yMovement;
            float xMovement;
            float yMovement;

             
          
            
            IList<string> xyPosition = s.Split(',').Reverse().ToList<string>();
            _yMovement = xyPosition[2];
            _xMovement = xyPosition[1];
            xMovement = (float) Convert.ToDouble(_xMovement);
            yMovement = (float) Convert.ToDouble(_yMovement);
            //Console.WriteLine(" xmove is: " + xMovement + "ymove is :" + yMovement);
            //Console.WriteLine(xyPosition[1]+" "+xyPosition[2]);

            /*   
            x tussen 0 en 1920  orgineel -880 tot 1000
             y tussen 0 en 1630  orgineel -770 tot 900
            */
            boatControl((xMovement+800)/1680,(yMovement+880)/1930); // stuurt het getal tussen 0 en 1
        }



        public static String boatControl(double xMovement, double yMovement)// geef getal tussen 0 en 1
        {         
            double steer= Math.Round(yMovement,1);
            double motor1= Math.Round(xMovement,1);
            double motor2 = motor1;
            String final;

            if (steer < 0.2)// naar links sturen
            {
                if (motor2 == 1.0) { motor1 -= 0.6; }// scherpe bocht vooruit 
                else if (motor2 == 0) { motor1 += 0.6; }// scherpe bocht achteruit
            }
            else if (steer > 0.8) // naar rechts sturen
            {
                if (motor1 == 1.0) { motor2 -= 0.6; }// scherpe bocht vooruit
                else if (motor1 == 0) { motor2 += 0.6; }//scherpe bocht achteruit
            }
            final = Convert.ToString("leftEngine "+motor1+", rightEngine: "+ motor2+", rudder: "+steer);
            //Console.WriteLine(final);


            return final;

        }

    }
}  