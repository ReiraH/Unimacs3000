using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace Websocket
{
    class Websocket
    {
        private class BoatreqMessage
        {
            public String id = "das09sad90ds90";
            public String name = "De henk boot";
        } 
        static void Main(string[] args)
        {
            Socket socket = IO.Socket("https://waterknakkers.niekeichner.nl");
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected");
               
                String json = JsonConvert.SerializeObject(new BoatreqMessage());
                Console.WriteLine(json);
                //socket.Emit("boatreq", json);
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Console.WriteLine("Disconnected");
                socket.Close();

            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () =>
            {
                Console.WriteLine("Can't connect to the server");
            });


            socket.On("controller", (data) =>
            {
                Console.WriteLine(data);
            });


            Console.ReadKey();
            socket.Close();


        }
    }
}
