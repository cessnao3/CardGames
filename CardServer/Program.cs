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
            Console.WriteLine("Starting Card Game Server");
            Server.Server server = new Server.Server(8088);

            int current_id = 0;

            Dictionary<int, Games.GenericGame> games = new Dictionary<int, Games.GenericGame>();
            games.Add(current_id, new Games.Hearts(game_id: current_id, players: new GameLibrary.Games.GamePlayer[]
            {
                new GameLibrary.Games.GamePlayer("ian"),
                new GameLibrary.Games.GamePlayer("p2"),
                new GameLibrary.Games.GamePlayer("p3"),
                new GameLibrary.Games.GamePlayer("p4")
            }));

            current_id += 1;

            //List<Object> available_games = new List<object>();

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
                                    Console.WriteLine("Unable to send available games");
                                    break;
                            }
                        }
                        else if (msg is MsgGamePlay)
                        {
                            MsgGamePlay play = (MsgGamePlay)msg;

                            if (games.ContainsKey(play.game_id))
                            {
                                games[play.game_id].Action(
                                    p: p.GetGamePlayer(),
                                    msg: play);
                                server.AddMessageToQueue(p, games[play.game_id].GetGameStatus());
                            }
                        }
                    }
                }
            }
        }
    }
}
