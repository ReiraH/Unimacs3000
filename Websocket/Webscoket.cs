using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Net.Http;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Unimacs_3000.Models;

namespace Websocket
{
    /// <summary>
    /// A client that connects to the API provided by the project group working on the boat.
    /// It makes use of the Socket.IO protocol and JSON messages.
    /// Their documentation can be found here: https://github.com/Huskyhond/Waterknakkers/tree/master/api
    /// </summary>
    public class Websocket
    {
        private Socket socket;
        private Boat selectedBoat;
        private List<Boat> connectedBoats = new List<Boat>();
        private UnimacsContext db = new UnimacsContext();

        /// <summary>
        /// Object to serialize JSON messages to control a boat.
        /// </summary>
        public class MotionMessage
        {
            public string boat;     //Boat ID
            public Motion motion;

            public class Motion
            {
                public double leftEngine;
                public double rightEngine;
                public double rudder;
                public bool followQuay;
                public bool followCoords;
                public List<Tuple<double, double>> goalLocation;    //List of waypoints containing latitudes and longitudes.
            }

        }

        /// <summary>
        /// Object to deserialize the response from the server containing the login token.
        /// </summary>
        public class LoginToken
        {
            public string error;        //0 on no error, otherwise containing an error message.
            public Payload payload;

            public class Payload
            {
                public string token;    //token needed to authenticate the socket connection.
            }
        }

        /// <summary>
        /// Object to deserialize a JSON message from the server containing a list of all currently connected boats.
        /// </summary>
        public class BoatsMessage
        {
            public List<Boat> boats;
        }

        /// <summary>
        /// Object to deserialize a JSON message from the server containing a boat that connects or disconnects from the server.
        /// </summary>
        public class BoatMessage
        {
            public Boat boat;
        }

        /// <summary>
        /// Object respresenting a boat. Contains an ID, name and the current mode. The mode can be "manual" for direct control, "gps" for gps control or "quay" for autonomously following the quay.
        /// </summary>
        public class Boat
        {
            public string id;
            public string name;
            public string mode = "manual";
        }

        /// <summary>
        /// Create a websocket connection with the server.
        /// </summary>
        /// <param name="adress">Hostname of the server.</param>
        /// <param name="username">Username required to log in.</param>
        /// <param name="password">Password required to log in.</param>
        public Websocket(string adress, string username, string password)
        {
            //get the logintoken from the server
            string getToken()
            {
                var httpClient = new HttpClient();
                var parameters = new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password }
                };
                var response = httpClient.PostAsync(adress + "/login", new FormUrlEncodedContent(parameters)).Result;
                var contents = response.Content.ReadAsStringAsync().Result;

                return contents;
            }
            LoginToken token = JsonConvert.DeserializeObject<LoginToken>(getToken());

            //check the response for errors
            if (token.error != "0")
            {
                throw new Exception("Login Error: " + token.error);
            }


            //create a socket connection
            socket = IO.Socket(adress);


