using System;
using System.Threading;

namespace CardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server");
            Server.Server server = new Server.Server(8088);

            while (true)
            {
                //Console.WriteLine("Tick...");
                server.Tick();
                Thread.Sleep(100);
            }
        }
    }
}
