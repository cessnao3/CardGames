using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    /// <summary>
    /// Defines the types of messages we can use in the given parameters
    /// </summary>
    public enum MessageType
    {
        UserLogin = 0,
        GameMessage = 10,
        InformationRequest = 20,
        ServerResponse = 30
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
    }
}
