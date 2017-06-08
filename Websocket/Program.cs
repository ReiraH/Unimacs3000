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
            Websocket websocket = new Websocket("https://waterknakkers.niekeichner.nl");
            Console.WriteLine("Socket started.");

            Console.ReadKey();
            websocket.Close();

        }
    }
}
