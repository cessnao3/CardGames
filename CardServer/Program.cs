using System;
using System.Collections.Generic;
using System.Threading;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;
using CardServer.Games;

namespace CardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Print output and setup parameters
            Console.WriteLine("Starting Card Game Server");
            Console.WriteLine("Enabling Message Printing");
            CardGameLibrary.Network.MessageReader.SetOutputPrinting(true);

            // Start the Server
            Server.Server server = new Server.Server(8088);

            // Setup the games and lobbies lists
            Dictionary<int, GameLobby> lobbies = new Dictionary<int, GameLobby>();
            Dictionary<int, GenericGame> games = new Dictionary<int, GenericGame>();

            // Setup the starting ID
            int current_id = 0;

            while (true)
            {
                // Tick the TCP server
                server.Tick();

                // Loop through all received messages
                foreach (var item in server.GetReceivedMessages())
                {
                    // Extract the player
                    Players.Player p = item.Key;

                    // Loop through each message in the list
                    foreach (MsgBase msg in item.Value)
                    {
                        // Check if the message is a client request
                        if (msg is MsgClientRequest)
                        {
                            // Type cast
                            MsgClientRequest req = (MsgClientRequest)msg;

                            // Switch to perform actions based on the message request type received
                            switch (req.request)
                            {
                                case MsgClientRequest.RequestType.GameStatus:
                                    if (games.ContainsKey(req.game_id))
                                    {
                                        server.AddMessageToQueue(
                                            p,
                                            games[req.game_id].GetGameStatus(player: p.GetGamePlayer()));
                                    }
                                    break;
                                case MsgClientRequest.RequestType.AvailableGames:
                                    {
                                        // Create a list of only the games that the player is part of
                                        List<MsgGameList.ListItem> player_games = new List<MsgGameList.ListItem>();

                                        foreach (GenericGame g in games.Values)
                                        {
                                            if (g.IsActive() && g.ContainsPlayer(p.GetGamePlayer()))
                                            {
                                                player_games.Add(new MsgGameList.ListItem()
                                                {
                                                    id_val = g.game_id,
                                                    game_type = (int)g.GetGameType()
                                                });
                                            }
                                        }

                                        // Define the lobby items
                                        List<MsgGameList.ListItem> player_lobbies = new List<MsgGameList.ListItem>();

                                        foreach (GameLobby l in lobbies.Values)
                                        {
                                            player_lobbies.Add(new MsgGameList.ListItem()
                                            {
                                                id_val = l.game_id,
                                                game_type = (int)l.game_type
                                            });
                                        }

                                        // Add the message to the send queue
                                        server.AddMessageToQueue(p, new MsgGameList()
                                        {
                                            lobbies = player_lobbies,
                                            games = player_games
                                        });
                                    }
                                    break;
                                case MsgClientRequest.RequestType.LobbyStatus:
                                    if (lobbies.ContainsKey(req.game_id)) server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    break;
                                case MsgClientRequest.RequestType.NewLobby:
                                    {
                                        // Check if an empty lobby of the requested game type already exists
                                        bool empty_lobby_exists = false;

                                        foreach (GameLobby l in lobbies.Values)
                                        {
                                            if (l.IsEmpty() && l.game_type == (GameTypes)req.data)
                                            {
                                                empty_lobby_exists = true;
                                            }
                                        }

                                        // Add a lobby, and send the lobby status back to the player, if one doesn't already exist
                                        if (!empty_lobby_exists)
                                        {
                                            lobbies.Add(
                                                current_id,
                                                new GameLobby(
                                                    game_id: current_id,
                                                    (GameTypes)req.data));
                                            server.AddMessageToQueue(p, lobbies[current_id].GetLobbyStatus());
                                            // Increment the game ID
                                            current_id += 1;
                                        }
                                    }
                                    break;
                                case MsgClientRequest.RequestType.JoinLobby:
                                    // Request to join the lobby if the game ID exists, and send a lobby status as a response
                                    if (lobbies.ContainsKey(req.game_id))
                                    {
                                        lobbies[req.game_id].JoinLobby(
                                            player: p.GetGamePlayer(),
                                            pos: (LobbyPositions)req.data);
                                        server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    }
                                    break;
                                case MsgClientRequest.RequestType.LeaveLobby:
                                    // Request to leave the lobby if the game ID exists, and send a lobby status as a response
                                    if (lobbies.ContainsKey(req.game_id))
                                    {
                                        lobbies[req.game_id].LeaveLobby(player: p.GetGamePlayer());
                                        server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    }
                                    break;
                            }
                        }
                        // Parse the message as a game play
                        else if (msg is MsgGamePlay)
                        {
                            // Type cast
                            MsgGamePlay play = (MsgGamePlay)msg;

                            // Check if the game exists and the game contains the given player
                            if (games.ContainsKey(play.game_id) && play.player.Equals(p.GetGamePlayer()))
                            {
                                // Call the action item on the given game
                                try
                                {
                                    games[play.game_id].Action(
                                        p: p.GetGamePlayer(),
                                        msg: play);
                                }
                                catch (GameException e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                                // Send a server response to each player in the game
                                foreach (GamePlayer gplayer in games[play.game_id].players)
                                {
                                    server.AddMessageToQueue(
                                        gplayer,
                                        games[play.game_id].GetGameStatus(gplayer));
                                }
                            }
                        }
                    }
                }

                // Check for any lobbies that can be converted into games
                List<int> lobby_ids = new List<int>(lobbies.Keys);
                foreach (int l_id in lobby_ids)
                {
                    // Convert any lobbies that are ready into games
                    if (lobbies[l_id].LobbyReady())
                    {
                        Games.GenericGame game = null;

                        try
                        {
                            game = lobbies[l_id].CreateGame();
                        }
                        catch (GameException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        if (game != null)
                        {
                            games.Add(
                                l_id,
                                game);
                        }
                        lobbies.Remove(l_id);
                    }
                    // Remove any lobbies that have timed out
                    else if (lobbies[l_id].Timeout())
                    {
                        lobbies.Remove(l_id);
                    }
                }

                // Check for any games that may be removed
                List<int> game_ids = new List<int>(games.Keys);
                foreach (int g_id in game_ids)
                {
                    if (games[g_id].Timeout())
                    {
                        games.Remove(g_id);
                    }
                }

                // Sleep to avoi dticking the server too frequently
                Thread.Sleep(100);
            }
        }
    }
}
