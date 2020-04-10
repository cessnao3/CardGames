using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace CardServer.Server
{
    /// <summary>
    /// Defines the card server class to manage the TCP connections
    /// </summary>
    class Server
    {
        List<TcpClient> clients;
        TcpListener server_socket;

        Dictionary<Players.Player, TcpClient> player_clients;

        Dictionary<Players.Player, string> data_so_far;

        public Server(int port)
        {
            // Setup the socket listener
            server_socket = new TcpListener(
                IPAddress.Any,
                port);
            server_socket.Start();
            clients = new List<TcpClient>();
        }

        public void Tick()
        {
            while (server_socket.Pending())
            {
                TcpClient client = server_socket.AcceptTcpClient();
                client.ReceiveTimeout = 1000;
                clients.Add(client);
            }

            foreach (TcpClient c in clients)
            {
                if (!c.Connected)
                {
                    c.Close();
                    clients.Remove(c);
                }
                else if (c.Available > 0)
                {
                    NetworkStream ns = c.GetStream();
                }
            }
        }
    }
}
