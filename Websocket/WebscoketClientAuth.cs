using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

using System.Net.Http;

namespace Websocket
{
    public class WebsocketClientAuth : IWebsocket
    {
        private class BoatreqMessage
        {
            public String id = "das09sad90ds90";
            public String name = "De henk boot";
        }

        private Socket socket;
        private static readonly HttpClient client = new HttpClient();


        public WebsocketClientAuth(String adress)
        {
            /*
            //Create Socket options
            var option = new IO.Options
            {
                
            }
            */

            string GetResponseString()
            {
                var httpClient = new HttpClient();

                var parameters = new Dictionary<string, string>
                {
                    { "username", "scheepsbrug" },
                    { "password", "unimax" }
                };

                var resp =  httpClient.PostAsync("https://waterknakkers.niekeichner.nl/login", new FormUrlEncodedContent(parameters)).Result;
                var contents = resp.Content.ReadAsStringAsync().Result;

                return contents;
            }


            var token = GetResponseString();




            socket = IO.Socket(adress);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected, sending token for authentication: "+token);
                socket.Emit("authentication", token);
             });

            socket.On("authenticated", () =>
            {
                Console.WriteLine("Ma Boi i'm in");
                socket.Emit("getBoats");


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

        public void Send(string message)
        {
            throw new NotImplementedException();
        }


        /*
         *  Send an instruction for remote control to the boat. Values should be between 0 and 1.
         */
        public void ControlBoat(double leftEngine, double rightEngine, double rudder)
        {
            if (leftEngine < 0 || leftEngine > 1 || rightEngine < 0 || rightEngine > 1 || rudder < 0 || rudder > 1)
            {
                throw new ArgumentOutOfRangeException("ControlBoat must be called with all parameters between 0 and 1.");
            }

            Dictionary<string, double> values = new Dictionary<string, double>
            {
                { "leftEngine", leftEngine },
                { "rightEngine", rightEngine },
                { "rudder", rudder }
            };

            string json = JsonConvert.SerializeObject(values, Formatting.Indented);
            Console.WriteLine(json);
            //socket.Emit("", json);

            //Boat, ID
            //Motion, json

        }

    }
}
