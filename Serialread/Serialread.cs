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
        Boatcontrol boatcontrol;
        String _xMovement;
        String _yMovement;
        float xMovement;
        float yMovement;




        IList<string> xyPosition = s.Split(',').Reverse().ToList<string>();
        _yMovement = xyPosition[2];
        _xMovement = xyPosition[1];
        xMovement = (float)Convert.ToDouble(_xMovement);
        yMovement = (float)Convert.ToDouble(_yMovement);
        boatcontrol = new Boatcontrol((xMovement + 800) / 1680, (yMovement + 880) / 1930);// checken ofdit werkt
        //boatControl((xMovement + 800) / 1680, (yMovement + 880) / 1930); // stuurt het getal tussen 0 en 1
    }


}

