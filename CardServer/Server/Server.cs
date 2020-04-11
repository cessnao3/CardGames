using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GameLibrary.Network;
using GameLibrary.Messages;

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
                MsgLogin msg = null;
                int iter_lim = 0;
                while (msg == null && iter_lim < 10)
                {
                    try
                    {
                        MsgBase msg_base = MessageReader.ReadMessage(client);

                        if (msg_base is MsgLogin)
                        {
                            msg = (MsgLogin)msg_base;
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        // Pass
                    }

                    // Sleep and wait for data
                    iter_lim += 1;
                    Thread.Sleep(100);
                }

                // Store the player object
                Players.Player player_obj = null;

                if (msg != null && msg.CheckMessage())
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
                    // Close any existing socket
                    if (clients.ContainsKey(player_obj))
                    {
                        CloseConnection(clients[player_obj]);
                    }

                    clients.Add(
                        player_obj,
                        new ServerTuple()
                        {
                            player = player_obj,
                            client = client
                        });

                    MessageReader.SendMessage(client, new MsgServerResponse()
                    {
                        code = ResponseCodes.OK
                    });

                    Console.WriteLine(string.Format("User {0:s} connected", player_obj.name));
                }
            }

            // Loop through each of the player parameters client connection list
            foreach (Players.Player p in new List<Players.Player>(clients.Keys))
            {
                // Extract the server tuple
                ServerTuple c = clients[p];

                // Attempt to send a heartbeat message
                try
                {
                    MessageReader.SendMessage(c.client, new MsgHeartbeat());
                }
                catch (System.IO.IOException)
                {
                    CloseConnection(c);
                    continue;
                }

                // Close the connection if not connected
                // Otherwise, read the connection result
                if (!c.client.Connected)
                {
                    CloseConnection(c);
                    continue;
                }

                // Only loop for a given iteration count limit
                int i = 0;
                while (c.client.Available > 0 && i < 10)
                {
                    // Read the message parameter
                    MsgBase msg_item = MessageReader.ReadMessage(c.client);

                    if (msg_item != null)
                    {
                        // Add the new message to the queue
                        message_queue.Add(new MessageQueueItem()
                        {
                            player = p,
                            msg = msg_item
                        });
                    }

                    i += 1;
                }
            }
        }

        /// <summary>
        /// Closes the server tuple provided
        /// </summary>
        /// <param name="st">The server tuple to close and remove from the client list</param>
        void CloseConnection(ServerTuple st)
        {
            st.client.Close();
            clients.Remove(st.player);
            Console.WriteLine(string.Format("User {0:s} disconnected", st.player.name));
        }
    }
}
