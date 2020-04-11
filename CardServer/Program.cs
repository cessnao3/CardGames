using System;
using System.Collections.Generic;
using System.Threading;

namespace CardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Card Game Server");
            Server.Server server = new Server.Server(8088);

            List<Games.GenericGame> games = new List<Games.GenericGame>();
            List<Object> available_games;

            while (true)
            {
                server.Tick();
                Thread.Sleep(100);
            }
        }
    }
}
