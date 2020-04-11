using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Defines the types of messages we can use in the given parameters
    /// </summary>
    public enum MessageType
    {
        Invalid = 0,
        Heartbeat = 1,
        GameMessage = 10,
        ClientRequest = 20,
        ServerResponse = 30,
        UserLogin = 40,
        GameStatus = 50
    };

    /// <summary>
    /// Defines the base class for various message types
    /// </summary>
    public abstract class MsgBase
    {
        /// <summary>
        /// Defines the type of the message
        /// </summary>
        public MessageType msg_type;

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
            msg_type = MessageType.Invalid;
        }

        /// <summary>
        /// Constructor to force definition of the message type
        /// </summary>
        /// <param name="msg_t">The message type to associate with the message</param>
        public MsgBase(MessageType msg_t)
        {
            msg_type = msg_t;
        }
    }
}
