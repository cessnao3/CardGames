using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    public class MsgClientRequest : MsgBase
    {
        /// <summary>
        /// Request types available to the client
        /// </summary>
        public enum RequestType
        {
            AvailableGames = 0
        };

        /// <summary>
        /// The type of information to request from the client
        /// </summary>
        public RequestType request;

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgClientRequest() : base(MessageType.ClientRequest)
        {
            // Empty Constructor
        }

        public override bool CheckMessage()
        {
            return msg_type == MessageType.ClientRequest;
        }
    }
}
