using System;
using System.Threading;

namespace CardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Card Game Server");
            Server.Server server = new Server.Server(8088);

            while (true)
            {
                server.Tick();
                Thread.Sleep(100);
            }
        }
    }
}
