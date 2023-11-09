using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.Messages
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
        public ResponseCodes ResponseCode { get; set; }

        /// <summary>
        /// Defines the current user parameter
        /// </summary>
        public GameParameters.GamePlayer User { get; set; }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgServerResponse() : base(MessageTypeID.ServerResponse)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Checks whether message parameters are valid
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool CheckMessage()
        {
            return
                MessageType == MessageTypeID.ServerResponse &&
                User != null;
        }
    }
}
