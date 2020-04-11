using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Simple heartbeat message to test whether the connection is still valid
    /// </summary>
    public class MsgHeartbeat : MsgBase
    {
        /// <summary>
        /// Constuctor to provide a basic heartbeat message
        /// </summary>
        public MsgHeartbeat() : base(MessageType.Heartbeat)
        {
            // Empty constructor
        }

        /// <summary>
        /// Determines if the message is valid
        /// </summary>
        /// <returns>Returns true if the expected type is correct</returns>
        public override bool CheckMessage()
        {
            return msg_type == MessageType.Heartbeat;
        }
    }
}
