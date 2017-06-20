using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

using System.Net.Http;
//using Unimacs_3000.Models;

namespace Websocket
{
    public class Websocket
    {
        private Socket socket;
        private string boatSelected;
        private List<Boat> boats = new List<Boat>();
        //private UnimacsContext db = new UnimacsContext();
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

        public class BoatsMessage
        {
            public List<Boat> boats;
        }

        public class BoatMessage
        {
            public Boat boat;
        }
        public class Boat
        {
            public string id;
            public string name;
        }

        public Websocket(string adress, string username, string password)
        {


            string getToken()
            {
                var httpClient = new HttpClient();

                var parameters = new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password }
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
                Console.WriteLine("Receiving boat List:");
                Console.WriteLine(data);
                BoatsMessage message = JsonConvert.DeserializeObject<BoatsMessage>(data.ToString());
                if (message.boats.Count > 0)
                {
                    boats = message.boats;
                    boatSelected = boats[0].id;
                    Console.WriteLine("Boat selected: "+boatSelected);

                }
            });

            socket.On("boatConnected", (data) =>
            {
                Console.WriteLine("Boat connected:");
                Console.WriteLine(data);
                BoatMessage message = JsonConvert.DeserializeObject<BoatMessage>(data.ToString());
                boats.Add(message.boat);
                if(boats.Count == 1)
                {
                    boatSelected = boats[0].id;
                    Console.WriteLine("Boat selected: " + boatSelected);

                }

            });

            socket.On("boatDisconnected", (data) =>
            {
                Console.WriteLine("Boat disconnected:");
                Console.WriteLine(data);
                BoatMessage message = JsonConvert.DeserializeObject<BoatMessage>(data.ToString());
                boats.Remove(message.boat);

                if(boatSelected == message.boat.id)
                {
                    Console.WriteLine("Disconnected Boat was the selected boat");
                    if(boats.Count > 0)
                    {

                        boatSelected = boats[0].id;
                        Console.WriteLine("Boat selected: "+boatSelected);
                    }
                    else
                    {
                        boatSelected = null;
                        Console.WriteLine("No boat selected.");
                    }
                    
                }

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

        public void SelectBoat(string id)
        {
            if(boats.Exists(obj => obj.id == id))
            {
                boatSelected = id;
            }
            else
            {
                Console.WriteLine("That boat isn't connected!");
            }
        }

        /*
         *  Send an instruction for remote control to the boat. Values should be between -1 and 1.
         */
        public void ControlBoat(double leftEngine, double rightEngine, double rudder)
        {

            if (leftEngine < -1 || leftEngine > 1 || rightEngine < -1 || rightEngine > 1 || rudder < -1 || rudder > 1)
            {
                leftEngine = Math.Max(-1, leftEngine);
                leftEngine = Math.Min(1, leftEngine);
                rightEngine = Math.Max(-1, rightEngine);
                rightEngine = Math.Min(1, rightEngine);
                rudder = Math.Max(-1, rudder);
                rudder = Math.Min(1, rudder);
            }

            if(boatSelected == null)
            {
                //throw new InvalidOperationException("There isn't a selected boat.");
                //Console.WriteLine("No boat connected yet!");
                //Console.WriteLine(leftEngine + " - "+ rightEngine + " - " + rudder);
                //return;
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
            //socket.Emit("controller", json);
            
            /*
            BoatMotion boatMotion = new BoatMotion();
            boatMotion.LeftEngineValue = leftEngine;
            boatMotion.RightEngineValue = rightEngine;
            boatMotion.RudderValue = rudder;
            boatMotion.Timestamp = DateTime.Now;
            db.BoatMotions.Add(boatMotion);
            db.SaveChanges();
            */
        }

    }
}
