using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Defines the types of messages we can use in the given parameters
    /// </summary>
    public enum MessageTypeID
    {
        Invalid = 0,
        Heartbeat = 1,
        GamePlay = 10,
        ClientRequest = 20,
        ServerResponse = 30,
        UserLogin = 40,
        GameStatus = 50,
        LobbyStatus = 60,
        GameList = 70
    };

    /// <summary>
    /// Defines the base class for various message types
    /// </summary>
    public abstract class MsgBase
    {
        /// <summary>
        /// Defines the type of the message
        /// </summary>
        public MessageTypeID MessageType { get; set; }

        /// <summary>
        /// Checks whether the message is valid
        /// </summary>
        /// <returns>true if valid; otherwise false</returns>
        public abstract bool CheckMessage();

        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        private MsgBase()
        {
            MessageType = MessageTypeID.Invalid;
        }

        /// <summary>
        /// Constructor to force definition of the message type
        /// </summary>
        /// <param name="message_type">The message type to associate with the message</param>
        public MsgBase(MessageTypeID message_type)
        {
            MessageType = message_type;
        }
    }
}
