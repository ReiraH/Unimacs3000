using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MainApp
{
    class MainApp
    {
        static void Main(string[] args)
        {
            Websocket.Websocket websocket = new Websocket.Websocket("https://waterknakkers.niekeichner.nl");
            serialread.InputController input = new serialread.InputController(websocket);
            Console.ReadKey();
        }
    }
}
