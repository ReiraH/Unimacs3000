using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;


public class Serialread
{
    Boatcontrol boat;
    
    public Serialread()
    {
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);
        readThread.Start();
    }

    public static void Read()
    {
        SerialPort serial = new SerialPort("COM4", 9600);
        Boolean _continue = true;
        String joyStickControlY;
        String joyStickControlX;
        string serialdata;


        serial.Open();
        while (_continue)
        {
            try
            {
                serialdata = serial.ReadLine();
                IList<string> serialsplit = serialdata.Split(',').Reverse().ToList<string>();

                
                joyStickControlY = serialsplit[2];
                joyStickControlX = serialsplit[1];


                joyStickControl(joyStickControlX,joyStickControlY);
            }
            catch (TimeoutException) { }
        }
    }


   






    public static void joyStickControl(String _xMovement,String _yMovement )// joystick heeft orgineel waarde x-880 tot 1000,-770 tot 900
    {
        Boatcontrol boatcontrol;
        float xMovement;
        float yMovement;

        xMovement = (float)Convert.ToDouble(_xMovement);
        yMovement = (float)Convert.ToDouble(_yMovement);
        boatcontrol = new Boatcontrol((xMovement + 800) / 1680, (yMovement + 880) / 1930);// checken ofdit werkt
        //boatControl((xMovement + 800) / 1680, (yMovement + 880) / 1930); // stuurt het getal tussen 0 en 1
    }


}

