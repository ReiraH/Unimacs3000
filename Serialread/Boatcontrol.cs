using System;

public class Boatcontrol
{
    public Boatcontrol(double xMovement, double yMovement)
    {
        //bla
        double steer = Math.Round(yMovement, 1);
        double motor1 = Math.Round(xMovement, 1);
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
        final = Convert.ToString("leftEngine " + motor1 + ", rightEngine: " + motor2 + ", rudder: " + steer);


    }
}