            //callback function on connection. Emit the logintoken to authenticate the connection.
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected, sending token for authentication");
                socket.Emit("authentication", JsonConvert.SerializeObject(token.payload));
            });

            //message from the server indicating that the client is authenticated. 
            socket.On("authenticated", () =>
            {
                //request a list with all the connected boats from the server.
                socket.Emit("getBoats");
            });

            //message from the server containing a list of all the connected boats.
            socket.On("getBoats", (data) =>
            {
                Console.WriteLine("Receiving boat List:");
                Console.WriteLine(data);
                BoatsMessage message = JsonConvert.DeserializeObject<BoatsMessage>(data.ToString());

                //save the list of connected boats and set the first boat as active.
                if (message.boats.Count > 0)
                {
                    connectedBoats = message.boats;
                    selectedBoat = connectedBoats[0];
                    Console.WriteLine("Boat selected: " + selectedBoat.name + " - ID: " + selectedBoat.id);
                }
            });

            //message from the server coontaining a new boat that connected to the server.
            socket.On("boatConnected", (data) =>
            {
                Console.WriteLine("Boat connected:");
                Console.WriteLine(data);
                BoatMessage message = JsonConvert.DeserializeObject<BoatMessage>(data.ToString());
                connectedBoats.Add(message.boat);

                //set the new boat as the selected boat if there were no other boats connected.
                if (connectedBoats.Count == 1)
                {
                    selectedBoat = connectedBoats[0];
                    Console.WriteLine("Boat selected: " + selectedBoat.name + " - ID: " + selectedBoat.id);
                }
            });

            //message from the server indicating that a boat disconnected from the server.
            socket.On("boatDisconnected", (data) =>
            {
                Console.WriteLine("Boat disconnected:");
                Console.WriteLine(data);
                BoatMessage message = JsonConvert.DeserializeObject<BoatMessage>(data.ToString());
                connectedBoats.RemoveAll(boat => boat.id == message.boat.id);

                //if the disconnected boat was the active boat, select another one or none if there aren't any.
                if (selectedBoat.id == message.boat.id)
                {
                    Console.WriteLine("Disconnected Boat was the selected boat");
                    if (connectedBoats.Count > 0)
                    {
                        selectedBoat = connectedBoats[0];
                        Console.WriteLine("Boat selected: " + selectedBoat.name + " - ID: " + selectedBoat.id);
                    }
                    else
                    {
                        selectedBoat = null;
                        Console.WriteLine("No boats connected anymore.");
                    }
                }
            });

            //message from the server containing sensordata from a boat. Not further implemented yet.
            socket.On("info", (data) =>
            {
                Console.WriteLine("Info received: ");
                Console.WriteLine(data);

            });

            //Close the socket on a disconnect.
            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Console.WriteLine("Disconnected");
                socket.Close();

            });

            //The websocket can't connect to te server.
            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () =>
            {
                Console.WriteLine("Can't connect to the server");
            });

        }

        /// <summary>
        /// Close the websocket connection.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// Set the next boat in the connected boat list as active, so that boat can be controlled.
        /// </summary>
        public void SelectNextBoat()
        {
            if (connectedBoats.Count > 0)
            {
                int indexCurrentBoat = connectedBoats.IndexOf(selectedBoat);
                int nextIndex = indexCurrentBoat % connectedBoats.Count;
                selectedBoat = connectedBoats[nextIndex];
            }
        }

        /// <summary>
        /// Set all boats to inactive, so no boats can be controlled.
        /// </summary>
        public void DeselectBoat()
        {
            selectedBoat = null;
        }

        /// <summary>
        /// Set the currently selected boat to manual control. The selected boat can not be controlled via the <c>ControlBoat</c> function.
        /// </summary>
        public void ChangeToDeskMode()
        {
            selectedBoat.mode = "manual";
        }

        /// <summary>
        /// Set the currently selected boat to GPS control. The selected boat can now be controlled via the <c>setGpsCoordinates</c> function.
        /// </summary>
        public void ChangeToGpsMode()
        {
            selectedBoat.mode = "gps";
        }

        /// <summary>
        /// Set the currently selected boat to Quay mode. The boat will now follow the quay using its ultrasound sensors.
        /// </summary>
        public void ChangeToQuayMode()
        {
            selectedBoat.mode = "quay";
            MotionMessage message = new MotionMessage()
            {
                boat = selectedBoat.id,
                motion = new MotionMessage.Motion()
                {
                    leftEngine = 0,
                    rightEngine = 0,
                    rudder = 0,
                    followQuay = true,
                    followCoords = false
                }
            };

            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            Console.WriteLine(json);
            if (selectedBoat == null)
            {
                Console.WriteLine("No boat connected yet!");
                return;
            }
            socket.Emit("controller", json);

        }

        /// <summary>
        /// Send an instruction to manually control the boat. Will only work if the currently selected boat is in "manual" mode.
        /// </summary>
        /// <param name="leftEngine">Power of the left engine. Expects a value between -1 for full backwards and 1 for full forwards.</param>
        /// <param name="rightEngine">Power of the right engine. Expects a value between -1 for full backwards and 1 for full forwards.</param>
        /// <param name="rudder">Move the rudder to steer te boat. Expects a value between -1 for full toward portside and 1 for full towards starboard.</param>
        public void ControlBoat(double leftEngine, double rightEngine, double rudder)
        {
            //only work in manual mode
            if (selectedBoat.mode == "manual")
            {
                //check input
                if (leftEngine < -1 || leftEngine > 1 || rightEngine < -1 || rightEngine > 1 || rudder < -1 || rudder > 1)
                {
                    leftEngine = Math.Max(-1, leftEngine);
                    leftEngine = Math.Min(1, leftEngine);
                    rightEngine = Math.Max(-1, rightEngine);
                    rightEngine = Math.Min(1, rightEngine);
                    rudder = Math.Max(-1, rudder);
                    rudder = Math.Min(1, rudder);
                }

                //make JSON message
                MotionMessage message = new MotionMessage()
                {
                    boat = selectedBoat.id,
                    motion = new MotionMessage.Motion()
                    {
                        leftEngine = leftEngine,
                        rightEngine = rightEngine,
                        rudder = rudder,
                        followQuay = false,
                        followCoords = false
                    }
                };

                string json = JsonConvert.SerializeObject(message, Formatting.Indented);
                Console.WriteLine(json);


                if (selectedBoat == null)
                {
                    Console.WriteLine("No boat connected yet!");
                    return;
                }
                socket.Emit("controller", json);

                //save the control info in the database so it can be shown on the webapplication.
                BoatMotion boatMotion = new BoatMotion()
                {
                    LeftEngineValue = leftEngine,
                    RightEngineValue = rightEngine,
                    RudderValue = rudder,
                    Timestamp = DateTime.Now
                };
                db.BoatMotions.Add(boatMotion);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Send a list of GPS coordinates to the currently selected boat for the boat to sail to in order. Will only work if the currently selected boat is in "gps" mode.
        /// </summary>
        /// <param name="coordinates">List of coordinates for the boat to sail towards. Coordinates are structured in a tuple containing the latitude and longitude.</param>
        public void SetGpsCoordinates(List<Tuple<double, double>> coordinates)
        {
            if (selectedBoat.mode == "gps")
            {
                MotionMessage message = new MotionMessage()
                {
                    boat = selectedBoat.id,
                    motion = new MotionMessage.Motion()
                    {
                        leftEngine = 0,
                        rightEngine = 0,
                        rudder = 0,
                        followQuay = false,
                        followCoords = true,
                        goalLocation = coordinates
                    }
                };
                string json = JsonConvert.SerializeObject(message, Formatting.Indented);
                Console.WriteLine(json);
                if (selectedBoat == null)
                {
                    Console.WriteLine("No boat connected yet!");
                    return;
                }
                socket.Emit("controller", json);
            }
        }
    }
}
