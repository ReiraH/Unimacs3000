using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Websocket
{
    class Program
    {
        static void Main(String[] Args)
        {

            Console.WriteLine("Starting websocket...");
            var websocket = new Websocket("https://waterknakkers.niekeichner.nl", "scheepsbrug", "unimax");
            Thread.Sleep(4000);
            while ((true))
            {
                websocket.ControlBoat(0, 0, 1);
                Thread.Sleep(1000);
                websocket.ControlBoat(0, 0, -1);
                Thread.Sleep(1000);




            }

            //websocket.SelectBoat("Henk");
            Console.ReadKey();
            websocket.Close();

        }
    }
}
