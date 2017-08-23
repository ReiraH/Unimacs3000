using System;
using System.Threading;


namespace MainApp
{
    class MainApp
    {
        static void Main(string[] args)
        {   
            //Websocket.OnlineWebsocket websocket = new Websocket.OnlineWebsocket("https://waterknakkers.niekeichner.nl", "scheepsbrug", "unimax");
            Websocket.DummyWebsocket websocket = new Websocket.DummyWebsocket();

            serialread.InputController input = new serialread.InputController(websocket);

            Console.WriteLine("Press ESC to close the application.");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { Thread.Sleep(5); }
        }
    }
}
