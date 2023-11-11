using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using CardGameLibrary.Network;
using CardGameLibrary.Messages;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Linq;

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
        sealed class ServerTuple
        {
            public ServerTuple(Players.Player player, ClientStruct client)
            {
                Player = player;
                Client = client;
            }

            public Players.Player Player { get; }
            public ClientStruct Client { get; }

            public DateTime LastReceiveTime { get; private set; } = DateTime.UtcNow;

            public void UpdateLastReceive()
            {
                LastReceiveTime = DateTime.UtcNow;
            }
        };

        /// <summary>
        /// Defines the server certificate that will be used
        /// </summary>
        X509Certificate2? ServerCertificate { get; } = null;

        /// <summary>
        /// Defines the dictionary to store client parameters with clients
        /// </summary>
        Dictionary<Players.Player, ServerTuple> Clients { get; } = new();

        /// <summary>
        /// Defines the message queue list that the server has received from clients
        /// </summary>
        Dictionary<Players.Player, List<MsgBase>> MessageReceiveQueue { get; } = new();

        /// <summary>
        /// Defines the message queue list for the server to send
        /// </summary>
        Dictionary<Players.Player, List<MsgBase>> MessageSendQueue { get; } = new();

        /// <summary>
        /// Defines the server socket to use
        /// </summary>
        readonly TcpListener ServerSocket;

        /// <summary>
        /// Creates the server to listen on the specified port
        /// </summary>
        /// <param name="port">The port to listen to message on</param>
        /// <param name="certFile">The certificate file to load to utilize SSL (optional)</param>
        public Server(int port, string? certFile = null)
        {
            // Setup the socket listener
            ServerSocket = new TcpListener(
                IPAddress.Any,
                port);
            ServerSocket.Start();

            // Load the certificate file
            if (certFile != null && certFile.Length > 0)
            {
                try
                {
                    ServerCertificate = new X509Certificate2(certFile);
                }
                catch (CryptographicException)
                {
                    ServerCertificate = null;
                }

                if (ServerCertificate == null)
                {
                    throw new ArgumentException("Unable to create certificate from provided file");
                }
                else
                {
                    Console.WriteLine("Starting server with certificate " + certFile);
                }
            }

            // Print the database file
            var dbfn = Players.PlayerDatabase.GetInstance()?.GetDatabaseFilename();
            if (dbfn != null)
            {
                Console.WriteLine($"Using {dbfn} as user database");
            }
            else
            {
                Console.WriteLine("No user file provided");
            }
        }

        /// <summary>
        /// Call the server update function
        /// </summary>
        public void Tick()
        {
            // Check for any pending socket connections
            while (ServerSocket.Pending())
            {
                // Accept the client and set the default receive timeout
                TcpClient client = ServerSocket.AcceptTcpClient();
                client.ReceiveTimeout = 1000;

                Stream clientStream;

                // Authenticate as SSL stream if provided
                if (ServerCertificate != null)
                {
                    // Create the SSL stream and attempt to authenticate
                    SslStream sslStream = new(
                        innerStream: client.GetStream(),
                        leaveInnerStreamOpen: false);

                    try
                    {
                        sslStream.AuthenticateAsServer(
                            serverCertificate: ServerCertificate,
                            clientCertificateRequired: false,
                            checkCertificateRevocation: true);
                    }
                    catch (IOException)
                    {
                        // List as unable to authenticate client
                        Console.WriteLine("Unable to authenticate client");

                        // Close the connection if unable to authenticate
                        sslStream.Close();
                        continue;
                    }

                    // Replace the stream struct with the SSL stream
                    clientStream = sslStream;
                }
                else
                {
                    clientStream = client.GetStream();
                }

                // Define the client struct
                ClientStruct clientStruct = new(client, clientStream);

                // Read the response/init string from the network stream
                MsgLogin? msg = null;
                int iterationCount = 0;
                while (msg == null && iterationCount < 10)
                {
                    try
                    {
                        MsgBase? mgsBase = MessageReader.ReadMessage(clientStruct);

                        if (mgsBase is MsgLogin msgLogin)
                        {
                            msg = msgLogin;
                        }
                    }
                    catch (IOException)
                    {
                        // Pass
                    }

                    // Sleep and wait for data
                    iterationCount += 1;
                    Thread.Sleep(100);
                }

                // Store the player object
                Players.Player? player = null;

                if (msg != null && msg.CheckMessage())
                {
                    var playerDb = Players.PlayerDatabase.GetInstance() ?? throw new NullReferenceException(nameof(Players.PlayerDatabase));

                    // Create the new user if requested
                    if (msg.Action == MsgLogin.ActionType.NewUser)
                    {
                        if (!playerDb.CreateNewPlayer(
                            name: msg.Username,
                            hash: msg.PasswordHash))
                        {
                            // Close the connection on failure
                            client.Close();
                            continue;
                        }
                    }

                    // Get the player object from the dictionary
                    player = playerDb.CheckPlayerNameHash(
                        username: msg.Username,
                        hash: msg.PasswordHash);
                }

                // If the player object is found, setup the server tuple and add to the dictionary
                if (player != null)
                {
                    // Close any existing socket
                    if (Clients.ContainsKey(player))
                    {
                        CloseConnection(Clients[player]);
                    }

                    Clients.Add(
                        player,
                        new ServerTuple(player, clientStruct));

                    MessageReader.SendMessage(clientStruct, new MsgServerResponse(player.GetGamePlayer(), MsgServerResponse.ResponseCodes.OK));

                    Console.WriteLine($"User {player.Name} connected");
                }
            }

            // Clear the output queues
            MessageReceiveQueue.Clear();

            // Loop through each of the player parameters client connection list
            foreach (var (p, c) in Clients.ToList())
            {
                // Attempt to send a heartbeat message
                if (DateTime.UtcNow - c.LastReceiveTime > TimeSpan.FromSeconds(5))
                {
                    try
                    {
                        MessageReader.SendMessage(c.Client, new MsgHeartbeat());
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Heartbeat Timeout");
                        CloseConnection(c);
                        continue;
                    }

                    c.UpdateLastReceive();
                }

                // Close the connection if not connected
                // Otherwise, read the connection result
                if (!c.Client.Client.Connected)
                {
                    CloseConnection(c);
                    continue;
                }

                // Only loop for a given iteration count limit
                int i = 0;
                while (c.Client.Client.Available > 0 && i < 10)
                {
                    // Read the message parameter
                    MsgBase? msgItem = MessageReader.ReadMessage(c.Client);

                    if (msgItem != null)
                    {
                        // Add the player to the queue if it doesn't exist
                        if (!MessageReceiveQueue.ContainsKey(p))
                        {
                            MessageReceiveQueue.Add(p, new List<MsgBase>());
                        }

                        // Add the new message to the queue
                        MessageReceiveQueue[p].Add(msgItem);

                        // Update the last receive count
                        c.UpdateLastReceive();
                    }

                    i += 1;
                }

                // Loop through to send parameters to the clients
                if (MessageSendQueue.TryGetValue(p, out var sendQueue))
                {
                    foreach (MsgBase msg in sendQueue)
                    {
                        try
                        {
                            MessageReader.SendMessage(
                                client: c.Client,
                                msg: msg);
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("Message Send Fail");
                            CloseConnection(c);
                            continue;
                        }
                    }
                }
            }

            // Clear the output queue
            MessageSendQueue.Clear();
        }

        /// <summary>
        /// Adds the provided message to the queue for the given player
        /// </summary>
        /// <param name="player">The player to add the message for</param>
        /// <param name="msg">The message to add to the queue</param>
        public void AddMessageToQueue(Players.Player player, MsgBase msg)
        {
            if (!MessageSendQueue.ContainsKey(player))
            {
                MessageSendQueue.Add(player, new List<MsgBase>());
            }

            MessageSendQueue[player].Add(msg);
        }

        /// <summary>
        /// Adds the provided message to the queue for the given player
        /// </summary>
        /// <param name="player">The player to add the message for</param>
        /// <param name="msg">The message to add to the queue</param>
        public void AddMessageToQueue(CardGameLibrary.GameParameters.GamePlayer gplayer, MsgBase msg)
        {
            Players.Player? p = Players.PlayerDatabase.GetInstance()?.GetPlayerForName(gplayer.Name);

            if (p != null)
            {
                AddMessageToQueue(
                    p,
                    msg);
            }
        }

        /// <summary>
        /// Provides the received messages dictionary since the last tick
        /// </summary>
        /// <returns>The received message dictionary</returns>
        public Dictionary<Players.Player, List<MsgBase>> GetReceivedMessages()
        {
            return MessageReceiveQueue;
        }

        /// <summary>
        /// Closes the server tuple provided
        /// </summary>
        /// <param name="st">The server tuple to close and remove from the client list</param>
        void CloseConnection(ServerTuple st)
        {
            st.Client.Close();
            Clients.Remove(st.Player);
            Console.WriteLine($"User {st.Player.Name} disconnected");
        }
    }
}
