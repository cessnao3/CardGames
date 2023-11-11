using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Defines the default server response messages
    /// </summary>
    public class MsgServerResponse : MsgBase
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
        /// The server response
        /// </summary>
        [JsonInclude]
        public ResponseCodes ResponseCode { get; private set; }

        /// <summary>
        /// Defines the current user parameter
        /// </summary>
        [JsonInclude]
        public GameParameters.GamePlayer User { get; private set; }

        /// <summary>
        /// Define the JSON empty constructor
        /// </summary>
        [JsonConstructor]
        public MsgServerResponse() : base(MessageTypeID.ServerResponse)
        {
            User = new(string.Empty);
        }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgServerResponse(GameParameters.GamePlayer user, ResponseCodes resp) : base(MessageTypeID.ServerResponse)
        {
            ResponseCode = resp;
            User = user;
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
