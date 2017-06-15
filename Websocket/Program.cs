using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Websocket
{
    class Program
    {
        static void Main(String[] Args)
        {

            Console.WriteLine("Starting websocket...");
            var websocket = new Websocket("https://waterknakkers.niekeichner.nl");

            //websocket.ControlBoat(0.3, 0.4, 0.6);

            Console.ReadKey();
            websocket.Close();

        }
    }
}
