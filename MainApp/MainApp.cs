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
            Websocket.Websocket websocket = new Websocket.Websocket("https://waterknakkers.niekeichner.nl");
            serialread.InputController input = new serialread.InputController(websocket);
            //ModBusReader.ModBusReader reader = new ModBusReader.ModBusReader(websocket);
            while (true)
            {
                Thread.Sleep(5000);
            }
        }
    }
}
