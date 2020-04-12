using System;
using System.Collections.Generic;
using System.Threading;

using GameLibrary.Messages;

namespace CardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Print output and setup parameters
            Console.WriteLine("Starting Card Game Server");
            Console.WriteLine("Enabling Message Printing");
            GameLibrary.Network.MessageReader.SetOutputPrinting(true);

            // Start the Server
            Server.Server server = new Server.Server(8088);

            // Setup the games and lobbies lists
            Dictionary<int, Games.GameLobby> lobbies = new Dictionary<int, Games.GameLobby>();
            Dictionary<int, Games.GenericGame> games = new Dictionary<int, Games.GenericGame>();

            // Setup a temporary game
            int current_id = 0;

            while (true)
            {
                server.Tick();
                Thread.Sleep(100);

                foreach (var item in server.GetReceivedMessages())
                {
                    Players.Player p = item.Key;

                    foreach (MsgBase msg in item.Value)
                    {
                        if (msg is MsgClientRequest)
                        {
                            MsgClientRequest req = (MsgClientRequest)msg;

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
                                    lobbies.Add(
                                        current_id,
                                        new Games.GameLobby(
                                            game_id: current_id,
                                            (GameLibrary.Games.GameTypes)req.data));
                                    server.AddMessageToQueue(p, lobbies[current_id].GetLobbyStatus());
                                    current_id += 1;
                                    break;
                                case MsgClientRequest.RequestType.JoinLobby:
                                    if (lobbies.ContainsKey(req.game_id))
                                    {
                                        lobbies[req.game_id].JoinLobby(
                                            player: p.GetGamePlayer(),
                                            pos: (GameLibrary.Games.LobbyPositions)req.data);
                                        server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    }
                                    break;
                                case MsgClientRequest.RequestType.LeaveLobby:
                                    if (lobbies.ContainsKey(req.game_id))
                                    {
                                        lobbies[req.game_id].LeaveLobby(player: p.GetGamePlayer());
                                        server.AddMessageToQueue(p, lobbies[req.game_id].GetLobbyStatus());
                                    }
                                    break;
                            }
                        }
                        else if (msg is MsgGamePlay)
                        {
                            MsgGamePlay play = (MsgGamePlay)msg;

                            if (games.ContainsKey(play.game_id) && play.player.Equals(p.GetGamePlayer()))
                            {
                                games[play.game_id].Action(
                                    p: p.GetGamePlayer(),
                                    msg: play);
                                foreach (GameLibrary.Games.GamePlayer gplayer in games[play.game_id].players)
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
                    if (lobbies[l_id].LobbyReady())
                    {
                        games.Add(
                            l_id,
                            lobbies[l_id].CreateGame());
                        lobbies.Remove(l_id);
                    }
                    else if (lobbies[l_id].Timeout())
                    {
                        lobbies.Remove(l_id);
                    }
                }
            }
        }
    }
}
