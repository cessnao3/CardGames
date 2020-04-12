using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using GameLibrary.Messages;

namespace GameLibrary.Network
{
    public class MessageReader
    {
        protected static bool print_output = false;

        static public void SetOutputPrinting(bool enabled)
        {
            print_output = enabled;
        }

        static public void SendMessage(TcpClient client, MsgBase msg)
        {
            string s = JsonConvert.SerializeObject(msg);
            if (print_output) Console.WriteLine("Sending " + s);
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            client.GetStream().Write(bytes, 0, bytes.Length);
            client.GetStream().Flush();
        }

        static public MsgBase ReadMessage(TcpClient client)
        {
            if (client.Available > 0)
            {
                // Read the network stream
                NetworkStream ns = client.GetStream();

                // Read the string
                StringBuilder sb = new StringBuilder();

                char c = '\0';
                int colon_count = 0;
                while ((c != '}' || colon_count > 0) && sb.Length < 10240)
                {
                    c = (char)ns.ReadByte();
                    sb.Append(c);

                    if (c == '{') colon_count += 1;
                    else if (c == '}') colon_count -= 1;
                }

                string s = sb.ToString();

                // Print the string output
                if (print_output) Console.WriteLine("Receiving " + s);

                // Define the message item
                MsgBase msg_item = null;

                // Check all input  items to check for conversion
                Type[] types_to_convert = new Type[]
                {
                        typeof(MsgGamePlay),
                        typeof(MsgLogin),
                        typeof(MsgServerResponse),
                        typeof(MsgClientRequest),
                        typeof(MsgGameStatus),
                        typeof(MsgHeartbeat),
                        typeof(MsgLobbyStatus),
                        typeof(MsgGameList)
                };

                foreach (Type t in types_to_convert)
                {
                    msg_item = (MsgBase)JsonConvert.DeserializeObject(s, t);

                    if (msg_item.CheckMessage())
                    {
                        break;
                    }
                    else
                    {
                        msg_item = null;
                    }
                }

                return msg_item;
            }
            else
            {
                return null;
            }
        }
    }
}
