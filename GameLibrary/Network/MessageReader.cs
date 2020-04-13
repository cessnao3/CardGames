using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using GameLibrary.Messages;

namespace GameLibrary.Network
{
    /// <summary>
    /// A helper class to manage sending/receiving data over the network
    /// </summary>
    public class MessageReader
    {
        /// <summary>
        /// Determines whether to print JSON output to the terminal
        /// </summary>
        protected static bool print_output = false;

        /// <summary>
        /// Sets whether console printing of sent/received messages is enabled
        /// </summary>
        /// <param name="enabled">If true, console messages will be output over the consoel</param>
        static public void SetOutputPrinting(bool enabled)
        {
            print_output = enabled;
        }

        /// <summary>
        /// Sends the provided message to the provided TCP client
        /// </summary>
        /// <param name="client">The client to send the message to</param>
        /// <param name="msg">The message to serialize and send</param>
        static public void SendMessage(TcpClient client, MsgBase msg)
        {
            // Serialize the message
            string s = JsonConvert.SerializeObject(msg);
            if (print_output) Console.WriteLine("Sending " + s);

            // Convert the message to bytes, write, and flush the stream
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            client.GetStream().Write(bytes, 0, bytes.Length);
            client.GetStream().Flush();
        }

        /// <summary>
        /// Attempts to receive a message from the provided TCP client
        /// </summary>
        /// <param name="client">The TCP client to receive a message from</param>
        /// <returns>A message if found; otherwise null</returns>
        static public MsgBase ReadMessage(TcpClient client)
        {
            // Check if the client has bytes availble to read
            // If not, return null
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

                // Check all input items to check for conversion
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

                // Loop through each type and attempt to convert as a type
                foreach (Type t in types_to_convert)
                {
                    // Attempt to convert to the given message type
                    msg_item = (MsgBase)JsonConvert.DeserializeObject(s, t);

                    // If the message is valid, break the loop and deserialize as the given message
                    // Otherwise, clear the message item to try again
                    if (msg_item.CheckMessage())
                    {
                        break;
                    }
                    else
                    {
                        msg_item = null;
                    }
                }

                // Return the parsed message, or null if all failed
                return msg_item;
            }
            // Return null if no bytes available to read
            else
            {
                return null;
            }
        }
    }
}
