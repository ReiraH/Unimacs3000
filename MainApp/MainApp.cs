using ModBusReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace MainApp
{
    class MainApp
    {
        static void Main(string[] args)
        {   
            Websocket.Websocket websocket = new Websocket.Websocket("https://waterknakkers.niekeichner.nl", "scheepsbrug", "unimax");
            serialread.InputController input = new serialread.InputController(websocket);
            //ModBusReader.ModBusReader reader = new ModBusReader.ModBusReader(websocket);

            Console.WriteLine("Press ESC to close the application.");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
}
