﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using GameLibrary.Network;

namespace CardServer.Server
{
    /// <summary>
    /// Defines the card server class to manage the TCP connections
    /// </summary>
    class Server
    {
        class ServerTuple
        {
            public Players.Player player;
            public TcpClient client;
            public NetworkStream stream;
        };

        Dictionary<Players.Player, ServerTuple> clients;
        TcpListener server_socket;

        public Server(int port)
        {
            // Setup the socket listener
            server_socket = new TcpListener(
                IPAddress.Any,
                port);
            server_socket.Start();
            clients = new Dictionary<Players.Player, ServerTuple>();
        }

        public void Tick()
        {
            while (server_socket.Pending())
            {
                TcpClient client = server_socket.AcceptTcpClient();
                client.ReceiveTimeout = 1000;

                NetworkStream ns = client.GetStream();
                ns.ReadTimeout = 1000;

                string response_str = ReadMessage(ns);

                GameConnectionMessage msg = (GameConnectionMessage)JsonConvert.DeserializeObject(response_str, typeof(GameConnectionMessage));

                Players.Player player_obj = null;

                if (msg.action == GameConnectionMessage.ActionType.NewUser)
                {
                    if (!Players.PlayerDatabase.GetInstance().NewPlayer(name: msg.username, hash: msg.password_hash))
                    {
                        client.Close();
                        continue;
                    }
                }

                player_obj = Players.PlayerDatabase.GetInstance().GetPlayerForName(
                    username: msg.username,
                    hash: msg.password_hash);

                if (player_obj != null)
                {
                    clients.Add(
                        player_obj,
                        new ServerTuple()
                        {
                            player = player_obj,
                            client = client,
                            stream = ns
                        });
                }
            }

            List<Players.Player> players = new List<Players.Player>(clients.Keys);
            foreach (Players.Player p in players)
            {
                ServerTuple c = clients[p];

                if (!c.client.Connected)
                {
                    Close(c);
                    continue;
                }
                else if (c.client.Available > 0)
                {
                    c.stream.ReadByte();
                }
            }
        }

        void Close(ServerTuple st)
        {
            st.stream.Close();
            st.client.Close();
            clients.Remove(st.player);
        }

        public string ReadMessage(NetworkStream ns)
        {
            string s = string.Empty;
            char c = '\0';
            while (c != '}')
            {
                c = (char)ns.ReadByte();
                s += c;
            }

            return s;
        }
    }
}
