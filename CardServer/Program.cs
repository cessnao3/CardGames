using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;
using CardServer.Games;

namespace CardServer
{
    class Program
    {
        /// <summary>
        /// Defines the available lobbies
        /// </summary>
        Dictionary<int, GameLobby> Lobbies { get; } = new();

        /// <summary>
        /// Defines teh available games
        /// </summary>
        Dictionary<int, GenericGame> Games { get; } = new();

        /// <summary>
        /// Defines the current ID of the games to provide
        /// </summary>
        int CurrentId { get; set; } = 0;

        /// <summary>
        /// Defines the server object to maintain communications
        /// </summary>
        Server.Server Server { get; }

        /// <summary>
        /// Constructs a game program class
        /// </summary>
        /// <param name="cert_file">Defines the certificate filename to try to use</param>
        Program(string? cert_file = null)
        {
            // Print output and setup parameters
            Console.WriteLine("Starting Card Game Server");
            Console.WriteLine("Enabling Message Printing");
            CardGameLibrary.Network.MessageReader.SetOutputPrinting(true);

            // Attempt to start the server
            try
            {
                Server = new Server.Server(8088, certFile: cert_file);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Unable to start server due to '" + e.Message + "'");
                throw;
            }
        }

        MsgGameList GetAvailableGamesForPlayer(GamePlayer p)
        {
            // Create a list of only the games that the player is part of
            var playerGames = Games.Values
                .Where(g => g.IsActive() && g.ContainsPlayer(p))
                .Select(g => new MsgGameList.ListItem(GameIDValue: g.GameID, GameType: (int)g.GetGameType()))
                .ToList();

            // Define the lobby items
            var player_lobbies = Lobbies.Values
                .Select(l => new MsgGameList.ListItem(GameIDValue: l.GameID, GameType: (int)l.GameType))
                .ToList();

            // Add the message to the send queue
            return new MsgGameList(player_lobbies, playerGames);
        }

