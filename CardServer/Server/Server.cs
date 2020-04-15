using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CardGameLibrary.Network;
using CardGameLibrary.Messages;

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
            public Players.Player player = null;
            public ClientStruct client_struct = null;
            public DateTime last_receive = DateTime.UtcNow;
        };

        /// <summary>
        /// Defines the dictionary to store client parameters with clients
        /// </summary>
        Dictionary<Players.Player, ServerTuple> clients = new Dictionary<Players.Player, ServerTuple>();

        /// <summary>
        /// Defines the message queue list that the server has received from clients
        /// </summary>
        Dictionary<Players.Player, List<MsgBase>> message_receive_queue = new Dictionary<Players.Player, List<MsgBase>>();

        /// <summary>
        /// Defines the message queue list for the server to send
        /// </summary>
        Dictionary<Players.Player, List<MsgBase>> message_send_queue = new Dictionary<Players.Player, List<MsgBase>>();

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

                // Define the client struct
                ClientStruct client_struct = new ClientStruct(client);

                // Read the response/init string from the network stream
                MsgLogin msg = null;
                int iter_lim = 0;
                while (msg == null && iter_lim < 10)
                {
                    try
                    {
                        MsgBase msg_base = MessageReader.ReadMessage(client_struct);

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
                        if (!Players.PlayerDatabase.GetInstance().CreateNewPlayer(
                            name: msg.username,
                            hash: msg.password_hash))
                        {
                            // Close the connection on failure
                            client.Close();
                            continue;
                        }
                    }

                    // Get the player object from the dictionary
                    player_obj = Players.PlayerDatabase.GetInstance().CheckPlayerNameHash(
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
                            client_struct = client_struct
                        });

                    MessageReader.SendMessage(client_struct, new MsgServerResponse()
                    {
                        code = ResponseCodes.OK,
                        user = player_obj.GetGamePlayer()
                    });

                    Console.WriteLine(string.Format("User {0:s} connected", player_obj.name));
                }
            }

            // Clear the output queues
            message_receive_queue.Clear();

            // Loop through each of the player parameters client connection list
            foreach (Players.Player p in new List<Players.Player>(clients.Keys))
            {
                // Extract the server tuple
                ServerTuple c = clients[p];

                // Attempt to send a heartbeat message
                if (DateTime.UtcNow - c.last_receive > TimeSpan.FromSeconds(5))
                {
                    try
                    {
                        MessageReader.SendMessage(c.client_struct, new MsgHeartbeat());
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("Heartbeat Timeout");
                        CloseConnection(c);
                        continue;
                    }

                    c.last_receive = DateTime.UtcNow;
                }

                // Close the connection if not connected
                // Otherwise, read the connection result
                if (!c.client_struct.client.Connected)
                {
                    CloseConnection(c);
                    continue;
                }

                // Only loop for a given iteration count limit
                int i = 0;
                while (c.client_struct.client.Available > 0 && i < 10)
                {
                    // Read the message parameter
                    MsgBase msg_item = MessageReader.ReadMessage(c.client_struct);

                    if (msg_item != null)
                    {
                        // Add the player to the queue if it doesn't exist
                        if (!message_receive_queue.ContainsKey(p))
                        {
                            message_receive_queue.Add(p, new List<MsgBase>());
                        }

                        // Add the new message to the queue
                        message_receive_queue[p].Add(msg_item);

                        // Update the last receive count
                        c.last_receive = DateTime.UtcNow;
                    }

                    i += 1;
                }

                // Loop through to send parameters to the clients
                if (message_send_queue.ContainsKey(p))
                {
                    foreach (MsgBase msg in message_send_queue[p])
                    {
                        try
                        {
                            MessageReader.SendMessage(
                                client: c.client_struct,
                                msg: msg);
                        }
                        catch (System.IO.IOException)
                        {
                            Console.WriteLine("Message Send Fail");
                            CloseConnection(c);
                            continue;
                        }
                    }
                }
            }

            // Clear the output queue
            message_send_queue.Clear();
        }

        /// <summary>
        /// Adds the provided message to the queue for the given player
        /// </summary>
        /// <param name="player">The player to add the message for</param>
        /// <param name="msg">The message to add to the queue</param>
        public void AddMessageToQueue(Players.Player player, MsgBase msg)
        {
            if (!message_send_queue.ContainsKey(player))
            {
                message_send_queue.Add(player, new List<MsgBase>());
            }

            message_send_queue[player].Add(msg);
        }

        /// <summary>
        /// Adds the provided message to the queue for the given player
        /// </summary>
        /// <param name="player">The player to add the message for</param>
        /// <param name="msg">The message to add to the queue</param>
        public void AddMessageToQueue(CardGameLibrary.Games.GamePlayer gplayer, MsgBase msg)
        {
            Players.Player p = Players.PlayerDatabase.GetInstance().GetPlayerForName(gplayer.name);

            if (p != null)
            {
                AddMessageToQueue(
                    p,
                    msg);
            }
        }



        public Dictionary<Players.Player, List<MsgBase>> GetReceivedMessages()
        {
            return message_receive_queue;
        }

        /// <summary>
        /// Closes the server tuple provided
        /// </summary>
        /// <param name="st">The server tuple to close and remove from the client list</param>
        void CloseConnection(ServerTuple st)
        {
            st.client_struct.Close();
            clients.Remove(st.player);
            Console.WriteLine(string.Format("User {0:s} disconnected", st.player.name));
        }
    }
}
