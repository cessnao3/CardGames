using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using GameLibrary.Network;

namespace CardServer.Server
{
    /// <summary>
    /// Defines the card server class to manage the TCP connections
    /// </summary>
    class Server
    {
        /// <summary>
        /// Defines the server tuple to link players, the clients, and the network stream
        /// </summary>
        class ServerTuple
        {
            public Players.Player player;
            public TcpClient client;
            public NetworkStream stream;
        };

        /// <summary>
        /// Defines the message queue item
        /// </summary>
        public class MessageQueueItem
        {
            public Players.Player player;
            public MsgBase msg;
        }

        /// <summary>
        /// Defines the dictionary to store client parameters with clients
        /// </summary>
        Dictionary<Players.Player, ServerTuple> clients = new Dictionary<Players.Player, ServerTuple>();

        /// <summary>
        /// Defines the message queue list for the server status that can be used to check server parameters
        /// </summary>
        public List<MessageQueueItem> message_queue = new List<MessageQueueItem>();

        /// <summary>
        /// Defines the server socket to use
        /// </summary>
        TcpListener server_socket;

        /// <summary>
        /// Creates the server to listen on the specified port
        /// </summary>
        /// <param name="port">The port to listen to message on</param>
        public Server(int port)
        {
            // Setup the socket listener
            server_socket = new TcpListener(
                IPAddress.Any,
                port);
            server_socket.Start();
        }

        /// <summary>
        /// Call the server update function
        /// </summary>
        public void Tick()
        {
            // Check for any pending socket connections
            while (server_socket.Pending())
            {
                // Accept the client and set the default receive timeout
                TcpClient client = server_socket.AcceptTcpClient();
                client.ReceiveTimeout = 1000;

                // Set the the network stream read timeout
                NetworkStream ns = client.GetStream();
                ns.ReadTimeout = 1000;

                // Read the response/init string from the network stream
                string response_str = ReadMessage(ns);

                // Deserialize the result
                MsgLogin msg = (MsgLogin)JsonConvert.DeserializeObject(response_str, typeof(MsgLogin));

                // Store the player object
                Players.Player player_obj = null;

                if (msg.CheckMessage())
                {
                    // Create the new user if requested
                    if (msg.action == MsgLogin.ActionType.NewUser)
                    {
                        if (!Players.PlayerDatabase.GetInstance().CreateNewPlayer(name: msg.username, hash: msg.password_hash))
                        {
                            // Close the connection on failure
                            client.Close();
                            continue;
                        }
                    }

                    // Get the player object from the dictionary
                    player_obj = Players.PlayerDatabase.GetInstance().GetPlayerForName(
                        username: msg.username,
                        hash: msg.password_hash);
                }

                // If the player object is found, setup the server tuple and add to the dictionary
                if (player_obj != null)
                {
                    clients.Add(
                        player_obj,
                        new ServerTuple()
                        {
                            player = player_obj,
                            client = client,
                            stream = ns
                        });
                    Console.WriteLine(string.Format("User {0:s} connected", player_obj.name));
                }
            }

            // Loop through each of the player parameters client connection list
            foreach (Players.Player p in new List<Players.Player>(clients.Keys))
            {
                // Extract the server tuple
                ServerTuple c = clients[p];

                // Close the connection if not connected
                // Otherwise, read the connection result
                if (!c.client.Connected)
                {
                    Close(c);
                    continue;
                }
                else if (c.client.Available > 0)
                {
                    // Read the string
                    string s = ReadMessage(c.stream);
                    Console.Out.WriteLine(s);

                    MsgBase tmp = (MsgBase)JsonConvert.DeserializeObject(s, typeof(MsgBase));

                    // Define the message item
                    MsgBase msg_item = null;

                    // Add the new message to the queue
                    message_queue.Add(new MessageQueueItem()
                    {
                        player = p,
                        msg = msg_item
                    });
                }
            }
        }

        /// <summary>
        /// Closes the server tuple provided
        /// </summary>
        /// <param name="st">The server tuple to close and remove from the client list</param>
        void Close(ServerTuple st)
        {
            st.stream.Close();
            st.client.Close();
            clients.Remove(st.player);
        }

        /// <summary>
        /// Reads a complete JSON message from the network stream
        /// </summary>
        /// <param name="ns">The network stream to read</param>
        /// <returns>A complete JSON string, if valid</returns>
        public string ReadMessage(NetworkStream ns)
        {
            string s = string.Empty;
            char c = '\0';
            while (c != '}')
            {
                c = (char)ns.ReadByte();
                s += c;
            }

            return s;
        }
    }
}