        void Tick()
        {
            // Tick the TCP server
            Server.Tick();

            // Loop through all received messages
            foreach (var item in Server.GetReceivedMessages())
            {
                // Extract the player
                Players.Player p = item.Key;

                // Loop through each message in the list
                foreach (MsgBase msg in item.Value)
                {
                    // Check if the message is a client request
                    if (msg is MsgClientRequest req)
                    {
                        // Switch to perform actions based on the message request type received
                        switch (req.Request)
                        {
                            case MsgClientRequest.RequestType.GameStatus:
                                if (Games.ContainsKey(req.GameID))
                                {
                                    Server.AddMessageToQueue(
                                        p,
                                        Games[req.GameID].GetGameStatus(player: p.GetGamePlayer()));
                                }
                                break;
                            case MsgClientRequest.RequestType.AvailableGames:
                                // Add the message to the send queue
                                Server.AddMessageToQueue(
                                    p,
                                    GetAvailableGamesForPlayer(p.GetGamePlayer()));
                                break;
                            case MsgClientRequest.RequestType.LobbyStatus:
                                if (Lobbies.ContainsKey(req.GameID))
                                {
                                    Server.AddMessageToQueue(
                                        p,
                                        Lobbies[req.GameID].GetLobbyStatus());
                                }
                                break;
                            case MsgClientRequest.RequestType.NewLobby:
                                {
                                    // Check if an empty lobby of the requested game type already exists
                                    bool empty_lobby_exists = false;

                                    foreach (GameLobby l in Lobbies.Values)
                                    {
                                        if (l.IsEmpty() && l.GameType == (GameTypes)req.Data)
                                        {
                                            empty_lobby_exists = true;
                                        }
                                    }

                                    // Add a lobby, and send the lobby status back to the player, if one doesn't already exist
                                    if (!empty_lobby_exists)
                                    {
                                        Lobbies.Add(
                                            CurrentId,
                                            new GameLobby(
                                                game_id: CurrentId,
                                                (GameTypes)req.Data));
                                        Server.AddMessageToQueue(
                                            p,
                                            GetAvailableGamesForPlayer(p.GetGamePlayer()));
                                        // Increment the game ID
                                        CurrentId += 1;
                                    }
                                }
                                break;
                            case MsgClientRequest.RequestType.JoinLobby:
                                // Request to join the lobby if the game ID exists, and send a lobby status as a response
                                if (Lobbies.ContainsKey(req.GameID))
                                {
                                    Lobbies[req.GameID].JoinLobby(
                                        player: p.GetGamePlayer(),
                                        pos: (LobbyPositions)req.Data);
                                    Server.AddMessageToQueue(p, Lobbies[req.GameID].GetLobbyStatus());
                                }
                                break;
                            case MsgClientRequest.RequestType.LeaveLobby:
                                // Request to leave the lobby if the game ID exists, and send a lobby status as a response
                                if (Lobbies.ContainsKey(req.GameID))
                                {
                                    Lobbies[req.GameID].LeaveLobby(player: p.GetGamePlayer());
                                    Server.AddMessageToQueue(p, Lobbies[req.GameID].GetLobbyStatus());
                                }
                                break;
                        }
                    }
                    // Parse the message as a game play
                    else if (msg is MsgGamePlay play)
                    {
                        // Check if the game exists and the game contains the given player
                        if (Games.ContainsKey(play.GameID) && play.Player.Equals(p.GetGamePlayer()))
                        {
                            // Call the action item on the given game
                            try
                            {
                                Games[play.GameID].Action(
                                    p: p.GetGamePlayer(),
                                    msg: play);
                            }
                            catch (GameException e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            // Send a server response to each player in the game
                            foreach (GamePlayer gplayer in Games[play.GameID].Players)
                            {
                                Server.AddMessageToQueue(
                                    gplayer,
                                    Games[play.GameID].GetGameStatus(gplayer));
                            }
                        }
                    }
                }
            }

            // Check for any lobbies that can be converted into games
            List<int> lobby_ids = new List<int>(Lobbies.Keys);
            foreach (int l_id in lobby_ids)
            {
                // Convert any lobbies that are ready into games
                if (Lobbies[l_id].LobbyReady())
                {
                    GenericGame? game = null;

                    try
                    {
                        game = Lobbies[l_id].CreateGame();
                    }
                    catch (GameException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    if (game != null)
                    {
                        Games.Add(
                            l_id,
                            game);
                    }
                    Lobbies.Remove(l_id);
                }
                // Remove any lobbies that have timed out
                else if (Lobbies[l_id].Timeout())
                {
                    Lobbies.Remove(l_id);
                }
            }

            // Check for any games that may be removed
            foreach (var g_id in Games.Keys.ToList())
            {
                if (Games[g_id].Timeout())
                {
                    Games.Remove(g_id);
                }
            }
        }

        /// <summary>
        /// The main intro function for the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Define input arguments
            string? cert_file = null;
            bool provide_help = false;

            // Define the user file
            string? user_database_file = null;

            // Read in the arguments
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Equals("-c"))
                {
                    if (i + 1 < args.Length)
                    {
                        cert_file = args[++i];
                    }
                    else
                    {
                        provide_help = true;
                    }
                }
                else if (args[i].Equals("-d"))
                {
                    if (i + 1 < args.Length)
                    {
                        user_database_file = args[++i];
                    }
                    else
                    {
                        provide_help = true;
                    }
                }
                else if (args[i].Equals("--help") || args[i].Equals("-h"))
                {
                    provide_help = true;
                }
                else
                {
                    Console.Write("Unknown argument \"{0:}\"", args[i]);
                    provide_help = true;
                }
            }

            // Provide help if requested
            if (provide_help)
            {
                Console.WriteLine("Card Game Server");
                Console.WriteLine("Provides a server interface for Harts and Euchre card games");
                Console.WriteLine("  Usage: [-c CertFile] [-h/--help]");
                Console.WriteLine("    -c  Allows the server to be run with a SSL certificate,");
                Console.WriteLine("        provided in CertFile, to encrypt connections");
                Console.WriteLine("    -d  Allows the server to be run with a user database file");
                Console.WriteLine("    -h  Prints this help message");
                return;
            }

            // Initialize the database
            Players.PlayerDatabase.InitDatabase(db_fname: user_database_file);

            // Defines the program class
            Program prog;
            try
            {
                prog = new Program(cert_file: cert_file);
            }
            catch (ArgumentException)
            {
                return;
            }

            // The main server loop
            while (true)
            {
                // Tick teh server
                prog.Tick();

                // Sleep to avoi dticking the server too frequently
                Thread.Sleep(100);
            }
        }
    }
}
