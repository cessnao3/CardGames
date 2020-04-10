using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    /// <summary>
    /// Defines default server response codes
    /// </summary>
    public enum ResponseCodes
    {
        Fail,
        Unauthorized,
        OK
    };

    /// <summary>
    /// Defines the default server response messages
    /// </summary>
    public class MsgServerResponse : MsgBase
    {
        /// <summary>
        /// The server response
        /// </summary>
        public ResponseCodes code;

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgServerResponse()
        {
            msg_type = MessageType.ServerResponse;
        }

        /// <summary>
        /// Checks whether message parameters are valid
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool CheckMessage()
        {
            return
                msg_type == MessageType.ServerResponse;
        }
    }
}
