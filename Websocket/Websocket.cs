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
    class Websocket : IWebsocket
    {
        private class BoatreqMessage
        {
            public String id = "das09sad90ds90";
            public String name = "De henk boot";
        }

        private Socket socket;

        public Websocket(String adress)
        {
            socket = IO.Socket(adress);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected");
                socket.Emit("getBoats");
                
                //code to subscribe as a boat on the server, for initial testing
                //String json = JsonConvert.SerializeObject(new BoatreqMessage());
                //Console.WriteLine(json);
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


            socket.On("getBoats", (data) =>
            {
                Console.WriteLine("Message received");
                Console.WriteLine(data);
            });

            socket.On("boatConnected", (data) =>
            {
                Console.WriteLine("Message received");
                Console.WriteLine(data);
            });

            socket.On("boatDisconnected", (data) =>
            {
                Console.WriteLine("Message received");
                Console.WriteLine(data);
            });

        }


        public void Close()
        {
            socket.Close();
        }

        void IWebsocket.Send(string message)
        {
            throw new NotImplementedException();
        }

        
    }
}
