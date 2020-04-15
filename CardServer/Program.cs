using System;
using System.Collections.Generic;
using System.Threading;
using CardGameLibrary.Games;
using CardGameLibrary.Messages;

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
            Dictionary<int, Games.GameLobby> lobbies = new Dictionary<int, Games.GameLobby>();
            Dictionary<int, Games.GenericGame> games = new Dictionary<int, Games.GenericGame>();

            // Setup a temporary game
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
                                    if (games.ContainsKey(req.game_id)) server.AddMessageToQueue(p, games[req.game_id].GetGameStatus());
                                    break;
                                case MsgClientRequest.RequestType.AvailableGames:
                                    {
                                        // Create a list of only the games that the player is part of
                                        List<int> player_games = new List<int>();

                                        foreach (Games.GenericGame g in games.Values)
                                        {
                                            if (g.ContainsPlayer(p.GetGamePlayer()))
                                            {
                                                player_games.Add(g.game_id);
                                            }
                                        }

                                        // Add the message to the send queue
                                        server.AddMessageToQueue(p, new MsgGameList()
                                        {
                                            lobbies = new List<int>(lobbies.Keys),
                                            games = player_games
                                        });
                                    }
                                    break;
                                case MsgClientRequest.RequestType.LobbyStatus:
                                    if (lobbies.ContainsKey(req.game_id)) server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    break;
                                case MsgClientRequest.RequestType.NewLobby:
                                    // Add a lobby, and send the lobby status back to the player
                                    lobbies.Add(
                                        current_id,
                                        new Games.GameLobby(
                                            game_id: current_id,
                                            (GameTypes)req.data));
                                    server.AddMessageToQueue(p, lobbies[current_id].GetLobbyStatus());
                                    // Increment the game ID
                                    current_id += 1;
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
                                games[play.game_id].Action(
                                    p: p.GetGamePlayer(),
                                    msg: play);

                                // Send a server response to each player in the game
                                foreach (GamePlayer gplayer in games[play.game_id].players)
                                {
                                    server.AddMessageToQueue(
                                        gplayer,
                                        games[play.game_id].GetGameStatus());
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
                        games.Add(
                            l_id,
                            lobbies[l_id].CreateGame());
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
