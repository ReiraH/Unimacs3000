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
    public class Websocket
    {
        private Socket socket;
        private string boatSelected;

        public class MotionMessage
        {
           public string boat;
           public Motion motion;

            public class Motion
            {
                public double leftEngine;
                public double rightEngine;
                public double rudder;
            }

        }

        public class LoginToken
        {
            public string error;
            public Payload payload;

            public class Payload
            {
                public string token;
            }
        }

        public Websocket(string adress)
        {


            string getToken()
            {
                var httpClient = new HttpClient();

                var parameters = new Dictionary<string, string>
                {
                    { "username", "scheepsbrug" },
                    { "password", "unimax" }
                };

                var resp =  httpClient.PostAsync(adress+"/login", new FormUrlEncodedContent(parameters)).Result;
                var contents = resp.Content.ReadAsStringAsync().Result;

                return contents;
            }


            LoginToken token = JsonConvert.DeserializeObject<LoginToken>(getToken());

            if (token.error != "0") 
            {
                throw new Exception("Login Error: " + token.error);
            }
            socket = IO.Socket(adress);


            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected, sending token for authentication");
                socket.Emit("authentication", JsonConvert.SerializeObject(token.payload));
             });


            socket.On("authenticated", () =>
            {
                Console.WriteLine("Ma Boi i'm in");
                socket.Emit("getBoats");



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



            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Console.WriteLine("Disconnected");
                socket.Close();

            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () =>
            {
                Console.WriteLine("Can't connect to the server");
            });

        }


        public void Close()
        {
            socket.Close();
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

            if(boatSelected == null)
            {
                throw new InvalidOperationException("There isn't a selected boat.");
            }
            MotionMessage message = new MotionMessage()
            {
                boat = boatSelected,
                motion = new MotionMessage.Motion()
                {
                    leftEngine = leftEngine,
                    rightEngine = rightEngine,
                    rudder = rudder
                }
            };

            
            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            Console.WriteLine(json);
            socket.Emit("controller", json);



        }

    }
}
