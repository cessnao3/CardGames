using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CardGameLibrary.Messages;

namespace CardGameLibrary.Network
{
    /// <summary>
    /// A helper class to manage sending/receiving data over the network
    /// </summary>
    public class MessageReader
    {
        /// <summary>
        /// Determines whether to print JSON output to the terminal
        /// </summary>
        private static bool PrintOutput { get; set; } = false;

        /// <summary>
        /// Message type conversion dictionary defines what message types to use for the provided
        /// message ID values in the JSON packets
        /// </summary>
        static IReadOnlyDictionary<MessageTypeID, Type> TypeConverterDict { get; } = new Dictionary<MessageTypeID, Type>()
        {
            { MessageTypeID.ClientRequest, typeof(MsgClientRequest) },
            { MessageTypeID.GameList, typeof(MsgGameList) },
            { MessageTypeID.GamePlay, typeof(MsgGamePlay) },
            { MessageTypeID.GameStatus, typeof(MsgGameStatus) },
            { MessageTypeID.Heartbeat, typeof(MsgHeartbeat) },
            { MessageTypeID.LobbyStatus, typeof(MsgLobbyStatus) },
            { MessageTypeID.ServerResponse, typeof(MsgServerResponse) },
            { MessageTypeID.UserLogin, typeof(MsgLogin) }
        };

        /// <summary>
        /// Sets whether console printing of sent/received messages is enabled
        /// </summary>
        /// <param name="enabled">If true, console messages will be output over the consoel</param>
        static public void SetOutputPrinting(bool enabled)
        {
            PrintOutput = enabled;
        }

        /// <summary>
        /// Sends the provided message to the provided TCP client
        /// </summary>
        /// <param name="client">The client to send the message over</param>
        /// <param name="msg">The message to serialize and send</param>
        static public void SendMessage(ClientStruct client, MsgBase msg)
        {
            // Serialize the message
            string s = JsonSerializer.Serialize(msg, msg.GetType());
            if (PrintOutput) Console.WriteLine("Sending " + s);

            // Convert the message to bytes, write, and flush the stream
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            client.NetworkStream.Write(bytes, 0, bytes.Length);
            client.NetworkStream.Flush();
        }

        /// <summary>
        /// Attempts to receive a message from the provided TCP client
        /// </summary>
        /// <param name="client">The client to read the message from</param>
        /// <returns>A message if found; otherwise null</returns>
        static public MsgBase? ReadMessage(ClientStruct client)
        {
            // Check if the client has bytes availble to read
            // If not, return null
            if (client.Client.Available > 0)
            {
                // Read the string
                StringBuilder sb = new();

                char c = '\0';
                int colon_count = 0;
                while ((c != '}' || colon_count > 0) && sb.Length < 10240)
                {
                    c = (char)client.NetworkStream.ReadByte();
                    sb.Append(c);

                    if (c == '{') colon_count += 1;
                    else if (c == '}') colon_count -= 1;
                }

                string s = sb.ToString();

                // Print the string output
                if (PrintOutput) Console.WriteLine("Receiving " + s);

                // Extract the message type to use in parsing
                MessageTypeID msg_type = MessageTypeID.Invalid;

                {
                    // Parse the JSON document
                    JsonDocument msg_doc = JsonDocument.Parse(s);

                    // Pull out the message type, and if successful, extract the message type value
                    if (msg_doc.RootElement.TryGetProperty("MessageType", out JsonElement msg_type_element) &&
                        msg_type_element.TryGetInt32(out int msg_type_id_int))
                    {
                        msg_type = (MessageTypeID)msg_type_id_int;
                    }
                }

                // Define the message item
                MsgBase? msg;

                if (TypeConverterDict.ContainsKey(msg_type))
                {
                    msg = (MsgBase?)JsonSerializer.Deserialize(
                        s,
                        TypeConverterDict[msg_type]);

                    if (msg != null && !msg.CheckMessage())
                    {
                        msg = null;
                    }
                }
                else
                {
                    msg = null;
                }

                // Return the parsed message, or null if all failed
                return msg;
            }
            // Return null if no bytes available to read
            else
            {
                return null;
            }
        }
    }
}
