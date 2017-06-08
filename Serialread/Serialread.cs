using System;

public class Serialread
{
   
	public Serialread(String com)
	{
        
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);
        readThread.Start();
    }

    public static void Read()
    {
        SerialPort serial = new SerialPort(com, 9600);
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
        xMovement = (float)Convert.ToDouble(_xMovement);
        yMovement = (float)Convert.ToDouble(_yMovement);
        
        //Console.WriteLine(" xmove is: " + xMovement + "ymove is :" + yMovement);
        //Console.WriteLine(xyPosition[1]+" "+xyPosition[2]);

        /*   
        x tussen 0 en 1920  orgineel -880 tot 1000
         y tussen 0 en 1630  orgineel -770 tot 900
        */
        //boatControl((xMovement + 800) / 1680, (yMovement + 880) / 1930); // stuurt het getal tussen 0 en 1
    }


}

